using System;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel.Values;
using Waher.Content.Xml;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Events
{
	/// <summary>
	/// Sets a variable value when an event is triggered.
	/// </summary>
	public class SetVariable : EventPreparation, IValueRecipient
	{
		private IValue value = null;
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

		/// <summary>
		/// Registers a value for the argument.
		/// </summary>
		/// <param name="Value">Value node</param>
		public void Register(IValue Value)
		{
			if (this.value is null)
				this.value = Value;
			else
				throw new Exception("SetVariable node already has a value defined.");
		}

		/// <summary>
		/// Prepares <paramref name="Variables"/> for the execution of an event.
		/// </summary>
		/// <param name="Model">Current model</param>
		/// <param name="Variables">Event variables</param>
		public override void Prepare(Model Model, Variables Variables)
		{
			Variables[this.name] = this.value?.Evaluate(Variables);
		}

		/// <summary>
		/// Releases resources at the end of an event.
		/// </summary>
		/// <param name="Model">Current model</param>
		/// <param name="Variables">Event variables</param>
		public override void Release(Model Model, Variables Variables)
		{
			Variables.Remove(this.name);
		}
	}
}
