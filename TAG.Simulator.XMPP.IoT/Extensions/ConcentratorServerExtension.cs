using System;
using System.Threading.Tasks;
using TAG.Simulator.ObjectModel.Actors;
using Waher.Networking.XMPP.Provisioning;
using Waher.Networking.XMPP.Concentrator;

namespace TAG.Simulator.XMPP.IoT.Extensions
{
	/// <summary>
	/// Concentrator Server XMPP extension
	/// </summary>
	public class ConcentratorServerExtension : IoTXmppExtension
	{
		/// <summary>
		/// Concentrator Server XMPP extension
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public ConcentratorServerExtension(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(ConcentratorServerExtension);

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new ConcentratorServerExtension(Parent, Model);
		}

		/// <summary>
		/// Adds the extension to the client.
		/// </summary>
		/// <param name="Instance">Actor instance.</param>
		/// <param name="Client">XMPP Client</param>
		/// <returns>Extension object.</returns>
		public override async Task<object> Add(IActor Instance, Waher.Networking.XMPP.XmppClient Client)
		{
			ConcentratorServer Extension;

			if (!Client.TryGetTag("ProvisioningClient", out object Obj) || !(Obj is ProvisioningClient ProvisioningClient))
				ProvisioningClient = null;

			if (!Client.TryGetTag("ThingRegistryClient", out Obj) || !(Obj is ThingRegistryClient ThingRegistryClient))
				ThingRegistryClient = null;

			Extension = await ConcentratorServer.Create(Client, ThingRegistryClient, ProvisioningClient);

			Client.SetTag("ConcentratorServer", Extension);

			return Extension;
		}

	}
}
