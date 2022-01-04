using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel.Values;

namespace TAG.Simulator.ObjectModel.MetaData
{
	/// <summary>
	/// Node that contains meta-data about the model.
	/// </summary>
	public class Meta : SimulationNodeChildren
	{
		/// <summary>
		/// Node that contains meta-data about the model.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public Meta(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "Meta";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new Meta(Parent, Model);
		}

		/// <summary>
		/// Exports Markdown
		/// </summary>
		/// <param name="Output">Output</param>
		public override async Task ExportMarkdown(StreamWriter Output)
		{
			await base.ExportMarkdown(Output);

			Output.WriteLine("General");
			Output.WriteLine("==========");
			Output.WriteLine();

			Output.WriteLine("| Time units ||");
			Output.WriteLine("|:-----|:-----|");
			Output.Write("| Simulation Duration: | ");
			Duration.ExportText(this.Model.Duration, Output);
			Output.WriteLine(" |");
			Output.Write("| Time Base: | ");
			
			switch (this.Model.TimeBase)
			{
				case TimeBase.ComputerClock:
					Output.Write("Computer Clock");
					break;

				case TimeBase.StartOfSimulation:
					Output.Write("Start of Simulation");
					break;

				default:
					Output.Write(this.Model.TimeBase.ToString());
					break;
			}

			Output.WriteLine(" |");
			Output.Write("| Time Unit: | ");
			Duration.ExportText(this.Model.TimeUnit, Output);
			Output.WriteLine(" |");
			Output.Write("| Time Cycle: | ");
			Duration.ExportText(this.Model.TimeCycle, Output);
			Output.WriteLine(" |");
			Output.Write("| Bucket Time: | ");
			Duration.ExportText(this.Model.BucketTime, Output);
			Output.WriteLine(" |");
			Output.Write("| Start Date: | ");
			Output.Write(this.Model.StartTime.ToString("d"));
			Output.WriteLine(" |");
			Output.Write("| Start Time: | ");
			Output.Write(this.Model.StartTime.ToString("T"));
			Output.WriteLine(" |");
			Output.Write("| End Date: | ");
			Output.Write(this.Model.EndTime.ToString("d"));
			Output.WriteLine(" |");
			Output.Write("| End Time: | ");
			Output.Write(this.Model.EndTime.ToString("T"));
			Output.WriteLine(" |");
			Output.WriteLine();
		}

		/// <summary>
		/// Exports XML
		/// </summary>
		/// <param name="Output">Output</param>
		public override async Task ExportXml(XmlWriter Output)
		{
			await base.ExportXml(Output);

			Output.WriteStartElement("TimeUnits");
			Output.WriteAttributeString("duration", this.Model.Duration.ToString());
			Output.WriteAttributeString("timeBase", this.Model.TimeBase.ToString());
			Output.WriteAttributeString("timeUnit", this.Model.TimeUnit.ToString());
			Output.WriteAttributeString("timeCycle", this.Model.TimeCycle.ToString());
			Output.WriteAttributeString("bucketTime", this.Model.BucketTime.ToString());
			Output.WriteEndElement();
		}
	}
}
