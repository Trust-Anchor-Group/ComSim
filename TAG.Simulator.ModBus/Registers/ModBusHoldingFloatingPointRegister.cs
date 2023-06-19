using System.Collections.Generic;
using System.Threading.Tasks;
using TAG.Simulator.ModBus.Actors;
using Waher.Networking.Modbus;

namespace TAG.Simulator.ModBus.Registers.Registers
{
	/// <summary>
	/// A holding floating-point register.
	/// </summary>
	public class ModBusHoldingFloatingPointRegister : ModBusRegister
	{
		/// <summary>
		/// A holding floating-point register.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public ModBusHoldingFloatingPointRegister(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(ModBusHoldingFloatingPointRegister);

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new ModBusHoldingFloatingPointRegister(Parent, Model);
		}

		/// <summary>
		/// Registers the node on the ModBus server object instance.
		/// </summary>
		/// <param name="InstanceAddress">Instance address.</param>
		/// <param name="Server">ModBus server object instance.</param>
		public override void RegisterRegister(byte InstanceAddress, ModBusServer Server)
		{
			base.RegisterRegister(InstanceAddress, Server);

			Server.Server.OnReadMultipleRegisters += this.Server_OnReadMultipleRegisters;
		}

		/// <summary>
		/// Unregisters the node from the ModBus server object instance.
		/// </summary>
		/// <param name="Server">ModBus server object instance.</param>
		public override void UnregisterRegister(ModBusServer Server)
		{
			Server.Server.OnReadMultipleRegisters -= this.Server_OnReadMultipleRegisters;
		}

		private Task Server_OnReadMultipleRegisters(object Sender, ReadWordsEventArgs e)
		{
			if (e.UnitAddress == this.instanceAddress)
			{
				this.Model.ExternalEvent((ModBusDevice)this.Parent, "OnExecuteReadoutRequest",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Register", this),
					new KeyValuePair<string, object>("RegisterNr", this.Register));
			}

			return Task.CompletedTask;
		}
	}
}
