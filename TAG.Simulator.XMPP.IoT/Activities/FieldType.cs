using System;
using System.Threading.Tasks;
using System.Xml;

namespace TAG.Simulator.XMPP.IoT.Activities
{
	/// <summary>
	/// Field type.
	/// </summary>
	public class FieldType : XmppIoTNode
	{
		private Waher.Things.SensorData.FieldType type;

		/// <summary>
		/// Field reference.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public FieldType(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "FieldType";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new FieldType(Parent, Model);
		}

		/// <summary>
		/// Field Type
		/// </summary>
		public Waher.Things.SensorData.FieldType Type => this.type;

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			if (!Enum.TryParse<Waher.Things.SensorData.FieldType>(Definition.InnerText, out this.type))
				throw new Exception("Invalid field type: " + Definition.InnerText);

			return base.FromXml(Definition);
		}

		/// <summary>
		/// If children are to be parsed by <see cref="FromXml(XmlElement)"/>
		/// </summary>
		public override bool ParseChildren => false;
	}
}
