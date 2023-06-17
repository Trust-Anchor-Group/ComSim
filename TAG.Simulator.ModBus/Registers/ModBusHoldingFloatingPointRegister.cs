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
	}
}
