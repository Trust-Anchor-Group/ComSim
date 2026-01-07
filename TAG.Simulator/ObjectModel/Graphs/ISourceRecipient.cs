namespace TAG.Simulator.ObjectModel.Graphs
{
	/// <summary>
	/// Interface for nodes holding a source node
	/// </summary>
	public interface ISourceRecipient : ISimulationNodeChildren
	{
		/// <summary>
		/// Registers a source.
		/// </summary>
		/// <param name="Source">Source node</param>
		void Register(ISource Source);
	}
}
