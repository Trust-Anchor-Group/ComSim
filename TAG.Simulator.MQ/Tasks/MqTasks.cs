using System;
using System.Collections.Generic;
using System.Threading;

namespace TAG.Simulator.MQ.Tasks
{
	/// <summary>
	/// Static class, managing MQ-related tasks that must be executed.
	/// </summary>
	internal static class MqTasks
	{
		private static readonly ManualResetEvent terminating = new ManualResetEvent(false);
		private static readonly LinkedList<MqTaskThread> threads = new LinkedList<MqTaskThread>();
		private static bool terminated = false;

		static MqTasks()
		{
			Model.OnFinalizing += Model_OnFinalizing;
		}

		private static void Model_OnFinalizing(object sender, EventArgs e)
		{
			terminated = true;
			terminating.Set();
		}

		internal static void ExecuteTask(MqTask Item)
		{
			if (terminated)
				return;

			MqTaskThread T;

			lock (threads)
			{
				if (threads.First is null)
					T = null;
				else
				{
					T = threads.First.Value;
					threads.RemoveFirst();
				}
			}

			if (T is null)
				T = new MqTaskThread(terminating);

			T.Execute(Item);
		}

		internal static void Idle(MqTaskThread Thread)
		{
			lock (threads)
			{
				threads.AddLast(Thread);
			}
		}
	}
}
