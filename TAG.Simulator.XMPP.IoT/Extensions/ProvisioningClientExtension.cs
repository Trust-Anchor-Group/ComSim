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
	/// Provisioning Client XMPP extension
	/// </summary>
	public class ProvisioningClientExtension : IoTXmppExtension
	{
		private string componentAddress;

		/// <summary>
		/// Provisioning Client XMPP extension
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public ProvisioningClientExtension(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "ProvisioningClientExtension";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new ProvisioningClientExtension(Parent, Model);
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
		public override Task Add(IActor Instance, Waher.Networking.XMPP.XmppClient Client)
		{
			if (Client.ContainsTag("SensorServer"))
				throw new Exception("Define provisioning clients before any sensor server extensions.");

			if (Client.ContainsTag("ControlServer"))
				throw new Exception("Define provisioning clients before any control server extensions.");

			if (Client.ContainsTag("ConcentratorServer"))
				throw new Exception("Define provisioning clients before any concentrator server extensions.");

			ProvisioningClient Extension = new ProvisioningClient(Client, this.componentAddress);
			Client.SetTag("ProvisioningClient", Extension);

			Extension.CanControlQuestion += (Sender, e) =>
			{
				this.Model.ExternalEvent(Instance, "OnExecuteReadoutRequest",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client));

				return Task.CompletedTask;
			};

			Extension.CanReadQuestion += (Sender, e) =>
			{
				this.Model.ExternalEvent(Instance, "CanReadQuestion",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client));

				return Task.CompletedTask;
			};

			Extension.IsFriendQuestion += (Sender, e) =>
			{
				this.Model.ExternalEvent(Instance, "IsFriendQuestion",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client));

				return Task.CompletedTask;
			};

			Extension.CacheCleared += (Sender, e) =>
			{
				this.Model.ExternalEvent(Instance, "CacheCleared",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client));
			};

			return Task.CompletedTask;
		}
	}
}
