using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TAG.Simulator.ObjectModel.Activities
{
	/// <summary>
	/// Container for activities.
	/// </summary>
	public class Activities : SimulationNodeChildren
	{
		/// <summary>
		/// Container for activities.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		public Activities(ISimulationNode Parent)
			: base(Parent)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "Activities";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent)
		{
			return new Activities(Parent);
		}

		/// <summary>
		/// Exports Markdown
		/// </summary>
		/// <param name="Output">Output node</param>
		public override Task ExportMarkdown(StreamWriter Output)
		{
			int MaxCount = 1;   // To avoid division by zero

			Output.WriteLine("Activities");
			Output.WriteLine("=============");
			Output.WriteLine();

			Output.WriteLine("| Activity | Count |");
			Output.WriteLine("|:---------|------:|");

			foreach (ISimulationNode Node in this.Children)
			{
				if (Node is IActivity Activity)
				{
					if (Activity.ExecutionCount > MaxCount)
						MaxCount = Activity.ExecutionCount;

					Output.Write("| `");
					Output.Write(Activity.Id);
					Output.Write("` | ");
					Output.Write(Activity.ExecutionCount.ToString());
					Output.WriteLine(" |");
				}
			}

			Output.WriteLine("[Total activity counts][TotalActivityCounts]");

			Output.WriteLine();
			Output.WriteLine("```layout: Total activity counts");
			Output.WriteLine("<Layout2D xmlns=\"http://waher.se/Layout/Layout2D.xsd\"");
			Output.WriteLine("          background=\"WhiteBackground\" pen=\"BlackPen\"");
			Output.WriteLine("          font=\"WhiteText\" textColor=\"Black\">");
			Output.WriteLine("  <SolidPen id=\"BlackPen\" color=\"Black\" width=\"1px\"/>");
			Output.WriteLine("  <SolidBackground id=\"WhiteBackground\" color=\"WhiteSmoke\"/>");
			Output.WriteLine("  <SolidBackground id=\"Bar\" color=\"{Alpha('Green',128)}\"/>");
			Output.WriteLine("  <Font id=\"WhiteText\" name=\"Arial\" size=\"12pt\" color=\"White\"/>");
			Output.WriteLine("  <Font id=\"BlackText\" name=\"Arial\" size=\"12pt\" color=\"Black\"/>");
			Output.WriteLine("  <Grid columns=\"2\">");

			int HalfMaxCount = MaxCount / 2;

			foreach (ISimulationNode Node in this.Children)
			{
				if (Node is IActivity Activity)
				{
					Output.WriteLine("    <Cell>");
					Output.WriteLine("      <Margins left=\"0.5em\" right=\"0.5em\">");
					Output.Write("        <Label text=\"");
					Output.Write(Activity.Id);
					Output.WriteLine("\" x=\"100%\" y=\"50%\" halign=\"Right\" valign=\"Center\" font=\"BlackText\"/>");
					Output.WriteLine("      </Margins>");
					Output.WriteLine("    </Cell>");
					Output.WriteLine("    <Cell>");
					Output.WriteLine("      <Margins left=\"1mm\" top=\"1mm\" bottom=\"1mm\" right=\"1mm\">");
					Output.Write("        <RoundedRectangle radiusX=\"3mm\" radiusY=\"3mm\" width=\"");
					Output.Write((Activity.ExecutionCount * 100 + HalfMaxCount) / MaxCount);
					Output.WriteLine("%\" height=\"1cm\" fill=\"Bar\">");
					Output.WriteLine("          <Margins left=\"0.5em\" right=\"0.5em\">");
					Output.Write("            <Label text=\"");
					Output.Write(Activity.ExecutionCount.ToString());
					Output.WriteLine("\" x=\"50%\" y=\"50%\" halign=\"Center\" valign=\"Center\"/>");
					Output.WriteLine("          </Margins>");
					Output.WriteLine("        </RoundedRectangle>");
					Output.WriteLine("      </Margins>");
					Output.WriteLine("    </Cell>");
				}
			}

			Output.WriteLine("  </Grid>");
			Output.WriteLine("</Layout2D>");
			Output.WriteLine("```");
			Output.WriteLine();

			return base.ExportMarkdown(Output);
		}

		/// <summary>
		/// Exports XML
		/// </summary>
		/// <param name="Output">Output node</param>
		public override async Task ExportXml(XmlWriter Output)
		{
			Output.WriteStartElement("Activities");
			await base.ExportXml(Output);
			Output.WriteEndElement();
		}

	}
}
