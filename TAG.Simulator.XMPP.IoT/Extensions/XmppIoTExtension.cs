using System;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel;
using TAG.Simulator.ObjectModel.Actors;
using TAG.Simulator.XMPP.Actors;
using TAG.Simulator.XMPP.Extensions;

namespace TAG.Simulator.XMPP.IoT.Extensions
{
	/// <summary>
	/// Abstract base class for IoT XMPP extensions.
	/// </summary>
	public abstract class IoTXmppExtension : XmppExtension
	{
		/// <summary>
		/// Abstract base class for IoT XMPP extensions.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public IoTXmppExtension(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Points to the embedded XML Schema resource defining the semantics of the XML namespace.
		/// </summary>
		public override string SchemaResource => "TAG.Simulator.XMPP.IoT.Schema.ComSimXmppIoT.xsd";

		/// <summary>
		/// XML Namespace where the element is defined.
		/// </summary>
		public override string Namespace => "http://trustanchorgroup.com/Schema/ComSim/XMPPIoT.xsd";

	}
}
