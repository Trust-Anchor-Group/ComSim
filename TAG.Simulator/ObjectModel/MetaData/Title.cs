using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Waher.Content.Xml;

namespace TAG.Simulator.ObjectModel.MetaData
{
	/// <summary>
	/// Title of model
	/// </summary>
	public class Title : SimulationNode
	{
		private string title;

		/// <summary>
		/// Title of model
		/// </summary>
		/// <param name="Parent">Parent node</param>
		public Title(ISimulationNode Parent)
			: base(Parent)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "Title";

		/// <summary>
		/// Title string
		/// </summary>
		public string TitleString => this.title;

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent)
		{
			return new Title(Parent);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.title = Definition.InnerText;

			return Task.CompletedTask;
		}

		/// <summary>
		/// Exports Markdown
		/// </summary>
		/// <param name="Output">Output node</param>
		public override Task ExportMarkdown(StreamWriter Output)
		{
			Output.WriteLine("Title: " + this.title);
			Output.WriteLine("Date: " + DateTime.Today.ToString("d"));
			Output.WriteLine("Description: This document contains the results of executing a simulation.");
			Output.WriteLine();
			Output.WriteLine(new string('=', 80));
			Output.WriteLine();
			Output.WriteLine(this.title);
			Output.WriteLine(new string('=', this.title.Length + 3));
			Output.WriteLine();

			return base.ExportMarkdown(Output);
		}

		/// <summary>
		/// Exports XML
		/// </summary>
		/// <param name="Output">Output node</param>
		public override Task ExportXml(XmlWriter Output)
		{
			Output.WriteElementString("Title", this.title);
			Output.WriteElementString("DateTime", XML.Encode(DateTime.Now));

			return base.ExportXml(Output);
		}
	}
}
