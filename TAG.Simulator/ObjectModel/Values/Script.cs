using System;
using System.IO;
using System.Text;
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
		/// Value defined by script.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public Script(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Value defined by script.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <param name="Script">Script expression</param>
		public Script(ISimulationNode Parent, Model Model, Expression Script)
			: base(Parent, Model)
		{
			this.script = Script.Script;
			this.expression = Script;
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
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new Script(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.script = RemoveIndent(Definition.InnerText);
			this.expression = new Expression(this.script);

			return Task.CompletedTask;
		}

		/// <summary>
		/// Removes indentation from a block of text.
		/// </summary>
		/// <param name="Text">Text</param>
		/// <returns>Unindented text</returns>
		public static string RemoveIndent(string Text)
		{
			string[] Rows = Text.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
			int j = 0;
			int d = Rows.Length;

			while (j < d && string.IsNullOrEmpty(Rows[j].Trim()))
				j++;

			while (d > j && string.IsNullOrEmpty(Rows[d - 1].Trim()))
				d--;

			if (j == d)
				return string.Empty;

			string Row = Rows[j];
			int i = 0;
			int c = Row.Length;
			char ch;

			while (i < c && ((ch = Row[i]) <= ' ' || ch == 160))
				i++;

			if (i == 0 || j == 0)
				return Text.TrimEnd();

			StringBuilder sb = new StringBuilder();
			string Indent = Row.Substring(0, i);

			sb.Append(Row.Substring(i));
			j++;

			while (j < d)
			{
				sb.AppendLine();
				Row = Rows[j++];

				if (Row.StartsWith(Indent))
					sb.Append(Row.Substring(i));
				else
					sb.Append(Row);
			}

			return sb.ToString();
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
		/// <param name="Output">Output</param>
		/// <param name="Indentation">Number of tabs to indent.</param>
		/// <param name="QuoteChar">Quote character.</param>
		public override void ExportPlantUml(StreamWriter Output, int Indentation, char QuoteChar)
		{
			Activities.Eval.ExportPlantUml(this.script, Output, Indentation, QuoteChar, false);
		}
	}
}
