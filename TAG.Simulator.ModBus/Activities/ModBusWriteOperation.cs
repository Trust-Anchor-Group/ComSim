namespace TAG.Simulator.ModBus.Activities
{
	/// <summary>
	/// Abstract base class for ModBus write operations.
	/// </summary>
	public abstract class ModBusWriteOperation : ModBusOperation 
	{
		/// <summary>
		/// Abstract base class for ModBus write operations.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public ModBusWriteOperation(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}
	}
}
