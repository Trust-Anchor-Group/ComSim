using TAG.Simulator.ObjectModel.Activities;
using TAG.Simulator.XMPP.Legal.Extensions;

namespace TAG.Simulator.XMPP.Legal.Activities
{
	/// <summary>
	/// Abstract base class for legal activity nodes
	/// </summary>
	public abstract class LegalActivityNode : ActivityNode
	{
		/// <summary>
		/// Abstract base class for legal activity nodes
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public LegalActivityNode(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Points to the embedded XML Schema resource defining the semantics of the XML namespace.
		/// </summary>
		public override string SchemaResource => LegalExtension.XmppSchema;

		/// <summary>
		/// XML Namespace where the element is defined.
		/// </summary>
		public override string Namespace => LegalExtension.XmppNamespace;
	}
}
