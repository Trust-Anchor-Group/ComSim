using System;
using System.Threading.Tasks;
using System.Xml;
using Waher.Content.Xml;
using Waher.Things;

namespace TAG.Simulator.XMPP.IoT.Activities
{
	/// <summary>
	/// Node reference.
	/// </summary>
	public class NodeReference : XmppIoTNode
	{
		private ThingReference thingRef;
		private string source;
		private string partition;
		private string node;

		/// <summary>
		/// Node reference.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public NodeReference(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "NodeReference";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new NodeReference(Parent, Model);
		}

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

			this.thingRef = new ThingReference(this.node, this.source, this.partition);

			return base.FromXml(Definition);
		}
	}
}
