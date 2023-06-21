using System.Collections.Generic;
using System.Threading.Tasks;
using TAG.Simulator.ModBus.Actors;
using Waher.Networking.Modbus;

namespace TAG.Simulator.ModBus.Registers.Registers
{
	/// <summary>
	/// A holding register.
	/// </summary>
	public class ModBusHoldingRegister : ModBusRegister
	{
		/// <summary>
		/// A holding register.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public ModBusHoldingRegister(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(ModBusHoldingRegister);

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new ModBusHoldingRegister(Parent, Model);
		}

		/// <summary>
		/// Registers the node on the ModBus server object instance.
		/// </summary>
		/// <param name="Server">ModBus server object instance.</param>
		public override void RegisterRegister(ModBusServer Server)
		{
			Server.Server.OnReadMultipleRegisters += this.Server_OnReadMultipleRegisters;
			Server.Server.OnWriteRegister += this.Server_OnWriteRegister;
		}

		/// <summary>
		/// Unregisters the node from the ModBus server object instance.
		/// </summary>
		/// <param name="Server">ModBus server object instance.</param>
		public override void UnregisterRegister(ModBusServer Server)
		{
			Server.Server.OnReadMultipleRegisters -= this.Server_OnReadMultipleRegisters;
			Server.Server.OnWriteRegister -= this.Server_OnWriteRegister;
		}

		private Task Server_OnReadMultipleRegisters(object Sender, ReadWordsEventArgs e)
		{
			ModBusDevice Device = this.FindInstance(e.UnitAddress);

			this.Model.ExternalEvent(this, "OnExecuteReadoutRequest",
				new KeyValuePair<string, object>("e", e),
				new KeyValuePair<string, object>("Device", Device),
				new KeyValuePair<string, object>("Register", this),
				new KeyValuePair<string, object>("RegisterNr", this.RegisterNr));

			return Task.CompletedTask;
		}

		private Task Server_OnWriteRegister(object Sender, WriteWordEventArgs e)
		{
			ModBusDevice Device = this.FindInstance(e.UnitAddress);

			this.Model.ExternalEvent(this, "OnExecuteSetRequest",
				new KeyValuePair<string, object>("e", e),
				new KeyValuePair<string, object>("Device", Device),
				new KeyValuePair<string, object>("Register", this),
				new KeyValuePair<string, object>("RegisterNr", this.RegisterNr),
				new KeyValuePair<string, object>("Value", e.Value));

			return Task.CompletedTask;
		}
	}
}
