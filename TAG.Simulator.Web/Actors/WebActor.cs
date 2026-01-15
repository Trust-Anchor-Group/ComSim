using System;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel.Actors;
using Waher.Content.Xml;
using Waher.Networking.Sniffers;
using Waher.Persistence;
using Waher.Persistence.Filters;

namespace TAG.Simulator.Web.Actors
{
	/// <summary>
	/// Represents a web actor.
	/// </summary>
	public class WebActor : Actor
	{
		/// <summary>
		/// http://lab.tagroot.io/Schema/ComSim/Web.xsd
		/// </summary>
		public const string WebNamespace = "http://lab.tagroot.io/Schema/ComSim/Web.xsd";

		/// <summary>
		/// TAG.Simulator.Web.Schema.ComSimWeb.xsd
		/// </summary>
		public const string WebSchema = "TAG.Simulator.Web.Schema.ComSimWeb.xsd";

		private CookieWebClient client;
		private AccountCredentials credentials;
		private Version protocolVersion;
		private ISniffer sniffer;
		private string loginUrl;
		private string userName;
		private string password;

		/// <summary>
		/// Represents a web actor.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public WebActor(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Represents a web actor.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <param name="InstanceIndex">Instance index.</param>
		/// <param name="InstanceId">ID of instance</param>
		public WebActor(ISimulationNode Parent, Model Model, int InstanceIndex, string InstanceId)
			: base(Parent, Model, InstanceIndex, InstanceId)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(WebActor);

		/// <summary>
		/// XML Namespace where the element is defined.
		/// </summary>
		public override string Namespace => WebNamespace;

		/// <summary>
		/// Points to the embedded XML Schema resource defining the semantics of the XML namespace.
		/// </summary>
		public override string SchemaResource => WebSchema;

		/// <summary>
		/// HTTP Client
		/// </summary>
		public CookieWebClient Client => this.client;

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.loginUrl = XML.Attribute(Definition, "loginUrl");
			this.userName = XML.Attribute(Definition, "userName");
			this.password = XML.Attribute(Definition, "password");

			this.protocolVersion = XML.Attribute(Definition, "httpVersion") switch
			{
				"1.0" => HttpVersion.Version10,
				"1.1" => HttpVersion.Version11,
				"2.0" => HttpVersion.Version20,
				"3.0" => HttpVersion.Version30,
				_ => null,
			};

			return base.FromXml(Definition);
		}

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new WebActor(Parent, Model);
		}

		/// <summary>
		/// Creates an instance of the actor.
		/// 
		/// Note: Parent of newly created actor should point to this node (the creator of the instance object).
		/// </summary>
		/// <param name="InstanceIndex">Instance index.</param>
		/// <param name="InstanceId">ID of instance</param>
		/// <returns>Actor instance.</returns>
		public override Task<Actor> CreateInstanceAsync(int InstanceIndex, string InstanceId)
		{
			WebActor Result = new(this, this.Model, InstanceIndex, InstanceId)
			{
				protocolVersion = this.protocolVersion,
				userName = this.userName,
				password = this.password
			};

			return Task.FromResult<Actor>(Result);
		}

		/// <summary>
		/// Initializes an instance of an actor.
		/// </summary>
		public override async Task InitializeInstance()
		{
			this.credentials = await this.GetInstanceCredentials();

			this.sniffer = this.Model.GetSniffer(this.Id);

			if (this.sniffer is null)
			{
				this.client = new CookieWebClient(this.protocolVersion,
					this.credentials?.UserName,
					this.credentials?.Password);
			}
			else
			{
				this.client = new CookieWebClient(this.protocolVersion,
					this.credentials?.UserName,
					this.credentials?.Password,
					this.sniffer);
			}
		}

		/// <summary>
		/// Finalizes an instance of an actor.
		/// </summary>
		public override Task FinalizeInstance()
		{
			if (this.client is not null)
			{
				this.client.Dispose();
				this.client = null;
			}

			if (this.sniffer is not null)
			{
				if (this.sniffer is IDisposable Disposable)
					Disposable.Dispose();

				this.sniffer = null;
			}

			return Task.CompletedTask;
		}

		/// <summary>
		/// Gets XMPP credentials for the instance.
		/// </summary>
		/// <returns>XMPP Credentials</returns>
		protected async virtual Task<AccountCredentials> GetInstanceCredentials()
		{
			if (string.IsNullOrEmpty(this.loginUrl))
				return null;

			AccountCredentials Credentials = await Database.FindFirstIgnoreRest<AccountCredentials>(new FilterAnd(
				new FilterFieldEqualTo("Url", this.loginUrl),
				new FilterFieldEqualTo("UserName", this.userName)));

			if (Credentials is not null)
				return Credentials;

			Credentials = new AccountCredentials()
			{
				Url = this.loginUrl,
				UserName = await this.Model.GetKey(this.userName, string.Empty),
				Password = await this.Model.GetKey(this.password, string.Empty)
			};

			return Credentials;
		}

		/// <summary>
		/// Starts an instance of an actor.
		/// </summary>
		public override Task StartInstance()
		{
			return Task.CompletedTask;
		}

		/// <summary>
		/// Returns the object that will be used by the actor for actions during an activity.
		/// </summary>
		public override object ActivityObject
		{
			get
			{
				return new WebActivityObject()
				{
					Client = this.client,
					UserName = this.userName,
					InstanceId = this.InstanceId,
					InstanceIndex = this.InstanceIndex
				};
			}
		}

	}
}
