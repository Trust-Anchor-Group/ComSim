using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Values
{
	/// <summary>
	/// String value.
	/// </summary>
	public class String : Value
	{
		private string value;

		/// <summary>
		/// String value.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public String(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Value
		/// </summary>
		public string Value => this.value;

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "String";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new String(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.value = Definition.InnerText;

			return Task.CompletedTask;
		}

		/// <summary>
		/// Evaluates the value.
		/// </summary>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>Evaluated value.</returns>
		public override object Evaluate(Variables Variables)
		{
			return this.value;
		}

		/// <summary>
		/// Exports PlantUML
		/// </summary>
		/// <param name="Output">Output node</param>
		/// <param name="Indentation">Number of tabs to indent.</param>
		public override void ExportPlantUml(StreamWriter Output, int Indentation)
		{
			Activities.Eval.ExportPlantUml("\"" + this.value.Replace("\"","\\\"") + "\"", Output, Indentation, false);
		}
	}
}
