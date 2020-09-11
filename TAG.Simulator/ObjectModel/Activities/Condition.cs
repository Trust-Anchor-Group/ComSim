using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using Waher.Content;
using Waher.Content.Xml;
using Waher.Events;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Activities
{
	/// <summary>
	/// Represents a condition.
	/// </summary>
	public class Condition : ActivityNode, IConditionNode
	{
		private string condition;
		private Expression expression;

		/// <summary>
		/// Represents a condition.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		public Condition(ISimulationNode Parent)
			: base(Parent)
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
		public override string LocalName => "Condition";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent)
		{
			return new Condition(Parent);
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
		/// <param name="Model">Model being executed.</param>
		public override Task Initialize(Model Model)
		{
			if (this.Parent is Conditional Conditional)
				Conditional.Register(this);

			return base.Initialize(Model);
		}

		/// <summary>
		/// Executes a node.
		/// </summary>
		/// <param name="Model">Current model</param>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>Next node of execution, if different from the default, otherwise null (for default).</returns>
		public override async Task<LinkedListNode<IActivityNode>> Execute(Model Model, Variables Variables)
		{
			await Activity.ExecuteActivity(Model, Variables, this.FirstNode);
			return null;
		}

		/// <summary>
		/// If the node condition is true.
		/// </summary>
		/// <param name="Model">Current model</param>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>If embedded nodes are to be executed.</returns>
		public bool IsTrue(Model Model, Variables Variables)
		{
			try
			{
				object Result = this.expression.Evaluate(Variables);
				return Result is bool b && b;
			}
			catch (Exception ex)
			{
				Log.Critical(ex);
				return false;
			}
		}
	}
}
