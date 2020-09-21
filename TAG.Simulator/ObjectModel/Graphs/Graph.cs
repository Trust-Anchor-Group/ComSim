using System;
using System.IO;
using System.Threading.Tasks;
using TAG.Simulator.Statistics;

namespace TAG.Simulator.ObjectModel.Graphs
{
	/// <summary>
	/// Abstract base class for graph nodes
	/// </summary>
	public abstract class Graph : SimulationNode, IGraph
	{
		/// <summary>
		/// Abstract base class for graph nodes
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public Graph(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// If the graph represents the visualization of a given entity. (Otherwise, null, or the empty string.)
		/// </summary>
		public abstract string For
		{
			get;
		}

		/// <summary>
		/// Starts the node.
		/// </summary>
		public override Task Start()
		{
			this.Model.Register(this);
			return base.Start();
		}

		/// <summary>
		/// Exports the graph to a markdown output.
		/// </summary>
		/// <param name="Output">Markdown output</param>
		public abstract void ExportSampleHistoryGraph(StreamWriter Output);
	}
}
