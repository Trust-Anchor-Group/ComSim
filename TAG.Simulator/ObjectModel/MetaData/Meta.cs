using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel.Values;
using Waher.Script.Functions.Vectors;

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
		/// <param name="Output">Output node</param>
		public override async Task ExportMarkdown(StreamWriter Output)
		{
			await base.ExportMarkdown(Output);

			Output.WriteLine("| Time units ||");
			Output.WriteLine("|:-----|:-----|");
			Output.Write("| Duration: | ");
			Duration.ExportText(this.Model.Duration, Output);
			Output.WriteLine(" |");
			Output.Write("| Base: | ");
			Output.Write(this.Model.TimeBase.ToString());
			Output.WriteLine(" |");
			Output.Write("| Unit: | ");
			Duration.ExportText(this.Model.TimeUnit, Output);
			Output.WriteLine(" |");
			Output.Write("| Cycle: | ");
			Duration.ExportText(this.Model.TimeCycle, Output);
			Output.WriteLine(" |");
			Output.Write("| Bucket: | ");
			Duration.ExportText(this.Model.BucketTime, Output);
			Output.WriteLine(" |");
			Output.WriteLine();
		}

		/// <summary>
		/// Exports XML
		/// </summary>
		/// <param name="Output">Output node</param>
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
