﻿using System.Threading.Tasks;
using TAG.Simulator.ModBus.Actors;
using Waher.Networking.Modbus;

namespace TAG.Simulator.ModBus.Registers.Registers
{
	/// <summary>
	/// A coil.
	/// </summary>
	public class ModBusCoil : ModBusRegister
	{
		/// <summary>
		/// A coil.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public ModBusCoil(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(ModBusCoil);

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new ModBusCoil(Parent, Model);
		}

		/// <summary>
		/// Registers the node on the ModBus server object instance.
		/// </summary>
		/// <param name="InstanceAddress">Instance address.</param>
		/// <param name="Server">ModBus server object instance.</param>
		public override void RegisterRegister(byte InstanceAddress, ModBusServer Server)
		{
			base.RegisterRegister(InstanceAddress, Server);

			Server.Server.OnReadCoils += this.Server_OnReadCoils;
		}

		/// <summary>
		/// Unregisters the node from the ModBus server object instance.
		/// </summary>
		/// <param name="Server">ModBus server object instance.</param>
		public override void UnregisterRegister(ModBusServer Server)
		{
			Server.Server.OnReadCoils -= this.Server_OnReadCoils;
		}

		private async Task Server_OnReadCoils(object Sender, ReadBitsEventArgs e)
		{
			if (e.UnitAddress != this.instanceAddress)
				return;

		}
	}
}
