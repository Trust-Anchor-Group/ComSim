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
	}
}
