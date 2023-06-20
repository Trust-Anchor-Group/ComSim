using System;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ModBus.Actors;
using TAG.Simulator.ObjectModel.Events;
using Waher.Content.Xml;

namespace TAG.Simulator.ModBus.Registers
{
	/// <summary>
	/// Abstract base class for ModBus registers.
	/// </summary>
	public abstract class ModBusRegister : ExternalEventsNode
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
		public ushort RegisterNr => this.register;

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.register = (ushort)XML.Attribute(Definition, "register", 0);

			return base.FromXml(Definition);
		}

		/// <summary>
		/// Registers the node on the ModBus server object instance.
		/// </summary>
		/// <param name="Server">ModBus server object instance.</param>
		public abstract void RegisterRegister(ModBusServer Server);

		/// <summary>
		/// Unregisters the node from the ModBus server object instance.
		/// </summary>
		/// <param name="Server">ModBus server object instance.</param>
		public abstract void UnregisterRegister(ModBusServer Server);

		/// <summary>
		/// ID of collection node.
		/// </summary>
		public override string Id
		{
			get
			{
				if (this.Parent is ModBusDevice Device)
					return Device.Id + "_" + this.register.ToString();
				else
					return this.register.ToString();
			}
		}

		/// <summary>
		/// Starts the node.
		/// </summary>
		public override Task Start()
		{
			if (this.Parent.Parent is ModBusServer Server)
				this.RegisterRegister(Server);

			return base.Start();
		}

		/// <summary>
		/// Finalizes the node after simulation.
		/// </summary>
		public override Task Finalize()
		{
			if (this.Parent.Parent is ModBusServer Server)
				this.UnregisterRegister(Server);

			return base.Finalize();
		}

		/// <summary>
		/// Finds the instance corresponding to a unit address.
		/// </summary>
		/// <param name="Address">Unit address</param>
		/// <returns>Instance, if found.</returns>
		public ModBusDevice FindInstance(ushort Address)
		{
			if (!(this.Parent is ModBusDevice Device))
				return null;

			return Device;
		}
	}
}
