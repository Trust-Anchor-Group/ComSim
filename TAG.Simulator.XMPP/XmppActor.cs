using System;
using TAG.Simulator.ObjectModel.Actors;
using Waher.Networking.XMPP;

namespace TAG.Simulator.XMPP
{
	/// <summary>
	/// Abstract base class for XMPP actors.
	/// </summary>
	public abstract class XmppActor : Actor
	{
		private XmppClient client;

		/// <summary>
		/// Abstract base class for XMPP actors.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		public XmppActor(ISimulationNode Parent)
			: base(Parent)
		{
		}

		/// <summary>
		/// XML Namespace where the element is defined.
		/// </summary>
		public override string Namespace => "http://trustanchorgroup.com/Schema/ComSim/XMPP.xsd";

		/// <summary>
		/// Points to the embedded XML Schema resource defining the semantics of the XML namespace.
		/// </summary>
		public override string SchemaResource => "TAG.Simulator.XMPP.Schema.ComSimXmpp.xsd";
	}
}
