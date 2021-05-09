using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TAG.Simulator.ObjectModel.Actors;
using Waher.Networking.XMPP.PEP;

namespace TAG.Simulator.XMPP.Extensions
{
	/// <summary>
	/// PEP XMPP extension
	/// </summary>
	public class PepExtension : XmppExtension
	{
		/// <summary>
		/// PEP XMPP extension
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public PepExtension(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "PepExtension";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new PepExtension(Parent, Model);
		}

		/// <summary>
		/// Adds the extension to the client.
		/// </summary>
		/// <param name="Instance">Actor instance.</param>
		/// <param name="Client">XMPP Client</param>
		/// <returns>Extension object.</returns>
		public override Task<object> Add(IActor Instance, Waher.Networking.XMPP.XmppClient Client)
		{
			PepClient Extension = new PepClient(Client);
			Client.SetTag("PEP", Extension);

			Extension.NonPepItemNotification += (Sender, e) =>
			{
				this.Model.ExternalEvent(Instance, "NonPepItemNotification",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client));

				return Task.CompletedTask;
			};

			Extension.NonPepItemRetraction += (Sender, e) =>
			 {
				 this.Model.ExternalEvent(Instance, "NonPepItemRetraction",
					 new KeyValuePair<string, object>("e", e),
					 new KeyValuePair<string, object>("Client", Client));

				 return Task.CompletedTask;
			 };

			Extension.OnUserActivity += (Sender, e) =>
			{
				this.Model.ExternalEvent(Instance, "OnUserActivity",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client));

				return Task.CompletedTask;
			};

			Extension.OnUserAvatarMetaData += (Sender, e) =>
			{
				this.Model.ExternalEvent(Instance, "OnUserAvatarMetaData",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client));

				return Task.CompletedTask;
			};

			Extension.OnUserLocation += (Sender, e) =>
			{
				this.Model.ExternalEvent(Instance, "OnUserLocation",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client));

				return Task.CompletedTask;
			};

			Extension.OnUserMood += (Sender, e) =>
			{
				this.Model.ExternalEvent(Instance, "OnUserMood",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client));

				return Task.CompletedTask;
			};

			Extension.OnUserTune += (Sender, e) =>
			{
				this.Model.ExternalEvent(Instance, "OnUserTune",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client));

				return Task.CompletedTask;
			};

			return Task.FromResult<object>(Extension);
		}

	}
}
