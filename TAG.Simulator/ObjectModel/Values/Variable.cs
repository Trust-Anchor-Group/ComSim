using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Waher.Content.Xml;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Values
{
	/// <summary>
	/// Value defined by variable.
	/// </summary>
	public class Variable : Value
	{
		private string name;

		/// <summary>
		/// Value defined by variable.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public Variable(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Variable name
		/// </summary>
		public string Name => this.name;

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "Variable";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new Variable(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.name = XML.Attribute(Definition, "name");

			return Task.CompletedTask;
		}

		/// <summary>
		/// Evaluates the value.
		/// </summary>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>Evaluated value.</returns>
		public override Task<object> EvaluateAsync(Variables Variables)
		{
			if (Variables.TryGetVariable(this.name, out Waher.Script.Variable v))
				return Task.FromResult<object>(v.ValueObject);
			else
				throw new Exception("Variable " + this.name + " not defined.");
		}

		/// <summary>
		/// Exports PlantUML
		/// </summary>
		/// <param name="Output">Output</param>
		/// <param name="Indentation">Number of tabs to indent.</param>
		/// <param name="QuoteChar">Quote character.</param>
		public override void ExportPlantUml(StreamWriter Output, int Indentation, char QuoteChar)
		{
			Output.Write(this.name);
		}

		/// <summary>
		/// Copies contents of the node to a new node.
		/// </summary>
		/// <param name="To">Node to receive copied contents.</param>
		public override void CopyContents(ISimulationNode To)
		{
			Variable TypedTo = (Variable)To;

			TypedTo.name = this.name;
		}
	}
}
