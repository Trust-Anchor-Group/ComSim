using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Activities
{
	/// <summary>
	/// Executes script in an activity.
	/// </summary>
	public class Eval : ActivityNode 
	{
		private string script;
		private Expression expression;

		/// <summary>
		/// Represents a delay in an activity.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public Eval(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Script string
		/// </summary>
		public string Script => this.script;

		/// <summary>
		/// Parsed expression
		/// </summary>
		public Expression Expression => this.expression;

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "Eval";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new Eval(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.script = Definition.InnerText;
			this.expression = new Expression(this.script);

			return base.FromXml(Definition);
		}

		/// <summary>
		/// If children are 
		/// </summary>
		public override bool ParseChildren => false;

		/// <summary>
		/// Executes a node.
		/// </summary>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>Next node of execution, if different from the default, otherwise null (for default).</returns>
		public override Task<LinkedListNode<IActivityNode>> Execute(Variables Variables)
		{
			this.expression.Evaluate(Variables);

			return Task.FromResult<LinkedListNode<IActivityNode>>(null);
		}

		/// <summary>
		/// Exports PlantUML
		/// </summary>
		/// <param name="Output">Output node</param>
		/// <param name="Indentation">Number of tabs to indent.</param>
		/// <param name="QuoteChar">Quote character.</param>
		public override void ExportPlantUml(StreamWriter Output, int Indentation, char QuoteChar)
		{
			ExportPlantUml(this.script, Output, Indentation, QuoteChar, true);
		}

		/// <summary>
		/// Exports PlantUML
		/// </summary>
		/// <param name="Script">Script expression.</param>
		/// <param name="Output">Output node</param>
		/// <param name="Indentation">Number of tabs to indent.</param>
		/// <param name="QuoteChar">Quote character.</param>
		/// <param name="Delimiters">If delimiters : and ; should be included at the beginning and end.</param>
		public static void ExportPlantUml(string Script, StreamWriter Output, int Indentation, char QuoteChar, bool Delimiters)
		{
			bool First = true;

			if (QuoteChar != '"')
				Script = Script.Replace('"', QuoteChar);

			foreach (string Row in Script.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n'))
			{
				if (First)
				{
					if (Delimiters)
					{
						Indent(Output, Indentation);
						Output.Write(':');
					}

					First = false;
				}
				else
				{
					Output.WriteLine();
					Indent(Output, Indentation);
				}

				Output.Write(Row);
			}

			if (!First && Delimiters)
				Output.WriteLine(";");
		}
	}
}
