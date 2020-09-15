using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Values
{
	/// <summary>
	/// Value defined by script.
	/// </summary>
	public class Script : Value
	{
		private string script;
		private Expression expression;

		/// <summary>
		/// Abstract base class of values
		/// </summary>
		/// <param name="Parent">Parent node</param>
		public Script(ISimulationNode Parent)
			: base(Parent)
		{
		}

		/// <summary>
		/// Script string
		/// </summary>
		public string ScriptString => this.script;

		/// <summary>
		/// Parsed expression
		/// </summary>
		public Expression Expression => this.expression;

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "Script";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent)
		{
			return new Script(Parent);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.script = Definition.InnerText;
			this.expression = new Expression(this.script);

			return Task.CompletedTask;
		}

		/// <summary>
		/// Evaluates the value.
		/// </summary>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>Evaluated value.</returns>
		public override object Evaluate(Variables Variables)
		{
			return this.expression.Evaluate(Variables);
		}

		/// <summary>
		/// Exports PlantUML
		/// </summary>
		/// <param name="Output">Output node</param>
		/// <param name="Indentation">Number of tabs to indent.</param>
		public override void ExportPlantUml(StreamWriter Output, int Indentation)
		{
			Activities.Eval.ExportPlantUml(this.script, Output, Indentation, false);
		}
	}
}
