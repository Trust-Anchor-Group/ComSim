using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TAG.Simulator.ObjectModel.Actors;
using Waher.Networking.XMPP.Provisioning;
using Waher.Networking.XMPP.Sensor;
using Waher.Things;

namespace TAG.Simulator.XMPP.IoT.Extensions
{
	/// <summary>
	/// Sensor Server XMPP extension
	/// </summary>
	public class SensorServerExtension : IoTXmppExtension
	{
		/// <summary>
		/// Sensor Server XMPP extension
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public SensorServerExtension(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "SensorServerExtension";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new SensorServerExtension(Parent, Model);
		}

		/// <summary>
		/// Adds the extension to the client.
		/// </summary>
		/// <param name="Instance">Actor instance.</param>
		/// <param name="Client">XMPP Client</param>
		/// <returns>Extension object.</returns>
		public override Task<object> Add(IActor Instance, Waher.Networking.XMPP.XmppClient Client)
		{
			SensorServer Extension;

			if (Client.TryGetTag("ProvisioningClient", out object Obj) && Obj is ProvisioningClient ProvisioningClient)
				Extension = new SensorServer(Client, ProvisioningClient, true);
			else
				Extension = new SensorServer(Client, true);

			Client.SetTag("SensorServer", Extension);

			Extension.OnExecuteReadoutRequest += (Sender, e) =>
			{
				if (!this.Model.ExternalEvent(Instance, "OnExecuteReadoutRequest",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client)))
				{
					e.ReportErrors(true, new ThingError(ThingReference.Empty, "No event handler registered on sensor."));
				}

				return Task.CompletedTask;
			};
			
			return Task.FromResult<object>(Extension);
		}

	}
}
