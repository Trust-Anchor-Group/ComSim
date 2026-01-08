using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel.Actors;
using TAG.Simulator.XMPP.Extensions;
using Waher.Content.Xml;
using Waher.Networking.XMPP.Contracts;

namespace TAG.Simulator.XMPP.Legal.Extensions
{
	/// <summary>
	/// Legal XMPP extension
	/// </summary>
	public class LegalExtension : XmppExtension
	{
		/// <summary>
		/// http://lab.tagroot.io/Schema/ComSim/XMPP.xsd
		/// </summary>
		public const string XmppNamespace = "http://lab.tagroot.io/Schema/ComSim/XMPPLegal.xsd";

		/// <summary>
		/// TAG.Simulator.XMPP.Schema.ComSimXmpp.xsd
		/// </summary>
		public const string XmppSchema = "TAG.Simulator.XMPP.Legal.Schema.ComSimXmppLegal.xsd";

		private string componentAddress;

		/// <summary>
		/// Legal XMPP extension
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public LegalExtension(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(LegalExtension);

		/// <summary>
		/// Points to the embedded XML Schema resource defining the semantics of the XML namespace.
		/// </summary>
		public override string SchemaResource => XmppSchema;

		/// <summary>
		/// XML Namespace where the element is defined.
		/// </summary>
		public override string Namespace => XmppNamespace;

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new LegalExtension(Parent, Model);
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
		public override async Task<object> Add(IActor Instance, Waher.Networking.XMPP.XmppClient Client)
		{
			ContractsClient Extension = new ContractsClient(Client, this.componentAddress);
			await Extension.LoadKeys(true);

			Client.SetTag("ContractsClient", Extension);

			Extension.ContractCreated += async (Sender, e) =>
			{
				await this.Model.ExternalEvent(Instance, "ContractCreated",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client),
					new KeyValuePair<string, object>("Legal", Extension));
			};

			Extension.ContractDeleted += async (Sender, e) =>
			{
				await this.Model.ExternalEvent(Instance, "ContractDeleted",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client),
					new KeyValuePair<string, object>("Legal", Extension));
			};

			Extension.ContractSigned += async (Sender, e) =>
			{
				await this.Model.ExternalEvent(Instance, "ContractSigned",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client),
					new KeyValuePair<string, object>("Legal", Extension));
			};

			Extension.ContractUpdated += async (Sender, e) =>
			{
				await this.Model.ExternalEvent(Instance, "ContractUpdated",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client),
					new KeyValuePair<string, object>("Legal", Extension));
			};

			Extension.IdentityUpdated += async (Sender, e) =>
			{
				await this.Model.ExternalEvent(Instance, "IdentityUpdated",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client),
					new KeyValuePair<string, object>("Legal", Extension));
			};

			Extension.PetitionForContractReceived += async (Sender, e) =>
			{
				await this.Model.ExternalEvent(Instance, "PetitionForContractReceived",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client),
					new KeyValuePair<string, object>("Legal", Extension));
			};

			Extension.PetitionedContractResponseReceived += async (Sender, e) =>
			{
				await this.Model.ExternalEvent(Instance, "PetitionedContractResponseReceived",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client),
					new KeyValuePair<string, object>("Legal", Extension));
			};

			Extension.PetitionForIdentityReceived += async (Sender, e) =>
			{
				await this.Model.ExternalEvent(Instance, "PetitionForIdentityReceived",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client),
					new KeyValuePair<string, object>("Legal", Extension));
			};

			Extension.PetitionedIdentityResponseReceived += async (Sender, e) =>
			{
				await this.Model.ExternalEvent(Instance, "PetitionedIdentityResponseReceived",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client),
					new KeyValuePair<string, object>("Legal", Extension));
			};

			Extension.PetitionForSignatureReceived += async (Sender, e) =>
			{
				await this.Model.ExternalEvent(Instance, "PetitionForSignatureReceived",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client),
					new KeyValuePair<string, object>("Legal", Extension));
			};

			Extension.PetitionedSignatureResponseReceived += async (Sender, e) =>
			{
				await this.Model.ExternalEvent(Instance, "PetitionedSignatureResponseReceived",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client),
					new KeyValuePair<string, object>("Legal", Extension));
			};

			Extension.PetitionForPeerReviewIDReceived += async (Sender, e) =>
			{
				await this.Model.ExternalEvent(Instance, "PetitionForPeerReviewIDReceived",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client),
					new KeyValuePair<string, object>("Legal", Extension));
			};

			Extension.PetitionedPeerReviewIDResponseReceived += async (Sender, e) =>
			{
				await this.Model.ExternalEvent(Instance, "PetitionedPeerReviewIDResponseReceived",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client),
					new KeyValuePair<string, object>("Legal", Extension));
			};

			Extension.IdentityReview += async (Sender, e) =>
			{
				await this.Model.ExternalEvent(Instance, "IdentityReview",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client),
					new KeyValuePair<string, object>("Legal", Extension));
			};

			Extension.ClientMessage += async (Sender, e) =>
			{
				await this.Model.ExternalEvent(Instance, "ClientMessage",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client),
					new KeyValuePair<string, object>("Legal", Extension));
			};

			Extension.ContractProposalReceived += async (Sender, e) =>
			{
				await this.Model.ExternalEvent(Instance, "ContractProposalReceived",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client),
					new KeyValuePair<string, object>("Legal", Extension));
			};

			Extension.PetitionClientUrlReceived += async (Sender, e) =>
			{
				await this.Model.ExternalEvent(Instance, "PetitionClientUrlReceived",
					new KeyValuePair<string, object>("e", e),
					new KeyValuePair<string, object>("Client", Client),
					new KeyValuePair<string, object>("Legal", Extension));
			};

			return Extension;
		}
	}
}
