using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ModBus.Registers;
using TAG.Simulator.ObjectModel.Actors;
using Waher.Content.Xml;

namespace TAG.Simulator.ModBus.Actors
{
	/// <summary>
	/// Represents a simulated ModBus device
	/// </summary>
	public class ModBusDevice : ModBusActor
	{
		private byte startAddress;
		private byte instanceAddress;

		/// <summary>
		/// Represents a simulated ModBus device
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public ModBusDevice(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Represents a simulated ModBus device
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <param name="InstanceIndex">Instance index.</param>
		/// <param name="InstanceId">ID of instance</param>
		public ModBusDevice(ISimulationNode Parent, Model Model, int InstanceIndex, string InstanceId)
			: base(Parent, Model, InstanceIndex, InstanceId)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(ModBusDevice);

		/// <summary>
		/// Instance address
		/// </summary>
		public byte InstanceAddress => this.instanceAddress;

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new ModBusDevice(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.startAddress = (byte)XML.Attribute(Definition, "startAddress", 0);

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
			ModBusDevice Result = new ModBusDevice(this, this.Model, InstanceIndex, InstanceId)
			{
				startAddress = this.startAddress,
				instanceAddress = (byte)(this.startAddress + InstanceIndex - 1)
			};

			foreach (ISimulationNode Node in this.Children)
			{
				if (Node is ModBusRegister Register)
					Result.AddChild(Register.Copy(Result, this.Model));
			}

			return Task.FromResult<Actor>(Result);
		}

		/// <summary>
		/// Initializes an instance of an actor.
		/// </summary>
		public override async Task InitializeInstance()
		{
			foreach (ISimulationNode Node in this.Children)
				await Node.Initialize();
		}

		/// <summary>
		/// Starts an instance of an actor.
		/// </summary>
		public override Task StartInstance()
		{
			if (this.Parent.Parent is ModBusServer Server)
			{
				foreach (ISimulationNode Node in this.Children)
				{
					if (Node is ModBusRegister Register)
						Register.RegisterRegister(this.instanceAddress, Server);
				}
			}

			return Task.CompletedTask;
		}

		/// <summary>
		/// Finalizes an instance of an actor.
		/// </summary>
		public override Task FinalizeInstance()
		{
			if (this.Parent.Parent is ModBusServer Server)
			{
				foreach (ISimulationNode Node in this.Children)
				{
					if (Node is ModBusRegister Register)
						Register.UnregisterRegister(Server);
				}
			}

			return Task.CompletedTask;
		}
	}
}
