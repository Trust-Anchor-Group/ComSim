using System;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel.Values;
using Waher.Content.Xml;

namespace TAG.Simulator.ObjectModel.Activities.Execution
{
	/// <summary>
	/// Defines an argument in an action.
	/// </summary>
	public class Argument : SimulationNodeChildren, IValueRecipient
	{
		private IValue value = null;
		private string name;

		/// <summary>
		/// Defines an argument in an action.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public Argument(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Defines an argument in an action.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <param name="Name">Name of argument.</param>
		/// <param name="Value">Argument value.</param>
		public Argument(ISimulationNode Parent, Model Model, string Name, Waher.Script.Expression Value)
			: base(Parent, Model)
		{
			this.name = Name;
			this.AddChild(new Script(this, Model, Value));
		}

		/// <summary>
		/// Defines an argument in an action.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <param name="Name">Name of argument.</param>
		/// <param name="Value">Argument value.</param>
		public Argument(ISimulationNode Parent, Model Model, string Name, System.DateTime Value)
			: base(Parent, Model)
		{
			this.name = Name;
			this.AddChild(new Values.DateTime(this, Model, Value));
		}

		/// <summary>
		/// Defines an argument in an action.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <param name="Name">Name of argument.</param>
		/// <param name="Value">Argument value.</param>
		public Argument(ISimulationNode Parent, Model Model, string Name, Waher.Content.Duration Value)
			: base(Parent, Model)
		{
			this.name = Name;
			this.AddChild(new Duration(this, Model, Value));
		}

		/// <summary>
		/// Defines an argument in an action.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <param name="Name">Name of argument.</param>
		/// <param name="Value">Argument value.</param>
		public Argument(ISimulationNode Parent, Model Model, string Name, double Value)
			: base(Parent, Model)
		{
			this.name = Name;
			this.AddChild(new Number(this, Model, Value));
		}

		/// <summary>
		/// Defines an argument in an action.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <param name="Name">Name of argument.</param>
		/// <param name="Value">Argument value.</param>
		public Argument(ISimulationNode Parent, Model Model, string Name, string Value)
			: base(Parent, Model)
		{
			this.name = Name;
			this.AddChild(new Values.String(this, Model, Value));
		}

		/// <summary>
		/// Defines an argument in an action.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <param name="Name">Name of argument.</param>
		/// <param name="Value">Argument value.</param>
		public Argument(ISimulationNode Parent, Model Model, string Name, TimeSpan Value)
			: base(Parent, Model)
		{
			this.name = Name;
			this.AddChild(new Time(this, Model, Value));
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(Argument);

		/// <summary>
		/// Name of variable within the scope of the event.
		/// </summary>
		public string Name => this.name;

		/// <summary>
		/// Value node.
		/// </summary>
		public IValue Value => this.value;

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new Argument(Parent, Model);
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

		/// <summary>
		/// Registers a value for the argument.
		/// </summary>
		/// <param name="Value">Value node</param>
		public void Register(IValue Value)
		{
			if (this.value is null)
				this.value = Value;
			else
				throw new Exception("Argument already has a value defined.");
		}

	}
}
