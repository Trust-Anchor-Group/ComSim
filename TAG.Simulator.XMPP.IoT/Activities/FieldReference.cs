using System;
using System.Threading.Tasks;
using System.Xml;
using Waher.Content.Xml;
using Waher.Things;

namespace TAG.Simulator.XMPP.IoT.Activities
{
	/// <summary>
	/// Field reference.
	/// </summary>
	public class FieldReference : XmppIoTNode
	{
		private string name;

		/// <summary>
		/// Field reference.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public FieldReference(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "FieldReference";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new FieldReference(Parent, Model);
		}

		/// <summary>
		/// Field Name
		/// </summary>
		public string Name => this.name;

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.name = Definition.InnerText;

			return base.FromXml(Definition);
		}

		/// <summary>
		/// If children are to be parsed by <see cref="FromXml(XmlElement)"/>
		/// </summary>
		public override bool ParseChildren => false;
	}
}
