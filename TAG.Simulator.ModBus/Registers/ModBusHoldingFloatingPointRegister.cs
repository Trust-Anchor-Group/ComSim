using System.Collections.Generic;
using System.Threading.Tasks;
using TAG.Simulator.ModBus.Actors;
using TAG.Simulator.ModBus.Registers.Activities;
using Waher.Networking.Modbus;

namespace TAG.Simulator.ModBus.Registers.Registers
{
	/// <summary>
	/// A holding floating-point register.
	/// </summary>
	public class ModBusHoldingFloatingPointRegister : ModBusRegister
	{
		private readonly Dictionary<uint, ushort> halfCookedValues = new Dictionary<uint, ushort>();

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
		/// Starts the node.
		/// </summary>
		public override Task Start()
		{
			lock (this.halfCookedValues)
			{
				this.halfCookedValues.Clear();
			}

			return base.Start();
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

			if (!(Device is null))
			{
				this.Model.ExternalEvent(this, Device, "OnExecuteReadoutRequest",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Register", this),
					new KeyValuePair<string, object>("RegisterNr", this.RegisterNr));
			}

			return Task.CompletedTask;
		}

		private Task Server_OnWriteRegister(object Sender, WriteWordEventArgs e)
		{
			ModBusDevice Device = this.FindInstance(e.UnitAddress);

			if (!(Device is null))
			{
				uint Key = e.UnitAddress;
				Key <<= 16;
				Key |= e.ReferenceNr;

				ushort Value1, Value2;

				lock (this.halfCookedValues)
				{
					this.halfCookedValues[Key] = e.Value;

					Key &= ~1U;

					if (!this.halfCookedValues.TryGetValue(Key, out Value1) ||
						!this.halfCookedValues.TryGetValue(Key + 1, out Value2))
					{
						return Task.CompletedTask;
					}

					this.halfCookedValues.Remove(Key);
					this.halfCookedValues.Remove(Key + 1);
				}

				double Value = ReadModBusHoldingFloatingPointRegister.ToFloat(FloatByteOrder.NetworkOrder, Value1, Value2);

				this.Model.ExternalEvent(this, Device, "OnExecuteSetRequest",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Register", this),
					new KeyValuePair<string, object>("RegisterNr", this.RegisterNr),
					new KeyValuePair<string, object>("Value", Value));
			}

			return Task.CompletedTask;
		}
	}
}
