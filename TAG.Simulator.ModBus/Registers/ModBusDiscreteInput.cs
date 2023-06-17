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
	}
}
