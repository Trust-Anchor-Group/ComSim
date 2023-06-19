using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Waher.Content;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Values
{
	/// <summary>
	/// JSON value.
	/// </summary>
	public class Json : Value
	{
		private Expression script;

		/// <summary>
		/// JSON value.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public Json(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of JSON element defining contents of class.
		/// </summary>
		public override string LocalName => "Json";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new Json(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with JSON definition.
		/// </summary>
		/// <param name="Definition">JSON definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			string s = Definition.InnerText;
			this.script = new Expression(s);

			return Task.CompletedTask;
		}

		/// <summary>
		/// Evaluates the value.
		/// </summary>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>Evaluated value.</returns>
		public override async Task<object> EvaluateAsync(Variables Variables)
		{
			object Value = await this.script.EvaluateAsync(Variables);
			return JSON.Encode(Value, false);
		}

		/// <summary>
		/// Exports PlantUML
		/// </summary>
		/// <param name="Output">Output</param>
		/// <param name="Indentation">Number of tabs to indent.</param>
		/// <param name="QuoteChar">Quote character.</param>
		public override void ExportPlantUml(StreamWriter Output, int Indentation, char QuoteChar)
		{
			Activities.Eval.ExportPlantUml("JSON", Output, Indentation, QuoteChar, false);
		}

		/// <summary>
		/// Copies contents of the node to a new node.
		/// </summary>
		/// <param name="To">Node to receive copied contents.</param>
		public override void CopyContents(ISimulationNode To)
		{
			Json TypedTo = (Json)To;

			TypedTo.script = this.script;
		}
	}
}
