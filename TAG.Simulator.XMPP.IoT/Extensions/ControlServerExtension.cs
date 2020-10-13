using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TAG.Simulator.ObjectModel.Actors;
using Waher.Networking.XMPP.Provisioning;
using Waher.Networking.XMPP.Control;
using Waher.Things.ControlParameters;

namespace TAG.Simulator.XMPP.IoT.Extensions
{
	/// <summary>
	/// Control Server XMPP extension
	/// </summary>
	public class ControlServerExtension : IoTXmppExtension
	{
		/// <summary>
		/// Control Server XMPP extension
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public ControlServerExtension(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "ControlServerExtension";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new ControlServerExtension(Parent, Model);
		}

		/// <summary>
		/// Adds the extension to the client.
		/// </summary>
		/// <param name="Instance">Actor instance.</param>
		/// <param name="Client">XMPP Client</param>
		public override Task Add(IActor Instance, Waher.Networking.XMPP.XmppClient Client)
		{
			ControlServer Extension;

			if (Client.TryGetTag("ProvisioningClient", out object Obj) && Obj is ProvisioningClient ProvisioningClient)
				Extension = new ControlServer(Client, ProvisioningClient);
			else
				Extension = new ControlServer(Client);

			Client.SetTag("ControlServer", Extension);

			Extension.OnGetControlParameters += (Node) =>
			{
				this.Model.ExternalEvent(Instance, "OnGetControlParameters",
					new KeyValuePair<string, object>("Node", Node),
					new KeyValuePair<string, object>("Client", Client));

				return Task.FromResult<ControlParameter[]>(new ControlParameter[0]);
			};

			return Task.CompletedTask;
		}

	}
}
