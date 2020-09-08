using System;
using System.Threading.Tasks;
using System.Xml;
using Waher.Content.Xml;

namespace TAG.Simulator.ObjectModel.Events
{
	/// <summary>
	/// Sets a variable value when an event is triggered.
	/// </summary>
	public class SetVariable : SimulationNodeChildren
	{
		private string name;

		/// <summary>
		/// Sets a variable value when an event is triggered.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		public SetVariable(ISimulationNode Parent)
			: base(Parent)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "SetVariable";

		/// <summary>
		/// Name of variable within the scope of the event.
		/// </summary>
		public string Name => this.name;

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent)
		{
			return new SetVariable(Parent);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.name = XML.Attribute(Definition, "name");

			return base.FromXml(Definition);
		}

	}
}
