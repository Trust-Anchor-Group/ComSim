using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Xml;

namespace TAG.Simulator.Statistics
{
	/// <summary>
	/// Represents a simple count table
	/// </summary>
	public class CountTable
	{
		private readonly LinkedList<KeyValuePair<string, long>> counts = new LinkedList<KeyValuePair<string, long>>();
		private readonly Dictionary<string, string> bgColors = new Dictionary<string, string>();
		private readonly Dictionary<string, string> fgColors = new Dictionary<string, string>();
		private long maxCount = 1;   // To avoid division by zero

		/// <summary>
		/// Represents a simple count table
		/// </summary>
		public CountTable()
		{
		}

		/// <summary>
		/// Adds a record to the table.
		/// </summary>
		/// <param name="Id">Count ID</param>
		/// <param name="Count">Count</param>
		public void Add(string Id, long Count)
		{
			this.counts.AddLast(new KeyValuePair<string, long>(Id, Count));

			if (Count > this.maxCount)
				this.maxCount = Count;
		}

		/// <summary>
		/// Sets the foreground color for a record.
		/// </summary>
		/// <param name="Id">Record ID</param>
		/// <param name="Color">String representation of color</param>
		public void SetFgColor(string Id, string Color)
		{
			this.fgColors[Id] = Color;
		}

		/// <summary>
		/// Sets the background color for a record.
		/// </summary>
		/// <param name="Id">Record ID</param>
		/// <param name="Color">String representation of color</param>
		public void SetBgColor(string Id, string Color)
		{
			this.bgColors[Id] = Color;
		}

		/// <summary>
		/// Exports data as a Markdown Table
		/// </summary>
		/// <param name="Output">Output node</param>
		/// <param name="Header">Header string</param>
		/// <param name="Title">Title string</param>
		/// <param name="Id">Table ID</param>
		public void ExportTableMarkdown(StreamWriter Output, string Header, string Title, string Id)
		{
			Output.Write("| ");
			Output.Write(Header);
			Output.WriteLine(" | Count |");
			Output.WriteLine("|:---------|------:|");

			foreach (KeyValuePair<string, long> P in this.counts)
			{
				Output.Write("| `");
				Output.Write(P.Key);
				Output.Write("` | ");
				Output.Write(P.Value.ToString());
				Output.WriteLine(" |");
			}

			Output.Write("[");
			Output.Write(Title);
			Output.Write("][");
			Output.Write(Id);
			Output.WriteLine("]");
			Output.WriteLine();
		}

		/// <summary>
		/// Exports data as a graph embedded in markdown
		/// </summary>
		/// <param name="Output">Markdown output</param>
		/// <param name="Title">Title string</param>
		public void ExportTableGraph(StreamWriter Output, string Title)
		{
			Output.Write("```layout: ");
			Output.WriteLine(Title);
			Output.WriteLine("<Layout2D xmlns=\"http://waher.se/Layout/Layout2D.xsd\"");
			Output.WriteLine("          background=\"WhiteBackground\" pen=\"BlackPen\"");
			Output.WriteLine("          font=\"WhiteText\" textColor=\"Black\">");
			Output.WriteLine("  <SolidPen id=\"BlackPen\" color=\"Black\" width=\"1px\"/>");
			Output.WriteLine("  <SolidBackground id=\"WhiteBackground\" color=\"WhiteSmoke\"/>");
			Output.WriteLine("  <SolidBackground id=\"Bar\" color=\"{Alpha('Green',128)}\"/>");

			foreach (KeyValuePair<string, string> P in this.bgColors)
			{
				Output.Write("  <SolidBackground id=\"");
				Output.Write(P.Key);
				Output.Write("Bg\" color=\"");
				Output.Write(P.Value);
				Output.WriteLine("\"/>");
			}

			Output.WriteLine("  <Font id=\"WhiteText\" name=\"Arial\" size=\"12pt\" color=\"White\"/>");
			Output.WriteLine("  <Font id=\"BlackText\" name=\"Arial\" size=\"12pt\" color=\"Black\"/>");

			foreach (KeyValuePair<string, string> P in this.fgColors)
			{
				Output.Write("  <Font id=\"");
				Output.Write(P.Key);
				Output.Write("Fg\" name=\"Arial\" size=\"12pt\" color=\"");
				Output.Write(P.Value);
				Output.WriteLine("\"/>");
			}

			Output.WriteLine("  <Grid columns=\"2\">");

			long HalfMaxCount = this.maxCount / 2;

			foreach (KeyValuePair<string, long> P in this.counts)
			{
				Output.WriteLine("    <Cell>");
				Output.WriteLine("      <Margins left=\"0.5em\" right=\"0.5em\">");
				Output.Write("        <Label text=\"");
				Output.Write(P.Key);
				Output.WriteLine("\" x=\"100%\" y=\"50%\" halign=\"Right\" valign=\"Center\" font=\"BlackText\"/>");
				Output.WriteLine("      </Margins>");
				Output.WriteLine("    </Cell>");
				Output.WriteLine("    <Cell>");
				Output.WriteLine("      <Margins left=\"1mm\" top=\"1mm\" bottom=\"1mm\" right=\"1mm\">");
				Output.Write("        <RoundedRectangle radiusX=\"3mm\" radiusY=\"3mm\" width=\"");
				Output.Write((P.Value * 100 + HalfMaxCount) / this.maxCount);
				Output.Write("%\" height=\"1cm\" fill=\"");

				if (this.bgColors.ContainsKey(P.Key))
				{
					Output.Write(P.Key);
					Output.Write("Bg");
				}
				else
					Output.Write("Bar");

				Output.WriteLine("\">");
				Output.WriteLine("          <Margins left=\"0.5em\" right=\"0.5em\">");
				Output.Write("            <Label text=\"");
				Output.Write(P.Value.ToString());
				Output.Write("\" x=\"50%\" y=\"50%\" halign=\"Center\" valign=\"Center\"");

				if (this.fgColors.ContainsKey(P.Key))
				{
					Output.Write(" font=\"");
					Output.Write(P.Key);
					Output.Write("Fg\"");
				}

				Output.WriteLine("/>");
				Output.WriteLine("          </Margins>");
				Output.WriteLine("        </RoundedRectangle>");
				Output.WriteLine("      </Margins>");
				Output.WriteLine("    </Cell>");
			}

			Output.WriteLine("  </Grid>");
			Output.WriteLine("</Layout2D>");
			Output.WriteLine("```");
			Output.WriteLine();
		}

		/// <summary>
		/// Exports data to XML
		/// </summary>
		/// <param name="Output">XML Output</param>
		/// <param name="TableElement">XML Table element name.</param>
		/// <param name="RowElement">XML Row element name.</param>
		public void ExportXml(XmlWriter Output, string TableElement, string RowElement)
		{
			Output.WriteStartElement(TableElement);

			foreach (KeyValuePair<string, long> P in this.counts)
			{
				Output.WriteStartElement(RowElement);
				Output.WriteAttributeString("type", P.Key);
				Output.WriteAttributeString("count", P.Value.ToString());
				Output.WriteEndElement();
			}

			Output.WriteEndElement();
		}

	}
}
