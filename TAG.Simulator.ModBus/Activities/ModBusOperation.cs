using TAG.Simulator.ModBus.Actors;
using TAG.Simulator.ObjectModel.Activities;

namespace TAG.Simulator.ModBus.Activities
{
	/// <summary>
	/// Abstract base class for ModBus operations.
	/// </summary>
	public abstract class ModBusOperation : ActivityNode
	{
		/// <summary>
		/// Abstract base class for ModBus operations.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public ModBusOperation(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// XML Namespace where the element is defined.
		/// </summary>
		public override string Namespace => ModBusActor.ComSimModBusNamespace;

		/// <summary>
		/// Points to the embedded XML Schema resource defining the semantics of the XML namespace.
		/// </summary>
		public override string SchemaResource => ModBusActor.ComSimModBusSchema;
	}
}
