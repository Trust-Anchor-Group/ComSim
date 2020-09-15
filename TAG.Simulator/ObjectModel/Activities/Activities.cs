using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.Statistics;

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
			CountTable Table = new CountTable();

			Output.WriteLine("Activities");
			Output.WriteLine("=============");
			Output.WriteLine();

			foreach (ISimulationNode Node in this.Children)
			{
				if (Node is IActivity Activity)
					Table.Add(Activity.Id, Activity.ExecutionCount);
			}

			Table.ExportTableMarkdown(Output, "Activity", "Total activity counts", "TotalActivityCounts");
			Table.ExportTableGraph(Output, "Total activity counts");

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
