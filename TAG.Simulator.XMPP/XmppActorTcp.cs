using System;
using TAG.Simulator.ObjectModel.Actors;
using Waher.Networking.XMPP;

namespace TAG.Simulator.XMPP
{
	/// <summary>
	/// XMPP Actor connecting to the XMPP network using traditional TCP (c2s).
	/// </summary>
	public class XmppActorTcp : XmppActor
	{
		/// <summary>
		/// XMPP Actor connecting to the XMPP network using traditional TCP (c2s).
		/// </summary>
		/// <param name="Parent">Parent node</param>
		public XmppActorTcp(ISimulationNode Parent)
			: base(Parent)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "XmppActorTcp";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent)
		{
			return new XmppActorTcp(Parent);
		}
	}
}
