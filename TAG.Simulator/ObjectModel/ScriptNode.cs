using System.Threading.Tasks;
using System.Xml;
using Waher.Script;

namespace TAG.Simulator.ObjectModel
{
	/// <summary>
	/// Abstract base class for simulation script nodes.
	/// </summary>
	public abstract class ScriptNode : SimulationNode
	{
		private string script;
		private Expression expression;

		/// <summary>
		/// Abstract base class for simulation script nodes.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public ScriptNode(ISimulationNode Parent, Model Model)
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
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.script = Values.Script.RemoveIndent(Definition.InnerText);
			this.expression = new Expression(this.script);

			return Task.CompletedTask;
		}
	}
}
