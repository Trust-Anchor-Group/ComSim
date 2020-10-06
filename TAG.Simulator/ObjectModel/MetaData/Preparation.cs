using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;

namespace TAG.Simulator.ObjectModel.MetaData
{
	/// <summary>
	/// Preparation of model
	/// </summary>
	public class Preparation : SimulationNode
	{
		private string preparation;

		/// <summary>
		/// Description of model
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public Preparation(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "Preparation";

		/// <summary>
		/// Description string
		/// </summary>
		public string DescriptionString => this.preparation;

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new Preparation(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.preparation = Values.Script.RemoveIndent(Definition.InnerText);

			return Task.CompletedTask;
		}

		/// <summary>
		/// Exports Markdown
		/// </summary>
		/// <param name="Output">Output</param>
		public override Task ExportMarkdown(StreamWriter Output)
		{
			Output.WriteLine("Preparation");
			Output.WriteLine("==============");
			Output.WriteLine();

			Output.WriteLine(this.preparation);
			Output.WriteLine();

			return base.ExportMarkdown(Output);
		}

		/// <summary>
		/// Exports XML
		/// </summary>
		/// <param name="Output">Output</param>
		public override Task ExportXml(XmlWriter Output)
		{
			Output.WriteElementString("Preparation", this.preparation);

			return base.ExportXml(Output);
		}
	}
}
