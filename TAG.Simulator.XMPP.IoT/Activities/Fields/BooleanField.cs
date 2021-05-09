﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using Waher.Script;
using Waher.Things.SensorData;

namespace TAG.Simulator.XMPP.IoT.Activities.Fields
{
	/// <summary>
	/// Boolean sensor data field node.
	/// </summary>
	public class BooleanField : FieldNode
	{
		/// <summary>
		/// Boolean sensor data field node.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public BooleanField(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "BooleanField";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new BooleanField(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			return base.FromXml(Definition);
		}

		/// <summary>
		/// Adds sensor-data fields.
		/// </summary>
		/// <param name="Fields">Generated list of fields.</param>
		/// <param name="Variables">Set of variables for the activity.</param>
		public override void AddFields(LinkedList<Field> Fields, Variables Variables)
		{
			object Value = this.Value.Evaluate(Variables);

			if (!(Value is bool TypedValue))
				TypedValue = Convert.ToBoolean(Value);

			Fields.AddLast(new Waher.Things.SensorData.BooleanField(this.ThingReference, DateTime.Now, this.Name, TypedValue, this.Type, this.QualityOfService, this.Writable));
		}

	}
}
