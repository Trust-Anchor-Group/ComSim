using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using Waher.Content.Xml;
using Waher.Runtime.Inventory;
using Waher.Script;
using Waher.Things.SensorData;

namespace TAG.Simulator.XMPP.IoT.Activities.Fields
{
	/// <summary>
	/// Enum sensor data field node.
	/// </summary>
	public class EnumField : FieldNode
	{
		private string enumTypeName;
		private Type enumType;

		/// <summary>
		/// Enum sensor data field node.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public EnumField(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "EnumField";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new EnumField(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.enumTypeName = XML.Attribute(Definition, "enumType");
			this.enumType = Types.GetType(this.enumTypeName);

			if (this.enumType is null)
				throw new Exception("Type not found: " + this.enumTypeName);

			if (!this.enumType.GetTypeInfo().IsEnum)
				throw new Exception("Not an enumeration type: " + this.enumTypeName);

			return base.FromXml(Definition);
		}

		/// <summary>
		/// Adds sensor-data fields.
		/// </summary>
		/// <param name="Fields">Generated list of fields.</param>
		/// <param name="Variables">Set of variables for the activity.</param>
		public override void AddFields(List<Field> Fields, Variables Variables)
		{
			object Value = this.Value.Evaluate(Variables);

			if (!(Value is Enum TypedValue))
				TypedValue = (Enum)Enum.Parse(this.enumType, Value.ToString());

			Fields.Add(new Waher.Things.SensorData.EnumField(this.ThingReference, DateTime.Now, this.Name, TypedValue, this.Type, this.QualityOfService, this.Writable));
		}

	}
}
