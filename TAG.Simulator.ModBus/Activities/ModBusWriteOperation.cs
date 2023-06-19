using System;
using TAG.Simulator.ObjectModel.Values;

namespace TAG.Simulator.ModBus.Activities
{
	/// <summary>
	/// Abstract base class for ModBus write operations.
	/// </summary>
	public abstract class ModBusWriteOperation : ModBusOperation, IValueRecipient
	{
		private IValue value = null;

		/// <summary>
		/// Abstract base class for ModBus write operations.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public ModBusWriteOperation(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Registers a value for the argument.
		/// </summary>
		/// <param name="Value">Value node</param>
		public void Register(IValue Value)
		{
			if (this.value is null)
				this.value = Value;
			else
				throw new Exception("Write node already has a value defined.");
		}
	}
}
