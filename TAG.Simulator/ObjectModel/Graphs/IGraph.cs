using System;
using System.IO;
using TAG.Simulator.Statistics;

namespace TAG.Simulator.ObjectModel.Graphs
{
	/// <summary>
	/// Interface for graph nodes
	/// </summary>
	public interface IGraph
	{
		/// <summary>
		/// Exports the graph to a markdown output.
		/// </summary>
		/// <param name="Output">Markdown output</param>
		void ExportGraph(StreamWriter Output);

		/// <summary>
		/// Exports the graph to a markdown output.
		/// </summary>
		/// <param name="Output">Markdown output</param>
		void ExportGraphScript(StreamWriter Output);
	}
}
