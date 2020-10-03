using System;
using System.Threading.Tasks;

namespace TAG.Simulator.MQ.Tasks
{
	/// <summary>
	/// Puts a message on a queue.
	/// </summary>
	internal class PutTask : MqTask
	{
		private readonly TaskCompletionSource<bool> result;
		private readonly string queue;
		private readonly string message;

		/// <summary>
		/// Puts a message on a queue.
		/// </summary>
		public PutTask(string Queue, string Message)
			: base()
		{
			this.queue = Queue;
			this.message = Message;
			this.result = new TaskCompletionSource<bool>();
		}

		/// <summary>
		/// Completion task.
		/// </summary>
		public Task Completed => this.result.Task;

		/// <summary>
		/// Performs work defined by the task.
		/// </summary>
		/// <param name="Client">MQ Client</param>
		/// <returns>If work should be continued (true), or if it is completed (false).</returns>
		public override bool DoWork(MqClient Client)
		{
			try
			{
				Client.Put(this.queue, this.message);
				this.result.TrySetResult(true);
			}
			catch (Exception ex)
			{
				this.result.TrySetException(ex);
			}

			return false;
		}
	}
}
