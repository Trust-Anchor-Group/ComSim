namespace TAG.Simulator.ModBus.Activities
{
	/// <summary>
	/// Abstract base class for ModBus read operations.
	/// </summary>
	public abstract class ModBusReadOperation : ModBusOperation 
	{
		/// <summary>
		/// Abstract base class for ModBus read operations.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public ModBusReadOperation(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}
	}
}
