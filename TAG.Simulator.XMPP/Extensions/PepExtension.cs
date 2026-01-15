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
		public override string LocalName => nameof(PepExtension);

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
				return this.Model.ExternalEvent(Instance, "NonPepItemNotification",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client));
			};

			Extension.NonPepItemRetraction += (Sender, e) =>
			 {
				 return this.Model.ExternalEvent(Instance, "NonPepItemRetraction",
					 new KeyValuePair<string, object>("e", e),
					 new KeyValuePair<string, object>("Client", Client));
			 };

			Extension.OnUserActivity += (Sender, e) =>
			{
				return this.Model.ExternalEvent(Instance, "OnUserActivity",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client));
			};

			Extension.OnUserAvatarMetaData += (Sender, e) =>
			{
				return this.Model.ExternalEvent(Instance, "OnUserAvatarMetaData",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client));
			};

			Extension.OnUserLocation += (Sender, e) =>
			{
				return this.Model.ExternalEvent(Instance, "OnUserLocation",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client));
			};

			Extension.OnUserMood += (Sender, e) =>
			{
				return this.Model.ExternalEvent(Instance, "OnUserMood",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client));
			};

			Extension.OnUserTune += (Sender, e) =>
			{
				return this.Model.ExternalEvent(Instance, "OnUserTune",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client));
			};

			return Task.FromResult<object>(Extension);
		}

	}
}
