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
		/// Optional header
		/// </summary>
		string Header
		{
			get;
		}

		/// <summary>
		/// Exports the graph to a markdown output.
		/// </summary>
		/// <param name="Output">Markdown output</param>
		void ExportGraph(StreamWriter Output);

		/// <summary>
		/// Exports the graph to a markdown output.
		/// </summary>
		/// <param name="Output">Markdown output</param>
		/// <param name="CustomColor">Optional custom color</param>
		/// <param name="Span">If the entire span can be included.</param>
		/// <returns>If script was exported.</returns>
		bool ExportGraphScript(StreamWriter Output, string CustomColor, bool Span);
	}
}
