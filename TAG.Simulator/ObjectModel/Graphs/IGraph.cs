using System;
using System.IO;
using TAG.Simulator.Statistics;

namespace TAG.Simulator.ObjectModel.Graphs
{
	/// <summary>
	/// Interface for graph nodes
	/// </summary>
	public interface IGraph : ISimulationNode
	{
		/// <summary>
		/// If the graph represents the visualization of a given entity. (Otherwise, null, or the empty string.)
		/// </summary>
		string For
		{
			get;
		}

		/// <summary>
		/// Exports the graph to a markdown output.
		/// </summary>
		/// <param name="Output">Markdown output</param>
		void ExportSampleHistoryGraph(StreamWriter Output);
	}
}
