using System;
using TAG.Simulator.ObjectModel.Actors;
using Waher.Networking.XMPP;

namespace TAG.Simulator.XMPP
{
	/// <summary>
	/// XMPP Actor connecting to the XMPP network using web-sockets.
	/// </summary>
	public class XmppActorWebSocket : XmppActorEndpoint
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
		/// XMPP Actor connecting to the XMPP network using web-sockets.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="InstanceIndex">Instance index.</param>
		/// <param name="InstanceId">ID of instance</param>
		public XmppActorWebSocket(ISimulationNode Parent, int InstanceIndex, string InstanceId)
			: base(Parent, InstanceIndex, InstanceId)
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

		/// <summary>
		/// Creates an instance object of the XMPP actor, and initializes it.
		/// </summary>
		/// <param name="InstanceIndex">Instance index.</param>
		/// <param name="InstanceId">ID of instance</param>
		/// <returns>Actor instance.</returns>
		protected override XmppActor CreateInstanceObject(int InstanceIndex, string InstanceId)
		{
			return new XmppActorWebSocket(this.Parent, InstanceIndex, InstanceId);
		}

		/// <summary>
		/// Type of XRD link representing endpoint.
		/// </summary>
		protected override string EndpointType => "urn:xmpp:alt-connections:websocket";
	}
}
