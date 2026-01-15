using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel.Actors;
using Waher.Content.Xml;
using Waher.Networking.XMPP.MUC;

namespace TAG.Simulator.XMPP.Extensions
{
	/// <summary>
	/// Multi-User Chat XMPP extension
	/// </summary>
	public class MucExtension : XmppExtension
	{
		private string componentAddress;

		/// <summary>
		/// Multi-User Chat XMPP extension
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public MucExtension(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(MucExtension);

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new MucExtension(Parent, Model);
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
			MultiUserChatClient Extension = new MultiUserChatClient(Client, this.componentAddress);
			Client.SetTag("MUC", Extension);

			Extension.OccupantPresence += (Sender, e) =>
			{
				return this.Model.ExternalEvent(Instance, "OccupantPresence",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client));
			};

			Extension.OccupantRequest += (Sender, e) =>
			{
				return this.Model.ExternalEvent(Instance, "OccupantRequest",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client));
			};

			Extension.PrivateMessageReceived += (Sender, e) =>
			{
				return this.Model.ExternalEvent(Instance, "PrivateMessageReceived",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client));
			};

			Extension.RegistrationRequest += (Sender, e) =>
			{
				return this.Model.ExternalEvent(Instance, "RegistrationRequest",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client));
			};

			Extension.RoomDeclinedInvitationReceived += (Sender, e) =>
			{
				return this.Model.ExternalEvent(Instance, "RoomDeclinedInvitationReceived",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client));
			};

			Extension.RoomDestroyed += (Sender, e) =>
			{
				return this.Model.ExternalEvent(Instance, "RoomDestroyed",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client));
			};

			Extension.RoomInvitationReceived += (Sender, e) =>
			{
				return this.Model.ExternalEvent(Instance, "RoomInvitationReceived",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client));
			};

			Extension.RoomMessage += (Sender, e) =>
			{
				return this.Model.ExternalEvent(Instance, "RoomMessage",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client));
			};

			Extension.RoomOccupantMessage += (Sender, e) =>
			{
				return this.Model.ExternalEvent(Instance, "RoomOccupantMessage",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client));
			};

			Extension.RoomPresence += (Sender, e) =>
			{
				return this.Model.ExternalEvent(Instance, "RoomPresence",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client));
			};

			Extension.RoomSubject += (Sender, e) =>
			{
				return this.Model.ExternalEvent(Instance, "RoomSubject",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client));
			};

			return Task.FromResult<object>(Extension);
		}

	}
}
