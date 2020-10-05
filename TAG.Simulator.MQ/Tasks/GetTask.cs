using System;
using System.Threading.Tasks;

namespace TAG.Simulator.MQ.Tasks
{
	/// <summary>
	/// Gets a message on a queue.
	/// </summary>
	internal class GetTask : MqTask
	{
		private readonly TaskCompletionSource<string> result;
		private readonly string queue;
		private readonly int timeoutMilliseconds;

		/// <summary>
		/// Gets a message on a queue.
		/// </summary>
		/// <param name="Client">MQ Client</param>
		/// <param name="Queue">Queue Name</param>
		/// <param name="TimeoutMilliseconds">Timeout, in milliseconds.</param>
		public GetTask(MqClient Client, string Queue, int TimeoutMilliseconds)
			: base(Client)
		{
			this.queue = Queue;
			this.timeoutMilliseconds = TimeoutMilliseconds;
			this.result = new TaskCompletionSource<string>();
		}

		/// <summary>
		/// Completion task.
		/// </summary>
		public Task<string> Completed => this.result.Task;

		/// <summary>
		/// Performs work defined by the task.
		/// </summary>
		/// <returns>If work should be continued (true), or if it is completed (false).</returns>
		public override bool DoWork()
		{
			try
			{
				string Message = this.Client.GetOne(this.queue, this.timeoutMilliseconds);
				this.result.TrySetResult(Message);
			}
			catch (Exception ex)
			{
				this.result.TrySetException(ex);
			}

			return false;
		}
	}
}
