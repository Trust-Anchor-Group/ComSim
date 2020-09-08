using System;
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
		public Variable(ISimulationNode Parent)
			: base(Parent)
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
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent)
		{
			return new Variable(Parent);
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
	}
}
