using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel.Actors;
using Waher.Content.Xml;
using Waher.Networking.Modbus;

namespace TAG.Simulator.ModBus.Actors
{
	/// <summary>
	/// Hosts a ModBus IP Gateway
	/// </summary>
	public class ModBusServer : ModBusActor
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
		/// Hosts a ModBus IP Gateway
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <param name="InstanceIndex">Instance index.</param>
		/// <param name="InstanceId">ID of instance</param>
		public ModBusServer(ISimulationNode Parent, Model Model, int InstanceIndex, string InstanceId)
			: base(Parent, Model, InstanceIndex, InstanceId)
		{
		}

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
		/// Creates an instance of the actor.
		/// 
		/// Note: Parent of newly created actor should point to this node (the creator of the instance object).
		/// </summary>
		/// <param name="InstanceIndex">Instance index.</param>
		/// <param name="InstanceId">ID of instance</param>
		/// <returns>Actor instance.</returns>
		public override Task<Actor> CreateInstanceAsync(int InstanceIndex, string InstanceId)
		{
			return Task.FromResult<Actor>(new ModBusServer(this, this.Model, InstanceIndex, InstanceId)
			{
				port = (ushort)(this.port + InstanceIndex - 1)
			});
		}

		/// <summary>
		/// Initializes an instance of an actor.
		/// </summary>
		public override async Task InitializeInstance()
		{
			this.server	= await ModBusTcpServer.CreateAsync(this.port);
		}

		/// <summary>
		/// Starts an instance of an actor.
		/// </summary>
		public override Task StartInstance()
		{
			return Task.CompletedTask;
		}

		/// <summary>
		/// Finalizes an instance of an actor.
		/// </summary>
		public override Task FinalizeInstance()
		{
			this.server?.Dispose();
			this.server = null;

			return Task.CompletedTask;
		}
	}
}
