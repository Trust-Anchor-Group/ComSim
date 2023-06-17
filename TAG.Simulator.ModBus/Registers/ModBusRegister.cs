using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ModBus.Actors;
using TAG.Simulator.ObjectModel;
using Waher.Content.Xml;

namespace TAG.Simulator.ModBus.Registers
{
	/// <summary>
	/// Abstract base class for ModBus registers.
	/// </summary>
	public abstract class ModBusRegister : SimulationNode
	{
		private ushort register;

		/// <summary>
		/// Abstract base class for ModBus registers.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public ModBusRegister(ISimulationNode Parent, Model Model)
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

		/// <summary>
		/// Register reference
		/// </summary>
		public ushort Register => this.register;

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.register = (ushort)XML.Attribute(Definition, "register", 0);

			return Task.CompletedTask;
		}
	}
}
