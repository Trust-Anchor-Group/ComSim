using System.Collections.Generic;
using System.Threading.Tasks;
using TAG.Simulator.ModBus.Actors;
using Waher.Networking.Modbus;

namespace TAG.Simulator.ModBus.Registers.Registers
{
	/// <summary>
	/// An input floating-point register.
	/// </summary>
	public class ModBusInputFloatingPointRegister : ModBusRegister
	{
		/// <summary>
		/// An input floating-point register.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public ModBusInputFloatingPointRegister(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(ModBusInputFloatingPointRegister);

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new ModBusInputFloatingPointRegister(Parent, Model);
		}

		/// <summary>
		/// Registers the node on the ModBus server object instance.
		/// </summary>
		/// <param name="Server">ModBus server object instance.</param>
		public override void RegisterRegister(ModBusServer Server)
		{
			Server.Server.OnReadInputRegisters += this.Server_OnReadInputRegisters;
		}

		/// <summary>
		/// Unregisters the node from the ModBus server object instance.
		/// </summary>
		/// <param name="Server">ModBus server object instance.</param>
		public override void UnregisterRegister(ModBusServer Server)
		{
			Server.Server.OnReadInputRegisters -= this.Server_OnReadInputRegisters;
		}

		private Task Server_OnReadInputRegisters(object Sender, ReadWordsEventArgs e)
		{
			ModBusDevice Device = this.FindInstance(e.UnitAddress);

			if (!(Device is null))
			{
				this.Model.ExternalEvent(Device, "OnExecuteReadoutRequest",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Register", this),
					new KeyValuePair<string, object>("RegisterNr", this.Register));
			}

			return Task.CompletedTask;
		}
	}
}
