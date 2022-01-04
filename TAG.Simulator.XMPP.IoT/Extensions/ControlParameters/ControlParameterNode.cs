using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel.Actors;
using TAG.Simulator.ObjectModel.Events;
using TAG.Simulator.ObjectModel.Values;
using Waher.Content.Xml;
using Waher.Script;
using Waher.Things.ControlParameters;

namespace TAG.Simulator.XMPP.IoT.Extensions.ControlParameters
{
	/// <summary>
	/// Abstract base class for control parameter nodes.
	/// </summary>
	public abstract class ControlParameterNode : NodeReference, IValueRecipient, IEventPreparation
	{
		private IValue value = null;
		private string name;
		private string page;
		private string label;
		private string description;
		private string variable;
		private string actor;
		private string setEventId;
		private IEvent setEvent;

		/// <summary>
		/// Abstract base class for control parameter nodes.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public ControlParameterNode(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Value node
		/// </summary>
		public IValue Value => this.value;

		/// <summary>
		/// Field Name
		/// </summary>
		public string Name => this.name;

		/// <summary>
		/// Field Name
		/// </summary>
		public string Page => this.page;

		/// <summary>
		/// Label
		/// </summary>
		public string Label => this.label;

		/// <summary>
		/// Description
		/// </summary>
		public string Description => this.description;

		/// <summary>
		/// Variable name
		/// </summary>
		public string Variable => this.variable;

		/// <summary>
		/// Actor variable name
		/// </summary>
		public string Actor => this.actor;

		/// <summary>
		/// Set event
		/// </summary>
		public IEvent SetEvent => this.setEvent;

		/// <summary>
		/// ID of Set event.
		/// </summary>
		public string SetEventId => this.setEventId;

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.name = XML.Attribute(Definition, "name");
			this.page = XML.Attribute(Definition, "page");
			this.label = XML.Attribute(Definition, "label");
			this.description = XML.Attribute(Definition, "description");
			this.variable = XML.Attribute(Definition, "variable");
			this.actor = XML.Attribute(Definition, "actor");
			this.setEventId = XML.Attribute(Definition, "setEvent");

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
				throw new Exception("Field node already has a value defined.");
		}

		/// <summary>
		/// Starts the node.
		/// </summary>
		public override Task Start()
		{
			if (!this.Model.TryGetEvent(this.setEventId, out this.setEvent))
				throw new Exception("Event not found: " + this.setEventId);

			this.setEvent.Register(this);

			return base.Start();
		}

		/// <summary>
		/// Adds control parameters.
		/// </summary>
		/// <param name="Parameters">Generated list of control parameters.</param>
		/// <param name="Actor">Actor instance</param>
		public abstract void AddParameters(List<ControlParameter> Parameters, IActor Actor);

		/// <summary>
		/// Prepares <paramref name="Variables"/> for the execution of an event.
		/// </summary>
		/// <param name="Variables">Event variables</param>
		/// <param name="Tags">Extensible list of meta-data tags related to the event.</param>
		public Task Prepare(Variables Variables, List<KeyValuePair<string, object>> Tags)
		{
			return Task.CompletedTask;	// Do nothing.
		}

		/// <summary>
		/// Releases resources at the end of an event.
		/// </summary>
		/// <param name="Variables">Event variables</param>
		public void Release(Variables Variables)
		{
			// Do nothing.
		}

		/// <summary>
		/// Exports the node to PlantUML script in a markdown document.
		/// </summary>
		/// <param name="Output">Output stream.</param>
		/// <param name="Name">Optional name for the association.</param>
		/// <param name="Index">Chart Index</param>
		public void ExportPlantUml(StreamWriter Output, string Name, int Index)
		{
			Output.Write("actor \"");
			Output.Write(this.actor);
			Output.Write("\" as A");
			Output.Write(" <<");
			Output.Write(this.actor);
			Output.WriteLine(">>");
			Output.WriteLine("note right of A : Triggered when external\\ncontrol client sets parameter");
			Output.Write("A --> UC1 : ");
			Output.WriteLine(this.name);
		}
	}
}
