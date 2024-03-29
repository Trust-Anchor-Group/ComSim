﻿using System;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel.Actors;
using Waher.Content.Xml;
using Waher.Networking.Modbus;
using Waher.Networking.Sniffers;

namespace TAG.Simulator.ModBus.Actors
{
	/// <summary>
	/// Represents a simulated ModBus client
	/// </summary>
	public class ModBusClient : ModBusActor
	{
		private readonly SemaphoreSlim synchObj = new SemaphoreSlim(1);
		private ModbusTcpClient client;
		private ISniffer sniffer;
		private string host;
		private int port;

		/// <summary>
		/// Represents a simulated ModBus client
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public ModBusClient(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Represents a simulated ModBus client
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <param name="InstanceIndex">Instance index.</param>
		/// <param name="InstanceId">ID of instance</param>
		public ModBusClient(ISimulationNode Parent, Model Model, int InstanceIndex, string InstanceId)
			: base(Parent, Model, InstanceIndex, InstanceId)
		{
		}

		/// <summarmy>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(ModBusClient);

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.host = XML.Attribute(Definition, "host", "localhost");
			this.port = XML.Attribute(Definition, "port", ModbusTcpClient.DefaultPort);

			return base.FromXml(Definition);
		}

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new ModBusClient(Parent, Model);
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
			return Task.FromResult<Actor>(new ModBusClient(this, this.Model, InstanceIndex, InstanceId)
			{
				host = this.host,
				port = this.port
			});
		}

		/// <summary>
		/// Initializes an instance of an actor.
		/// </summary>
		public override Task InitializeInstance()
		{
			this.client = null;
			return Task.CompletedTask;
		}

		/// <summary>
		/// Starts an instance of an actor.
		/// </summary>
		public override async Task StartInstance()
		{
			this.sniffer = this.Model.GetSniffer(this.InstanceId, BinaryPresentationMethod.Hexadecimal);
			this.client = await ModbusTcpClient.Connect(this.host, this.port, this.sniffer);
		}

		/// <summary>
		/// Finalizes an instance of an actor.
		/// </summary>
		public override async Task FinalizeInstance()
		{
			await this.Lock();
			try
			{
				this.client?.Dispose();
				this.client = null;

				if (!(this.sniffer is null))
				{
					if (this.sniffer is IDisposable Disposable)
						Disposable.Dispose();

					this.sniffer = null;
				}
			}
			finally
			{
				this.Unlock();
			}
		}

		/// <summary>
		/// ModBus TCP client reference.
		/// </summary>
		public ModbusTcpClient Client => this.client;

		/// <summary>
		/// Locks the client for use by one caller.
		/// </summary>
		public async Task Lock()
		{
			await this.synchObj.WaitAsync();
		}

		/// <summary>
		/// Unlocks the client.
		/// </summary>
		public void Unlock()
		{
			this.synchObj.Release();
		}
	}
}