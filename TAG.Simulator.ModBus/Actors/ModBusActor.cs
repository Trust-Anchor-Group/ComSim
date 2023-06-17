using TAG.Simulator.ObjectModel.Actors;

namespace TAG.Simulator.ModBus.Actors
{
	/// <summary>
	/// Abstract base class for ModBus actors.
	/// </summary>
	public abstract class ModBusActor : Actor
	{
		/// <summary>
		/// http://lab.tagroot.io/Schema/ComSim.xsd
		/// </summary>
		public const string ComSimModBusNamespace = "http://lab.tagroot.io/Schema/ComSim/ModBus.xsd";

		/// <summary>
		/// Resource name of ModBus schema.
		/// </summary>
		public const string ComSimModBusSchema = "TAG.Simulator.ModBus.Schema.ComSimModBus.xsd";

		/// <summary>
		/// Abstract base class for ModBus actors.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public ModBusActor(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Abstract base class for ModBus actors.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <param name="InstanceIndex">Instance index.</param>
		/// <param name="InstanceId">ID of instance</param>
		public ModBusActor(ISimulationNode Parent, Model Model, int InstanceIndex, string InstanceId)
			: base(Parent, Model, InstanceIndex, InstanceId)
		{
		}

		/// <summary>
		/// XML Namespace where the element is defined.
		/// </summary>
		public override string Namespace => ComSimModBusNamespace;

		/// <summary>
		/// Points to the embedded XML Schema resource defining the semantics of the XML namespace.
		/// </summary>
		public override string SchemaResource => ComSimModBusSchema;
	}
}
