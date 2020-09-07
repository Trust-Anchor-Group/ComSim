using System;
using TAG.Simulator.ObjectModel.Actors;
using Waher.Networking.XMPP;

namespace TAG.Simulator.XMPP
{
	/// <summary>
	/// XMPP Actor connecting to the XMPP network using web-sockets.
	/// </summary>
	public class XmppActorWebSocket : XmppActor
	{
		/// <summary>
		/// XMPP Actor connecting to the XMPP network using web-sockets.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		public XmppActorWebSocket(ISimulationNode Parent)
			: base(Parent)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "XmppActorWebSocket";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent)
		{
			return new XmppActorWebSocket(Parent);
		}
	}
}
