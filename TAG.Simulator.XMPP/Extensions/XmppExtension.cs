using System;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel;
using TAG.Simulator.ObjectModel.Actors;
using TAG.Simulator.XMPP.Actors;
using Waher.Content.Xml;

namespace TAG.Simulator.XMPP.Extensions
{
	/// <summary>
	/// Abstract base class for XMPP extensions.
	/// </summary>
	public abstract class XmppExtension : SimulationNodeChildren, IXmppExtension
	{
		private string id;
		private readonly string instanceId;
		private readonly int instanceIndex;

		/// <summary>
		/// Abstract base class for XMPP extensions.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public XmppExtension(ISimulationNode Parent, Model Model)
			: this(Parent, Model, 0, string.Empty)
		{
		}

		/// <summary>
		/// Abstract base class for XMPP extensions.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <param name="InstanceIndex">Instance index.</param>
		/// <param name="InstanceId">ID of instance</param>
		public XmppExtension(ISimulationNode Parent, Model Model, int InstanceIndex, string InstanceId)
			: base(Parent, Model)
		{
			this.instanceIndex = InstanceIndex;
			this.instanceId = InstanceId;
		}

		/// <summary>
		/// ID of actor.
		/// </summary>
		public string Id => this.id;

		/// <summary>
		/// ID of actor instance.
		/// </summary>
		public string InstanceId => this.instanceId;

		/// <summary>
		/// Actor instance index.
		/// </summary>
		public int InstanceIndex => this.instanceIndex;

		/// <summary>
		/// Points to the embedded XML Schema resource defining the semantics of the XML namespace.
		/// </summary>
		public override string SchemaResource => XmppActor.XmppSchema;

		/// <summary>
		/// XML Namespace where the element is defined.
		/// </summary>
		public override string Namespace => XmppActor.XmppNamespace;

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.id = XML.Attribute(Definition, "id");

			return base.FromXml(Definition);
		}

		/// <summary>
		/// Adds the extension to the client.
		/// </summary>
		/// <param name="Instance">Actor instance.</param>
		/// <param name="Client">XMPP Client</param>
		/// <returns>Extension object.</returns>
		public abstract Task<object> Add(IActor Instance, Waher.Networking.XMPP.XmppClient Client);
	}
}
