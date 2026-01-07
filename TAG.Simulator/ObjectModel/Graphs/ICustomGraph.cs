namespace TAG.Simulator.ObjectModel.Graphs
{
	/// <summary>
	/// Interface for custom graph nodes
	/// </summary>
	public interface ICustomGraph : IGraph
	{
		/// <summary>
		/// If the graph represents the visualization of a given entity. (Otherwise, null, or the empty string.)
		/// </summary>
		string For
		{
			get;
		}
	}
}
