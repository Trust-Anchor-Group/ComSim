using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using Waher.Content.Xml;
using Waher.Script;
using Waher.Things.SensorData;

namespace TAG.Simulator.XMPP.IoT.Activities.Fields
{
	/// <summary>
	/// Quantity sensor data field node.
	/// </summary>
	public class QuantityField : FieldNode
	{
		private string unit;
		private byte nrDecimals;

		/// <summary>
		/// Quantity sensor data field node.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public QuantityField(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(QuantityField);

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new QuantityField(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.unit = XML.Attribute(Definition, "unit");
			this.nrDecimals = (byte)XML.Attribute(Definition, "nrDecimals", 0);

			return base.FromXml(Definition);
		}

		/// <summary>
		/// Adds sensor-data fields.
		/// </summary>
		/// <param name="Fields">Generated list of fields.</param>
		/// <param name="Variables">Set of variables for the activity.</param>
		public override async Task AddFields(LinkedList<Field> Fields, Variables Variables)
		{
			object Value = await this.Value.EvaluateAsync(Variables);

			if (!(Value is double TypedValue))
				TypedValue = Convert.ToDouble(Value);
			
			Fields.AddLast(new Waher.Things.SensorData.QuantityField(this.ThingReference, DateTime.Now, this.Name, TypedValue, this.nrDecimals, this.unit, this.Type, this.QualityOfService, this.Writable));
		}

	}
}
