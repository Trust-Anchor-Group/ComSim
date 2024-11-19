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
		/// <param name="Client">MQ Client</param>
		/// <param name="Queue">Queue Name</param>
		/// <param name="Message">Message to put.</param>
		public PutTask(MqClient Client, string Queue, string Message)
			: base(Client)
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
		/// <returns>If work should be continued (true), or if it is completed (false).</returns>
		public override Task<bool> DoWork()
		{
			try
			{
				this.Client.Put(this.queue, this.message);
				this.result.TrySetResult(true);
			}
			catch (Exception ex)
			{
				this.result.TrySetException(ex);
			}

			return Task.FromResult(false);
		}
	}
}
