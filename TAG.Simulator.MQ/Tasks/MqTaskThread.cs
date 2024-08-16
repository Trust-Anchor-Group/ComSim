using System;
using System.Collections.Generic;
using System.Threading;
using Waher.Events;

namespace TAG.Simulator.MQ.Tasks
{
	/// <summary>
	/// Manages a thread executing MQ-related tasks.
	/// </summary>
	internal class MqTaskThread
	{
		private readonly Thread thread;
		private readonly AutoResetEvent newItem;
		private readonly LinkedList<MqTask> tasks = new LinkedList<MqTask>();
		private readonly WaitHandle[] waitHandles;

		public MqTaskThread(ManualResetEvent Terminating)
		{
			this.newItem = new AutoResetEvent(false);
			this.waitHandles = new WaitHandle[] { this.newItem, Terminating };

			this.thread = new Thread(this.TaskExecutor)
			{
				Name = "MQ Async Thread",
				Priority = ThreadPriority.BelowNormal
			};

			this.thread.Start();
		}

		public void Execute(MqTask Task)
		{
			lock (tasks)
			{
				this.tasks.AddLast(Task);
			}

			this.newItem.Set();
		}

		private void TaskExecutor()
		{
			try
			{
				MqTask Task;
				bool Working = true;

				while (true)
				{
					switch (WaitHandle.WaitAny(this.waitHandles, 1000))
					{
						case 0: // New item
							Working = true;
							while (true)
							{
								lock (tasks)
								{
									if (tasks.First is null)
										break;
									else
									{
										Task = tasks.First.Value;
										tasks.RemoveFirst();
									}
								}

								if (Task.DoWork())
								{
									lock (tasks)
									{
										tasks.AddLast(Task);
									}
								}
							}
							break;

						case 1: // Terminating
							return;

						default:
							if (Working)
							{
								Working = false;
								MqTasks.Idle(this);
							}
							break;
					}
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex);
			}
		}
	}
}
