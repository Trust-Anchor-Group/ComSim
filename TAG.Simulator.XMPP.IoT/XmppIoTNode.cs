using System;
using TAG.Simulator.ObjectModel;

namespace TAG.Simulator.XMPP.IoT
{
	/// <summary>
	/// Abstract base class for IoT XMPP simulation nodes.
	/// </summary>
	public abstract class XmppIoTNode : SimulationNodeChildren
	{
		internal const string XmppIoTSchemaResource = "TAG.Simulator.XMPP.IoT.Schema.ComSimXmppIoT.xsd";
		internal const string XmppIoTNamespace = "http://trustanchorgroup.com/Schema/ComSim/XMPPIoT.xsd";

		/// <summary>
		/// Abstract base class for IoT XMPP simulation nodes.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public XmppIoTNode(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Points to the embedded XML Schema resource defining the semantics of the XML namespace.
		/// </summary>
		public override string SchemaResource => XmppIoTSchemaResource;

		/// <summary>
		/// XML Namespace where the element is defined.
		/// </summary>
		public override string Namespace => XmppIoTNamespace;
	}
}
