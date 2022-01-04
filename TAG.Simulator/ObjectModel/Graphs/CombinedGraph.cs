using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using SkiaSharp;
using Waher.Content.Xml;
using Waher.Events;

namespace TAG.Simulator.ObjectModel.Graphs
{
	/// <summary>
	/// Abstract base class for combined graphs
	/// </summary>
	public abstract class CombinedGraph : Graph, ISourceRecipient
	{
		private readonly LinkedList<ISource> sources = new LinkedList<ISource>();
		private int count = 0;
		private string title;
		private bool legend;
		private bool span;

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
			this.legend = XML.Attribute(Definition, "legend", true);
			this.span = XML.Attribute(Definition, "span", true);

			return base.FromXml(Definition);
		}

		/// <summary>
		/// Registers a source.
		/// </summary>
		/// <param name="Source">Source node</param>
		public virtual void Register(ISource Source)
		{
			this.sources.AddLast(Source);
			this.count++;
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
			this.ExportGraphScript(Output, null, true);
			Output.WriteLine("}");
			Output.WriteLine();

			if (this.legend)
			{
				List<string> Labels = new List<string>();

				foreach (Source Source in this.sources)
					Labels.Add(Source.Reference);

				ExportLegend(Output, Labels.ToArray());
			}
		}

		/// <summary>
		/// Exports a legend.
		/// </summary>
		/// <param name="Output">Markdown output</param>
		/// <param name="Labels">Labels to add to legend.</param>
		/// <param name="Palette">Palette to use</param>
		public static void ExportLegend(StreamWriter Output, string[] Labels, params SKColor[] Palette)
		{
			int i = 0;

			if (Palette is null || Palette.Length < Labels.Length)
				Palette = Model.CreatePalette(Labels.Length);

			Output.WriteLine("```layout: Legend");
			Output.WriteLine("<Layout2D xmlns=\"http://waher.se/Layout/Layout2D.xsd\"");
			Output.WriteLine("          background=\"WhiteBackground\" pen=\"BlackPen\"");
			Output.WriteLine("          font=\"Text\" textColor=\"Black\">");
			Output.WriteLine("  <SolidPen id=\"BlackPen\" color=\"Black\" width=\"1px\"/>");
			Output.WriteLine("  <SolidBackground id=\"WhiteBackground\" color=\"WhiteSmoke\"/>");

			foreach (string Label in Labels)
			{
				SKColor Color = Palette[i++];

				Output.Write("  <SolidBackground id=\"");
				Output.Write(Label);
				Output.Write("Bg\" color=\"");
				Output.Write(Model.ToString(Color));
				Output.WriteLine("\"/>");
			}

			Output.WriteLine("  <Font id=\"Text\" name=\"Arial\" size=\"8pt\" color=\"Black\"/>");
			Output.WriteLine("  <Grid columns=\"2\">");

			foreach (string Label in Labels)
			{
				Output.WriteLine("    <Cell>");
				Output.WriteLine("      <Margins left=\"1mm\" top=\"1mm\" bottom=\"1mm\" right=\"1mm\">");
				Output.Write("        <RoundedRectangle radiusX=\"1mm\" radiusY=\"1mm\" width=\"5mm\" height=\"5mm\" fill=\"");
				Output.Write(Label);
				Output.Write("Bg");
				Output.WriteLine("\"/>");
				Output.WriteLine("      </Margins>");
				Output.WriteLine("    </Cell>");
				Output.WriteLine("    <Cell>");
				Output.WriteLine("      <Margins left=\"0.5em\" right=\"0.5em\">");
				Output.Write("        <Label text=\"");
				Output.Write(Label);
				Output.WriteLine("\" x=\"0%\" y=\"50%\" halign=\"Left\" valign=\"Center\" font=\"Text\"/>");
				Output.WriteLine("      </Margins>");
				Output.WriteLine("    </Cell>");
			}

			Output.WriteLine("  </Grid>");
			Output.WriteLine("</Layout2D>");
			Output.WriteLine("```");
			Output.WriteLine();
		}

		/// <summary>
		/// Exports the graph to a markdown output.
		/// </summary>
		/// <param name="Output">Markdown output</param>
		/// <param name="CustomColor">Optional custom color</param>
		/// <param name="Span">If the entire span can be included.</param>
		/// <returns>If script was exported.</returns>
		public override bool ExportGraphScript(StreamWriter Output, string CustomColor, bool Span)
		{
			SKColor[] Palette = Model.CreatePalette(this.count);
			IGraph Graph;
			int i = 0;
			bool First = true;

			Output.WriteLine("G:=Sum([(");

			foreach (Source Source in this.sources)
			{
				Graph = this.GetGraph(Source.Reference);
				if (Graph is null)
					Log.Error("Graph for " + Source.Reference + " could not be found.");
				else
				{
					if (!First)
						Output.WriteLine("), (");

					if (Graph.ExportGraphScript(Output, Model.ToString(Palette[i]), Span && this.span))
						First = false;
				}

				i++;
			}

			Output.WriteLine(")]);");
			Output.Write("G.LabelX:=\"Time × ");
			Output.Write(Model.TimeUnitStr);
			Output.WriteLine("\";");
			Output.Write("G.Title:=\"");
			Output.Write(this.title.Replace("\"", "\\\""));
			Output.WriteLine("\";");
			Output.WriteLine("G");

			return true;
		}

		/// <summary>
		/// Gets a graph from its reference.
		/// </summary>
		/// <param name="Reference">Source reference.</param>
		/// <returns>Graph object if found, null otherwise.</returns>
		public abstract IGraph GetGraph(string Reference);
	}
}
