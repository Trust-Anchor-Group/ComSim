using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TAG.Simulator.ObjectModel.Actors;
using Waher.Networking.XMPP.Avatar;

namespace TAG.Simulator.XMPP.Extensions
{
	/// <summary>
	/// Avatar XMPP extension
	/// </summary>
	public class AvatarExtension : XmppExtension
	{
		/// <summary>
		/// Avatar XMPP extension
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public AvatarExtension(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "AvatarExtension";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new AvatarExtension(Parent, Model);
		}

		/// <summary>
		/// Adds the extension to the client.
		/// </summary>
		/// <param name="Instance">Actor instance.</param>
		/// <param name="Client">XMPP Client</param>
		public override void Add(IActor Instance, Waher.Networking.XMPP.XmppClient Client)
		{
			AvatarClient Extension = new AvatarClient(Client);
			Client.SetTag("Avatar", Extension);

			Extension.AvatarAdded += (Sender, e) =>
			{
				this.Model.ExternalEvent(Instance, "AvatarAdded",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client));

				return Task.CompletedTask;
			};

			Extension.AvatarRemoved += (Sender, e) =>
			{
				this.Model.ExternalEvent(Instance, "AvatarRemoved",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client));

				return Task.CompletedTask;
			};

			Extension.AvatarUpdated += (Sender, e) =>
			{
				this.Model.ExternalEvent(Instance, "AvatarUpdated",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client));

				return Task.CompletedTask;
			};

			Extension.VCardReceived += (Sender, e) =>
			{
				this.Model.ExternalEvent(Instance, "VCardReceived",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client));

				return Task.CompletedTask;
			};
		}

	}
}
