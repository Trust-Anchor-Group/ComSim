using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel.Actors;
using Waher.Content.Xml;

namespace TAG.Simulator.ModBus.Actors
{
	/// <summary>
	/// Represents a simulated ModBus device
	/// </summary>
	public class ModBusDevice : ModBusActor
	{
		private readonly Dictionary<ushort, ModBusDevice> devicesPerAddress = new Dictionary<ushort, ModBusDevice>();
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
			return Task.FromResult<Actor>(new ModBusDevice(this, this.Model, InstanceIndex, InstanceId)
			{
				startAddress = this.startAddress,
				instanceAddress = (byte)(this.startAddress + InstanceIndex - 1)
			});
		}

		/// <summary>
		/// Starts the node.
		/// </summary>
		public override Task Start()
		{
			lock (this.devicesPerAddress)
			{
				this.devicesPerAddress.Clear();

				if (!(this.Instances is null))
				{
					foreach (IActor Actor in this.Instances)
					{
						if (Actor is ModBusDevice Device)
						{
							if (this.devicesPerAddress.ContainsKey(Device.instanceAddress))
								throw new Exception("Multiple devices with the same address: " + Device.instanceAddress.ToString());

							this.devicesPerAddress[Device.instanceAddress] = Device;
						}
					}
				}
			}

			return base.Start();
		}

		/// <summary>
		/// Finalizes the node after simulation.
		/// </summary>
		public override Task Finalize()
		{
			lock (this.devicesPerAddress)
			{
				this.devicesPerAddress.Clear();
			}

			return base.Finalize();
		}

		/// <summary>
		/// Initializes an instance of an actor.
		/// </summary>
		public override Task InitializeInstance()
		{
			return Task.CompletedTask;
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
			return Task.CompletedTask;
		}

		/// <summary>
		/// Finds the instance corresponding to a unit address.
		/// </summary>
		/// <param name="Address">Unit address</param>
		/// <returns>Instance, if found.</returns>
		public ModBusDevice FindInstance(ushort Address)
		{
			lock (this.devicesPerAddress)
			{
				if (this.devicesPerAddress.TryGetValue(Address, out ModBusDevice Instance))
					return Instance;
			}

			return null;
		}
	}
}
