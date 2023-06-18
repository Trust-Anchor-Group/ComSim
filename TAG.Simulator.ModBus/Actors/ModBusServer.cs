using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel;
using TAG.Simulator.ObjectModel.Actors;
using Waher.Content.Xml;
using Waher.Networking.Modbus;

namespace TAG.Simulator.ModBus.Actors
{
	/// <summary>
	/// Hosts a ModBus IP Gateway
	/// </summary>
	public class ModBusServer : SimulationNodeChildren, IActors
	{
		private ModBusTcpServer server;
		private int port;

		/// <summary>
		/// Hosts a ModBus IP Gateway
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public ModBusServer(ISimulationNode Parent, Model Model)
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
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(ModBusServer);

		/// <summary>
		/// Reference to the ModBus TCP server object.
		/// (Only available on initialized instances.)
		/// </summary>
		public ModBusTcpServer Server => this.server;

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new ModBusServer(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.port = XML.Attribute(Definition, "port", 502);

			return base.FromXml(Definition);
		}

		/// <summary>
		/// Registers an actor with the collection of actors.
		/// </summary>
		/// <param name="Actor">Actor</param>
		public void Register(IActor Actor)
		{
			if (this.Parent is IActors Actors)
				Actors.Register(Actor);
		}

		/// <summary>
		/// Initialized the node before simulation.
		/// </summary>
		public override async Task Initialize()
		{
			this.server	= await ModBusTcpServer.CreateAsync(this.port);

			await base.Initialize();
		}

		/// <summary>
		/// Starts the node.
		/// </summary>
		public override Task Start()
		{
			return base.Start();
		}

		/// <summary>
		/// Finalizes the node after simulation.
		/// </summary>
		public override Task Finalize()
		{
			this.server.Dispose();
			this.server = null;

			return base.Finalize();
		}
	}
}
