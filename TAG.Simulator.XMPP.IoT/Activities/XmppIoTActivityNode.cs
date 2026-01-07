using TAG.Simulator.ObjectModel.Activities;

namespace TAG.Simulator.XMPP.IoT.Activities
{
	/// <summary>
	/// Abstract base class for IoT XMPP activity nodes.
	/// </summary>
	public abstract class XmppIoTActivityNode : ActivityNode
	{
		/// <summary>
		/// Abstract base class for IoT XMPP activity nodes.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public XmppIoTActivityNode(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Points to the embedded XML Schema resource defining the semantics of the XML namespace.
		/// </summary>
		public override string SchemaResource => XmppIoTNode.XmppIoTSchemaResource;

		/// <summary>
		/// XML Namespace where the element is defined.
		/// </summary>
		public override string Namespace => XmppIoTNode.XmppIoTNamespace;
	}
}
