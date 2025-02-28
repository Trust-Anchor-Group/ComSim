﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Waher.Events;

namespace TAG.Simulator.MQ.Tasks
{
	/// <summary>
	/// Subscribes to messages frmo a queue.
	/// </summary>
	/// <param name="Client">MQ Client</param>
	/// <param name="Queue">Quene Name</param>
	/// <param name="Callback">Event object that can be used to cancel subscription.</param>
	/// <param name="Cancel">Event object that can be used to wait for the subscription to be stopped.</param>
	/// <param name="State">Method to call when messages are received.</param>
	/// <param name="Stopped">State object to pass on to callback method.</param>
	internal class SubscriptionTask(MqClient Client, string Queue, ManualResetEvent Cancel, TaskCompletionSource<bool> Stopped,
		EventHandlerAsync<MqMessageEventArgs> Callback, object State) 
		: MqTask(Client)
	{
		private readonly ManualResetEvent cancel = Cancel;
		private readonly TaskCompletionSource<bool> stopped = Stopped;
		private readonly EventHandlerAsync<MqMessageEventArgs> callback = Callback;
		private readonly string queue = Queue;
		private readonly object state = State;

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
		/// <returns>If work should be continued (true), or if it is completed (false).</returns>
		public override async Task<bool> DoWork()
		{
			if (this.cancel?.WaitOne(0) ?? false)
			{
				this.Client.Information("Cancelling subscription to messages from " + this.queue);
				this.stopped?.TrySetResult(true);
				return false;
			}
			else
			{
				string Message = this.Client.GetOne(this.queue, 1000);

				if (Message is not null)
					await this.callback.Raise(this.Client, new MqMessageEventArgs(this.Client, Message, this.state));

				return true;
			}
		}
	}
}
