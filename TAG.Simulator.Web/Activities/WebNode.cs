using TAG.Simulator.ObjectModel.Activities;
using TAG.Simulator.Web.Actors;

namespace TAG.Simulator.Web.Activities
{
	/// <summary>
	/// Abstract base class for web activity nodes.
	/// </summary>
	public abstract class WebNode : ActivityNode
	{
		/// <summary>
		/// Abstract base class for web activity nodes.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public WebNode(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Points to the embedded XML Schema resource defining the semantics of the XML namespace.
		/// </summary>
		public override string SchemaResource => WebActor.WebSchema;

		/// <summary>
		/// XML Namespace where the element is defined.
		/// </summary>
		public override string Namespace => WebActor.WebNamespace;
	}
}
