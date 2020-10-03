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
	internal class SubscriptionTask : MqTask, IDisposable
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
		public void Dispose()
		{
			this.stopped?.TrySetResult(true);
		}

		/// <summary>
		/// Performs work defined by the task.
		/// </summary>
		/// <param name="Client">MQ Client</param>
		public override void DoWork(MqClient Client)
		{
			if (this.cancel?.WaitOne(0) ?? false)
			{
				Client.Information("Cancelling subscription to messages from " + this.queue);
				this.stopped?.TrySetResult(true);
			}
			else
			{
				string Message = Client.GetOne(this.queue, 10);

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

				Client.QueueTask(this);
			}
		}
	}
}
