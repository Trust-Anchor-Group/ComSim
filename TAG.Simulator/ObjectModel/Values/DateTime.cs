using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Waher.Content.Xml;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Values
{
	/// <summary>
	/// DateTime value.
	/// </summary>
	public class DateTime : Value
	{
		private System.DateTime value;

		/// <summary>
		/// DateTime value.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public DateTime(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// DateTime value.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <param name="Value">Value.</param>
		public DateTime(ISimulationNode Parent, Model Model, System.DateTime Value)
			: base(Parent, Model)
		{
			this.value = Value;
		}

		/// <summary>
		/// Value
		/// </summary>
		public System.DateTime Value => this.value;

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "DateTime";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new DateTime(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.value = XML.Attribute(Definition, "value", System.DateTime.MinValue);

			return Task.CompletedTask;
		}

		/// <summary>
		/// Evaluates the value.
		/// </summary>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>Evaluated value.</returns>
		public override Task<object> EvaluateAsync(Variables Variables)
		{
			return Task.FromResult<object>(this.value);
		}

		/// <summary>
		/// Exports PlantUML
		/// </summary>
		/// <param name="Output">Output</param>
		/// <param name="Indentation">Number of tabs to indent.</param>
		/// <param name="QuoteChar">Quote character.</param>
		public override void ExportPlantUml(StreamWriter Output, int Indentation, char QuoteChar)
		{
			Output.Write(this.value.ToString());
		}
	}
}
