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
		/// <param name="Model">Model in which the node is defined.</param>
		public Parameter(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Sets a variable to the value of an event parameter.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <param name="Name">Parameter name</param>
		/// <param name="Variable">Variable name</param>
		public Parameter(ISimulationNode Parent, Model Model, string Name, string Variable)
			: base(Parent, Model)
		{
			this.name = Name;
			this.variable = Variable;
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(Parameter);

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
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new Parameter(Parent, Model);
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
		public override Task Initialize()
		{
			(this.Parent as ExternalEvent)?.Register(this);
			return Task.CompletedTask;
		}
	}
}
