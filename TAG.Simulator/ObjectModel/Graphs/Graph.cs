using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Waher.Content.Xml;

namespace TAG.Simulator.ObjectModel.Graphs
{
	/// <summary>
	/// Abstract base class for graph nodes
	/// </summary>
	public abstract class Graph : SimulationNodeChildren, IGraph
	{
		private string header;

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
		/// Optional header
		/// </summary>
		public string Header => this.header;

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.header = XML.Attribute(Definition, "header");

			return base.FromXml(Definition);
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
		public abstract void ExportGraph(StreamWriter Output);

		/// <summary>
		/// Exports the graph to a markdown output.
		/// </summary>
		/// <param name="Output">Markdown output</param>
		/// <param name="CustomColor">Optional custom color</param>
		/// <returns>If script was exported.</returns>
		public abstract bool ExportGraphScript(StreamWriter Output, string CustomColor);
	}
}
