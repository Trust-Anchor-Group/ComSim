using System;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ModBus.Actors;
using TAG.Simulator.ObjectModel;
using TAG.Simulator.ObjectModel.Activities;
using Waher.Script;

namespace TAG.Simulator.ModBus.Activities
{
	/// <summary>
	/// Abstract base class for ModBus operations.
	/// </summary>
	public abstract class ModBusOperation : ActivityNode
	{
		/// <summary>
		/// ModBus client to use in communication.
		/// </summary>
		protected StringAttribute client;

		/// <summary>
		/// ModBus device Address attribute
		/// </summary>
		protected DoubleAttribute address;

		/// <summary>
		/// ModBus register attribute
		/// </summary>
		protected DoubleAttribute register;

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

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			foreach (XmlAttribute Attr in Definition.Attributes)
			{
				switch (Attr.Name)
				{
					case "client":
						this.client = new StringAttribute(Attr.Value);
						break;

					case "address":
						this.address = new DoubleAttribute(Attr.Name, Attr.Value);
						break;

					case "register":
						this.register = new DoubleAttribute(Attr.Name, Attr.Value);
						break;
				}
			}

			return base.FromXml(Definition);
		}

		/// <summary>
		/// Gets the ModBus TCP Client to use in the operation.
		/// </summary>
		/// <param name="Variables">Current variables.</param>
		/// <returns>ModBus Client reference.</returns>
		public async Task<ModBusClient> GetClient(Variables Variables)
		{
			object Obj = await this.Model.GetActorObjectAsync(this.client, Variables);
			if (!(Obj is ModBusClient Client))
				throw new Exception("Client ID does not point to a ModBus client.");

			return Client;
		}
	}
}
