using System.Collections.Generic;
using System.Threading.Tasks;
using TAG.Simulator.ModBus.Actors;
using Waher.Networking.Modbus;

namespace TAG.Simulator.ModBus.Registers.Registers
{
	/// <summary>
	/// A discrete input.
	/// </summary>
	public class ModBusDiscreteInput : ModBusRegister
	{
		/// <summary>
		/// A discrete input.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public ModBusDiscreteInput(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(ModBusDiscreteInput);

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new ModBusDiscreteInput(Parent, Model);
		}

		/// <summary>
		/// Registers the node on the ModBus server object instance.
		/// </summary>
		/// <param name="Server">ModBus server object instance.</param>
		public override void RegisterRegister(ModBusServer Server)
		{
			Server.Server.OnReadInputDiscretes += this.Server_OnReadInputDiscretes;
		}

		/// <summary>
		/// Unregisters the node from the ModBus server object instance.
		/// </summary>
		/// <param name="Server">ModBus server object instance.</param>
		public override void UnregisterRegister(ModBusServer Server)
		{
			Server.Server.OnReadInputDiscretes -= this.Server_OnReadInputDiscretes;
		}

		private Task Server_OnReadInputDiscretes(object Sender, ReadBitsEventArgs e)
		{
			ModBusDevice Device = this.FindInstance(e.UnitAddress);

			if (!(Device is null))
			{
				this.Model.ExternalEvent(this, Device, "OnExecuteReadoutRequest",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Register", this),
					new KeyValuePair<string, object>("RegisterNr", this.RegisterNr));
			}

			return Task.CompletedTask;
		}
	}
}
