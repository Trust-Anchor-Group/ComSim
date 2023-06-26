using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel.Actors;
using Waher.Content.Xml;
using Waher.Networking.XMPP.PubSub;

namespace TAG.Simulator.XMPP.Extensions
{
	/// <summary>
	/// PubSub XMPP extension
	/// </summary>
	public class PubSubExtension : XmppExtension
	{
		private string componentAddress;

		/// <summary>
		/// PubSub XMPP extension
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public PubSubExtension(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(PubSubExtension);

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new PubSubExtension(Parent, Model);
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
		/// <returns>Extension object.</returns>
		public override Task<object> Add(IActor Instance, Waher.Networking.XMPP.XmppClient Client)
		{
			PubSubClient Extension = new PubSubClient(Client, this.componentAddress);
			Client.SetTag("PubSub", Extension);

			Extension.AffiliationNotification += (Sender, e) =>
			{
				this.Model.ExternalEvent(Instance, "AffiliationNotification",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client));

				return Task.CompletedTask;
			};

			Extension.ItemNotification += (Sender, e) =>
			{
				this.Model.ExternalEvent(Instance, "ItemNotification",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client));

				return Task.CompletedTask;
			};

			Extension.ItemRetracted += (Sender, e) =>
			{
				this.Model.ExternalEvent(Instance, "ItemRetracted",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client));

				return Task.CompletedTask;
			};

			Extension.NodePurged += (Sender, e) =>
			{
				this.Model.ExternalEvent(Instance, "NodePurged",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client));

				return Task.CompletedTask;
			};

			Extension.SubscriptionRequest += (Sender, e) =>
			{
				this.Model.ExternalEvent(Instance, "SubscriptionRequest",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client));

				return Task.CompletedTask;
			};

			Extension.SubscriptionStatusChanged += (Sender, e) =>
			{
				this.Model.ExternalEvent(Instance, "SubscriptionStatusChanged",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client));

				return Task.CompletedTask;
			};

			return Task.FromResult<object>(Extension);
		}

	}
}
