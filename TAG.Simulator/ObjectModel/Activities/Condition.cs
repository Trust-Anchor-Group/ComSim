using System;
using System.Threading.Tasks;
using System.Xml;
using Waher.Content;
using Waher.Content.Xml;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Activities
{
	/// <summary>
	/// Represents a condition.
	/// </summary>
	public class Condition : SimulationNodeChildren 
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
	}
}
