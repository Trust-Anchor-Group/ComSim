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
		public GetTask(string Queue, int TimeoutMilliseconds)
			: base()
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
		/// <param name="Client">MQ Client</param>
		public override void DoWork(MqClient Client)
		{
			try
			{
				string Message = Client.GetOne(this.queue, this.timeoutMilliseconds);
				this.result.TrySetResult(Message);
			}
			catch (Exception ex)
			{
				this.result.TrySetException(ex);
			}
		}
	}
}
