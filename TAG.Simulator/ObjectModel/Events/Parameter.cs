using System;
using System.Threading.Tasks;
using System.Xml;
using Waher.Content.Xml;

namespace TAG.Simulator.ObjectModel.Events
{
	/// <summary>
	/// Sets a variable to the value of an event parameter.
	/// </summary>
	public class Parameter : SimulationNode
	{
		private string name;
		private string variable;

		/// <summary>
		/// Sets a variable to the value of an event parameter.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		public Parameter(ISimulationNode Parent)
			: base(Parent)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "Parameter";

		/// <summary>
		/// Name of external event
		/// </summary>
		public string Name => this.name;

		/// <summary>
		/// Optional name for variable, if different from <see cref="Name"/>
		/// </summary>
		public string Variable => this.variable;

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent)
		{
			return new Parameter(Parent);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.name = XML.Attribute(Definition, "name");
			this.variable = XML.Attribute(Definition, "variable");

			return Task.CompletedTask;
		}

		/// <summary>
		/// Initialized the node before simulation.
		/// </summary>
		/// <param name="Model">Model being executed.</param>
		public override Task Initialize(Model Model)
		{
			(this.Parent as ExternalEvent)?.Register(this);
			return Task.CompletedTask;
		}

	}
}
