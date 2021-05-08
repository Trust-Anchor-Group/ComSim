using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel.Values;
using Waher.Content.Xml;
using Waher.Script;
using Waher.Things;
using Waher.Things.SensorData;

namespace TAG.Simulator.XMPP.IoT.Activities.Fields
{
	/// <summary>
	/// Abstract base class for sensor data field nodes.
	/// </summary>
	public abstract class FieldNode : XmppIoTNode, IValueRecipient
	{
		private IValue value = null;
		private ThingReference thingRef;
		private string source;
		private string partition;
		private string node;
		private string name;
		private FieldType type;
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
		/// Data source
		/// </summary>
		public string Source => this.source;

		/// <summary>
		/// Partition
		/// </summary>
		public string Partition => this.partition;

		/// <summary>
		/// Node ID
		/// </summary>
		public string Node => this.node;

		/// <summary>
		/// Field Name
		/// </summary>
		public string Name => this.name;

		/// <summary>
		/// Field Type
		/// </summary>
		public FieldType Type => this.type;

		/// <summary>
		/// Field Quality of Service
		/// </summary>
		public FieldQoS QualityOfService => this.qos;

		/// <summary>
		/// If the field is writable (using a control parameter with the same name).
		/// </summary>
		public bool Writable => this.writable;

		/// <summary>
		/// Thing reference.
		/// </summary>
		public ThingReference ThingReference => this.thingRef;

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.source = XML.Attribute(Definition, "source");
			this.partition = XML.Attribute(Definition, "partition");
			this.node = XML.Attribute(Definition, "node");
			this.name = XML.Attribute(Definition, "name");
			this.type = (FieldType)XML.Attribute(Definition, "type", FieldType.Momentary);
			this.qos = (FieldQoS)XML.Attribute(Definition, "qos", FieldQoS.AutomaticReadout);
			this.writable = XML.Attribute(Definition, "writable", false);

			this.thingRef = new ThingReference(this.node, this.source, this.partition);

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
		public abstract void AddFields(List<Field> Fields, Variables Variables);
	}
}
