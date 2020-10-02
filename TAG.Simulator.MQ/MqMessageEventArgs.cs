using System;
using System.Threading.Tasks;

namespace TAG.Simulator.MQ
{
	/// <summary>
	/// Delegate for MQ Message event handlers or callback methods.
	/// </summary>
	/// <param name="Sender">Sender of event.</param>
	/// <param name="e">Event arguments</param>
	public delegate Task MqMessageEventHandler(object Sender, MqMessageEventArgs e);

	/// <summary>
	/// Event arguments for MQ Message events.
	/// </summary>
	public class MqMessageEventArgs : EventArgs
	{
		private readonly MqClient client;
		private readonly string message;
		private readonly object state;

		/// <summary>
		/// Event arguments for MQ Message events.
		/// </summary>
		/// <param name="Client">MQ Client</param>
		/// <param name="Message">Message text</param>
		/// <param name="State">State object</param>
		public MqMessageEventArgs(MqClient Client, string Message, object State)
		{
			this.client = Client;
			this.message = Message;
			this.state = State;
		}

		/// <summary>
		/// MQ Client object
		/// </summary>
		public MqClient Client => this.client;

		/// <summary>
		/// Message text
		/// </summary>
		public string Message => this.message;

		/// <summary>
		/// State object
		/// </summary>
		public object State => this.state;
	}
}
