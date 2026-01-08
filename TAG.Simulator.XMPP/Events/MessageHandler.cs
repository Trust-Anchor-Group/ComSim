using System.Collections.Generic;
using System.Threading.Tasks;
using TAG.Simulator.ObjectModel.Actors;
using Waher.Networking.XMPP;

namespace TAG.Simulator.XMPP.Events
{
	/// <summary>
	/// Custom message handler.
	/// </summary>
	public class MessageHandler : HandlerNode
	{
		/// <summary>
		/// Custom message handler.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public MessageHandler(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(MessageHandler);

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new MessageHandler(Parent, Model);
		}

		/// <summary>
		/// Registers handlers on the XMPP Client.
		/// </summary>
		/// <param name="Actor">Actor instance reference</param>
		/// <param name="Client">XMPP Client</param>
		public override void RegisterHandlers(IActor Actor, XmppClient Client)
		{
			Client.RegisterMessageHandler(this.Name, this.HandlerNamespace, async (sender, e) =>
			{
				await this.Trigger(Actor, new KeyValuePair<string, object>(string.IsNullOrEmpty(this.EventArgs) ? "e" : this.EventArgs, e));
			}, true);
		}

	}
}
