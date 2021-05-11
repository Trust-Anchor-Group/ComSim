using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel.Activities;
using TAG.Simulator.ObjectModel.Actors;
using TAG.Simulator.ObjectModel.Values;
using Waher.Content.Xml;
using Waher.Things.ControlParameters;

namespace TAG.Simulator.XMPP.IoT.Extensions.ControlParameters
{
	/// <summary>
	/// Abstract base class for control parameter nodes.
	/// </summary>
	public abstract class ControlParameterNode : NodeReference, IValueRecipient
	{
		private IValue value = null;
		private string name;
		private string page;
		private string label;
		private string description;
		private string variable;
		private string actor;
		private string setActivityId;
		private IActivity setActivity;

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
		/// Set activity
		/// </summary>
		public IActivity SetActivity => this.setActivity;

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
			this.setActivityId = XML.Attribute(Definition, "setActivity");

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
			if (!this.Model.TryGetActivity(this.setActivityId, out this.setActivity))
				throw new Exception("Activity not found: " + this.setActivityId);

			return base.Start();
		}

		/// <summary>
		/// Adds control parameters.
		/// </summary>
		/// <param name="Parameters">Generated list of control parameters.</param>
		/// <param name="Actor">Actor instance</param>
		public abstract void AddParameters(List<ControlParameter> Parameters, IActor Actor);
	}
}
