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
			StringBuilder IDs = new StringBuilder();
			StringBuilder Counts = new StringBuilder();
			bool First = true;

			Output.WriteLine("Activities");
			Output.WriteLine("=============");
			Output.WriteLine();

			Output.WriteLine("| Activity | Count |");
			Output.WriteLine("|:---------|------:|");

			foreach (ISimulationNode Node in this.Children)
			{
				if (Node is IActivity Activity)
				{
					Output.Write("| `");
					Output.Write(Activity.Id);
					Output.Write("` | ");
					Output.Write(Activity.ExecutionCount.ToString());
					Output.WriteLine(" |");

					if (First)
					{
						IDs.Append("[");
						Counts.Append("[");
						First = false;
					}
					else
					{
						IDs.Append(",");
						Counts.Append(",");
					}

					IDs.Append('"');
					IDs.Append(Activity.Id);
					IDs.Append('"');

					Counts.Append(Activity.ExecutionCount.ToString());
				}
			}

			Output.WriteLine("[Total activity counts][TotalActivityCounts]");

			if (!(First))
			{
				IDs.Append(']');
				Counts.Append(']');

				Output.WriteLine();
				Output.Write("{HorizontalBars(");
				Output.Write(IDs.ToString());
				Output.Write(',');
				Output.Write(Counts.ToString());
				Output.WriteLine(")}");
				Output.WriteLine();
			}

			return Task.CompletedTask;
		}

		/// <summary>
		/// Exports XML
		/// </summary>
		/// <param name="Output">Output node</param>
		public override Task ExportXml(XmlWriter Output)
		{
			Output.WriteStartElement("Activities");
			Output.WriteStartElement("TotalCounts");

			foreach (ISimulationNode Node in this.Children)
			{
				if (Node is IActivity Activity)
				{
					Output.WriteStartElement("TotalCount");
					Output.WriteAttributeString("activity", Activity.Id);
					Output.WriteAttributeString("count", Activity.ExecutionCount.ToString());
					Output.WriteEndElement();
				}
			}

			Output.WriteEndElement();
			Output.WriteEndElement();

			return base.ExportXml(Output);
		}

	}
}
