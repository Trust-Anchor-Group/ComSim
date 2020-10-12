using System;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel;
using TAG.Simulator.ObjectModel.Actors;
using TAG.Simulator.XMPP.Actors;

namespace TAG.Simulator.XMPP.Extensions
{
	/// <summary>
	/// Abstract base class for XMPP extensions.
	/// </summary>
	public abstract class XmppExtension : SimulationNode, IXmppExtension
	{
		/// <summary>
		/// Abstract base class for XMPP extensions.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public XmppExtension(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

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
			return Task.CompletedTask;
		}

		/// <summary>
		/// Adds the extension to the client.
		/// </summary>
		/// <param name="Instance">Actor instance.</param>
		/// <param name="Client">XMPP Client</param>
		public abstract void Add(IActor Instance, Waher.Networking.XMPP.XmppClient Client);
	}
}
