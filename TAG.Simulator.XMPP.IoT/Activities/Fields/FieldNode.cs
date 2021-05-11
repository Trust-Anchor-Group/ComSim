using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel.Values;
using Waher.Content.Xml;
using Waher.Script;
using Waher.Things.SensorData;

namespace TAG.Simulator.XMPP.IoT.Activities.Fields
{
	/// <summary>
	/// Abstract base class for sensor data field nodes.
	/// </summary>
	public abstract class FieldNode : NodeReference, IValueRecipient
	{
		private IValue value = null;
		private string name;
		private Waher.Things.SensorData.FieldType type;
		private FieldQoS qos;
		private bool writable;

		/// <summary>
		/// Abstract base class for sensor data field nodes.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public FieldNode(ISimulationNode Parent, Model Model)
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
		/// Field Type
		/// </summary>
		public Waher.Things.SensorData.FieldType Type => this.type;

		/// <summary>
		/// Field Quality of Service
		/// </summary>
		public FieldQoS QualityOfService => this.qos;

		/// <summary>
		/// If the field is writable (using a control parameter with the same name).
		/// </summary>
		public bool Writable => this.writable;

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.name = XML.Attribute(Definition, "name");
			this.type = (Waher.Things.SensorData.FieldType)XML.Attribute(Definition, "type", Waher.Things.SensorData.FieldType.Momentary);
			this.qos = (FieldQoS)XML.Attribute(Definition, "qos", FieldQoS.AutomaticReadout);
			this.writable = XML.Attribute(Definition, "writable", false);

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
		/// Adds sensor-data fields.
		/// </summary>
		/// <param name="Fields">Generated list of fields.</param>
		/// <param name="Variables">Set of variables for the activity.</param>
		public abstract void AddFields(LinkedList<Field> Fields, Variables Variables);
	}
}
