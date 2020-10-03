using System;
using System.Threading;
using System.Threading.Tasks;
using IBM.WMQ;
using Waher.Events;

namespace TAG.Simulator.MQ.Tasks
{
	/// <summary>
	/// Subscribes to messages frmo a queue.
	/// </summary>
	internal class SubscriptionTask : MqTask
	{
		private readonly ManualResetEvent cancel;
		private readonly TaskCompletionSource<bool> stopped;
		private readonly MqMessageEventHandler callback;
		private readonly string queue;
		private readonly object state;

		/// <summary>
		/// Subscribes to messages frmo a queue.
		/// </summary>
		public SubscriptionTask(string Queue, ManualResetEvent Cancel, TaskCompletionSource<bool> Stopped,
			MqMessageEventHandler Callback, object State)
			: base()
		{
			this.queue = Queue;
			this.cancel = Cancel;
			this.stopped = Stopped;
			this.callback = Callback;
			this.state = State;
		}

		/// <summary>
		/// <see cref="IDisposable"/>
		/// </summary>
		public override void Dispose()
		{
			this.stopped?.TrySetResult(true);
			base.Dispose();
		}

		/// <summary>
		/// Performs work defined by the task.
		/// </summary>
		/// <param name="Client">MQ Client</param>
		/// <returns>If work should be continued (true), or if it is completed (false).</returns>
		public override bool DoWork(MqClient Client)
		{
			if (this.cancel?.WaitOne(0) ?? false)
			{
				Client.Information("Cancelling subscription to messages from " + this.queue);
				this.stopped?.TrySetResult(true);
				return false;
			}
			else
			{
				string Message = Client.GetOne(this.queue, 1000);

				if (!(Message is null))
				{
					try
					{
						this.callback(Client, new MqMessageEventArgs(Client, Message, this.state));
					}
					catch (Exception ex)
					{
						Log.Critical(ex);
					}
				}

				return true;
			}
		}
	}
}
