using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel.Actors;
using Waher.Content.Xml;
using Waher.Networking.XMPP.Provisioning;

namespace TAG.Simulator.XMPP.IoT.Extensions
{
	/// <summary>
	/// ThingRegistry Client XMPP extension
	/// </summary>
	public class ThingRegistryClientExtension : IoTXmppExtension
	{
		private string componentAddress;

		/// <summary>
		/// ThingRegistry Client XMPP extension
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public ThingRegistryClientExtension(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "ThingRegistryClientExtension";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new ThingRegistryClientExtension(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.componentAddress = XML.Attribute(Definition, "componentAddress");

			return base.FromXml(Definition);
		}

		/// <summary>
		/// Adds the extension to the client.
		/// </summary>
		/// <param name="Instance">Actor instance.</param>
		/// <param name="Client">XMPP Client</param>
		public override void Add(IActor Instance, Waher.Networking.XMPP.XmppClient Client)
		{
			if (Client.ContainsTag("ConcentratorServer"))
				throw new Exception("Define thing registry clients before any concentrator server extensions.");

			ThingRegistryClient Extension = new ThingRegistryClient(Client, this.componentAddress);
			Client.SetTag("ThingRegistryClient", Extension);

			Extension.Claimed += (Sender, e) =>
			{
				this.Model.ExternalEvent(Instance, "Claimed",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client));

				return Task.CompletedTask;
			};

			Extension.Disowned += (Sender, e) =>
			{
				this.Model.ExternalEvent(Instance, "Disowned",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client));

				return Task.CompletedTask;
			};

			Extension.Removed += (Sender, e) =>
			{
				this.Model.ExternalEvent(Instance, "Removed",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client));

				return Task.CompletedTask;
			};
		}
	}
}
