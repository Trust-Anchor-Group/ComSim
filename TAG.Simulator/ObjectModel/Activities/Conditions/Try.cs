using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.Extensions;
using Waher.Content.Xml;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Activities.Conditions
{
	/// <summary>
	/// Represents a statement that may or may not fail.
	/// </summary>
	public class Try : ActivityNode, IConditionNode
	{
		private string condition;
		private Expression expression;

		/// <summary>
		/// Represents a statement that may or may not fail.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public Try(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Condition string
		/// </summary>
		public string ConditionString => this.condition;

		/// <summary>
		/// Parsed expression
		/// </summary>
		public Expression Expression => this.expression;

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(Try);

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new Try(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.condition = XML.Attribute(Definition, "condition");
			this.expression = new Expression(this.condition);

			return base.FromXml(Definition);
		}

		/// <summary>
		/// Initialized the node before simulation.
		/// </summary>
		public override Task Initialize()
		{
			if (this.Parent is Conditional Conditional)
				Conditional.Register(this);

			return base.Initialize();
		}

		/// <summary>
		/// Executes a node.
		/// </summary>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>Next node of execution, if different from the default, otherwise null (for default).</returns>
		public override async Task<LinkedListNode<IActivityNode>> Execute(Variables Variables)
		{
			await Activity.ExecuteActivity(Variables, this.FirstNode);
			return null;
		}

		/// <summary>
		/// If the node condition is true.
		/// </summary>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>If embedded nodes are to be executed.</returns>
		public async Task<bool> IsTrue(Variables Variables)
		{
			try
			{
				await this.expression.EvaluateAsync(Variables);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		/// <summary>
		/// Exports PlantUML
		/// </summary>
		/// <param name="Output">Output</param>
		/// <param name="Indentation">Number of tabs to indent.</param>
		/// <param name="First">If the condition is the first condition.</param>
		/// <param name="QuoteChar">Quote character.</param>
		public void ExportPlantUml(StreamWriter Output, int Indentation, bool First, char QuoteChar)
		{
			Output.Indent(Indentation);

			if (!First)
				Output.Write("else");

			Output.Write("if (");
			Output.Write(this.condition);
			Output.WriteLine(") then (successful)");

			base.ExportPlantUml(Output, Indentation + 1, QuoteChar);
		}

	}
}
