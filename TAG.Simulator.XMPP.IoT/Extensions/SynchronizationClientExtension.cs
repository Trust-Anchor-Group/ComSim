using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TAG.Simulator.ObjectModel.Actors;
using Waher.Networking.XMPP.Synchronization;

namespace TAG.Simulator.XMPP.IoT.Extensions
{
	/// <summary>
	/// Synchronization Client XMPP extension
	/// </summary>
	public class SynchronizationClientExtension : IoTXmppExtension
	{
		/// <summary>
		/// Synchronization Client XMPP extension
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public SynchronizationClientExtension(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "SynchronizationClientExtension";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new SynchronizationClientExtension(Parent, Model);
		}

		/// <summary>
		/// Adds the extension to the client.
		/// </summary>
		/// <param name="Instance">Actor instance.</param>
		/// <param name="Client">XMPP Client</param>
		public override Task Add(IActor Instance, Waher.Networking.XMPP.XmppClient Client)
		{
			SynchronizationClient Extension = new SynchronizationClient(Client);
			Client.SetTag("SynchronizationClient", Extension);

			Extension.OnUpdated += (Sender, e) =>
			{
				this.Model.ExternalEvent(Instance, "OnUpdated",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client));
			};

			return Task.CompletedTask;
		}

	}
}
