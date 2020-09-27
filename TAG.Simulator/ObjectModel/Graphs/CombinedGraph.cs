using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Waher.Content.Xml;

namespace TAG.Simulator.ObjectModel.Graphs
{
	/// <summary>
	/// Abstract base class for combined graphs
	/// </summary>
	public abstract class CombinedGraph : Graph, ISourceRecipient
	{
		private readonly LinkedList<ISource> sources = new LinkedList<ISource>();
		private string title;

		/// <summary>
		/// Abstract base class for combined graphs
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public CombinedGraph(ISimulationNode Parent, Model Model) 
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Title of graph.
		/// </summary>
		public string Title => this.title;

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.title = XML.Attribute(Definition, "title");

			return base.FromXml(Definition);
		}

		/// <summary>
		/// Registers a source.
		/// </summary>
		/// <param name="Source">Source node</param>
		public virtual void Register(ISource Source)
		{
			this.sources.AddLast(Source);
		}

		/// <summary>
		/// Exports the graph to a markdown output.
		/// </summary>
		/// <param name="Output">Markdown output</param>
		public override void ExportGraph(StreamWriter Output)
		{
			Output.WriteLine("{");
			Output.WriteLine("GraphWidth:=1000;");
			Output.WriteLine("GraphHeight:=400;");
			Output.Write("G:=");
			this.ExportGraphScript(Output);
			Output.WriteLine(";");
			Output.Write("G.Title:=\"");
			Output.Write(this.title.Replace("\"", "\\\""));
			Output.WriteLine("\";");
			Output.WriteLine("G");
			Output.WriteLine("}");
			Output.WriteLine();
		}

		/// <summary>
		/// Exports the graph to a markdown output.
		/// </summary>
		/// <param name="Output">Markdown output</param>
		public override void ExportGraphScript(StreamWriter Output)
		{
			IGraph Graph;
			bool First = true;

			Output.WriteLine("Sum([(");

			foreach (Source Source in this.sources)
			{
				if (First)
					First = false;
				else
					Output.WriteLine("), (");

				Graph = this.GetGraph(Source.Reference);
				if (Graph is null)
					throw new Exception("Graph for " + Source.Reference + " could not be found.");
				
				Graph.ExportGraphScript(Output);
			}

			Output.Write(")])");
		}

		/// <summary>
		/// Gets a graph from its reference.
		/// </summary>
		/// <param name="Reference">Source reference.</param>
		/// <returns>Graph object if found, null otherwise.</returns>
		public abstract IGraph GetGraph(string Reference);
	}
}
