using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel.Actors;
using TAG.Simulator.XMPP.Events;
using Waher.Content.Xml;
using Waher.Networking.DNS;
using Waher.Networking.DNS.ResourceRecords;
using Waher.Networking.Sniffers;
using Waher.Networking.XMPP;
using Waher.Persistence;
using Waher.Persistence.Filters;

namespace TAG.Simulator.XMPP.Actors
{
	/// <summary>
	/// Abstract base class for XMPP actors.
	/// </summary>
	public abstract class XmppActor : Actor
	{
		/// <summary>
		/// http://trustanchorgroup.com/Schema/ComSim/XMPP.xsd
		/// </summary>
		public const string XmppNamespace = "http://trustanchorgroup.com/Schema/ComSim/XMPP.xsd";

		/// <summary>
		/// TAG.Simulator.XMPP.Schema.ComSimXmpp.xsd
		/// </summary>
		public const string XmppSchema = "TAG.Simulator.XMPP.Schema.ComSimXmpp.xsd";

		private XmppClient client;
		private ISniffer sniffer;
		private AccountCredentials credentials;
		private XmppCredentials xmppCredentials;
		private string domain;
		private string host;
		private int? port;
		private string userName;
		private string apiKey;
		private string secret;
		private bool alwaysConnected;
		private bool allowCramMD5;
		private bool allowDigestMD5;
		private bool allowEncryption;
		private bool allowPlain;
		private bool allowScramSHA1;
		private bool allowScramSHA256;
		private bool requestRosterOnStartup;
		private bool trustServer;
		private bool isOnline = false;

		/// <summary>
		/// Abstract base class for XMPP actors.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public XmppActor(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Abstract base class for XMPP actors.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <param name="InstanceIndex">Instance index.</param>
		/// <param name="InstanceId">ID of instance</param>
		public XmppActor(ISimulationNode Parent, Model Model, int InstanceIndex, string InstanceId)
			: base(Parent, Model, InstanceIndex, InstanceId)
		{
		}

		/// <summary>
		/// XML Namespace where the element is defined.
		/// </summary>
		public override string Namespace => XmppNamespace;

		/// <summary>
		/// Points to the embedded XML Schema resource defining the semantics of the XML namespace.
		/// </summary>
		public override string SchemaResource => XmppSchema;

		/// <summary>
		/// If instance is online.
		/// </summary>
		public bool IsOnline => this.isOnline;

		/// <summary>
		/// If server is to be trusted, regardless of state of certificate.
		/// </summary>
		public bool TrustServer => this.trustServer;

		/// <summary>
		/// Domain
		/// </summary>
		public string Domain => this.domain;

		/// <summary>
		/// Host
		/// </summary>
		public string Host => this.host;

		/// <summary>
		/// XMPP Client
		/// </summary>
		public XmppClient Client => this.client;

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.domain = XML.Attribute(Definition, "domain");
			this.userName = XML.Attribute(Definition, "userName");
			this.apiKey = XML.Attribute(Definition, "apiKey");
			this.secret = XML.Attribute(Definition, "secret");
			this.alwaysConnected = XML.Attribute(Definition, "alwaysConnected", false);

			if (Definition.HasAttribute("port"))
				this.port = XML.Attribute(Definition, "port", 0);
			else
				this.port = null;

			this.allowCramMD5 = XML.Attribute(Definition, "allowCramMD5", false);
			this.allowDigestMD5 = XML.Attribute(Definition, "allowDigestMD5", false);
			this.allowEncryption = XML.Attribute(Definition, "allowEncryption", true);
			this.allowPlain = XML.Attribute(Definition, "allowPlain", false);
			this.allowScramSHA1 = XML.Attribute(Definition, "allowScramSHA1", true);
			this.allowScramSHA256 = XML.Attribute(Definition, "allowScramSHA256", true);
			this.requestRosterOnStartup = XML.Attribute(Definition, "requestRosterOnStartup", true);
			this.trustServer = XML.Attribute(Definition, "trustServer", false);

			return base.FromXml(Definition);
		}

		/// <summary>
		/// Initialized the node before simulation.
		/// </summary>
		public override async Task Initialize()
		{
			if (this.port.HasValue)
				this.host = this.domain;
			else
			{
				try
				{
					SRV SRV = await DnsResolver.LookupServiceEndpoint(this.domain, "xmpp-client", "tcp");
					if (!(SRV is null) && !string.IsNullOrEmpty(SRV.TargetHost) && SRV.Port > 0)
					{
						this.host = SRV.TargetHost;
						this.port = SRV.Port;
					}
				}
				catch (Exception)
				{
					this.host = this.domain;
					this.port = 5222;
				}
			}

			await base.Initialize();
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
			XmppActor Result = this.CreateInstanceObject(InstanceIndex, InstanceId);

			Result.domain = this.domain;
			Result.host = this.host;
			Result.port = this.port;
			Result.userName = this.userName + InstanceIndex.ToString();
			Result.apiKey = this.apiKey;
			Result.secret = this.secret;
			Result.alwaysConnected = this.alwaysConnected;
			Result.allowCramMD5 = this.allowCramMD5;
			Result.allowDigestMD5 = this.allowDigestMD5;
			Result.allowEncryption = this.allowEncryption;
			Result.allowPlain = this.allowPlain;
			Result.allowScramSHA1 = this.allowScramSHA1;
			Result.allowScramSHA256 = this.allowScramSHA256;
			Result.requestRosterOnStartup = this.requestRosterOnStartup;
			Result.trustServer = this.trustServer;

			return Task.FromResult<Actor>(Result);
		}

		/// <summary>
		/// Creates an instance object of the XMPP actor, and initializes it.
		/// 
		/// Note: Parent of newly created actor should point to this node (the creator of the instance object).
		/// </summary>
		/// <param name="InstanceIndex">Instance index.</param>
		/// <param name="InstanceId">ID of instance</param>
		/// <returns>Actor instance.</returns>
		protected abstract XmppActor CreateInstanceObject(int InstanceIndex, string InstanceId);

		/// <summary>
		/// Initializes an instance of an actor.
		/// </summary>
		public override async Task InitializeInstance()
		{
			this.xmppCredentials = await this.GetInstanceCredentials();

			this.sniffer = this.Model.GetSniffer(this.userName);

			if (this.sniffer is null)
				this.client = new XmppClient(this.xmppCredentials, "en", typeof(XmppActor).GetTypeInfo().Assembly);
			else
				this.client = new XmppClient(this.xmppCredentials, "en", typeof(XmppActor).GetTypeInfo().Assembly, this.sniffer);

			this.client.OnStateChanged += this.Client_OnStateChanged;
			this.client.OnChatMessage += Client_OnChatMessage;
			this.client.OnConnectionError += Client_OnConnectionError;
			this.client.OnError += Client_OnError;
			this.client.OnErrorMessage += Client_OnErrorMessage;
			this.client.OnGroupChatMessage += Client_OnGroupChatMessage;
			this.client.OnHeadlineMessage += Client_OnHeadlineMessage;
			this.client.OnNormalMessage += Client_OnNormalMessage;
			this.client.OnPresence += Client_OnPresence;
			this.client.OnPresenceSubscribe += Client_OnPresenceSubscribe;
			this.client.OnRosterItemAdded += Client_OnRosterItemAdded;
			this.client.OnRosterItemRemoved += Client_OnRosterItemRemoved;
			this.client.OnRosterItemUpdated += Client_OnRosterItemUpdated;
			this.client.CustomPresenceXml += Client_CustomPresenceXml;

			string InstanceIndexSuffix = this.InstanceIndex.ToString();
			int c = this.N.ToString().Length;
			int Nr0 = c - InstanceIndexSuffix.Length;

			if (Nr0 > 0)
				InstanceIndexSuffix = new string('0', Nr0) + InstanceIndexSuffix;

			if (this.Parent is IActor ParentActor)
			{
				foreach (ISimulationNode Node in ParentActor.Children)
				{
					if (Node is HandlerNode HandlerNode)
						HandlerNode.RegisterHandlers(this, this.client);

					if (Node is Extensions.IXmppExtension Extension)
					{
						object ExtensionObj = await Extension.Add(this, Client);

						if (!string.IsNullOrEmpty(Extension.Id))
							this.Model.Variables[Extension.Id + InstanceIndexSuffix] = ExtensionObj;
					}
				}
			}

			if (this.alwaysConnected)
			{
				this.client.Connect(this.domain);

				if (this.xmppCredentials.AllowRegistration)
				{
					switch (await this.client.WaitStateAsync(30000, XmppState.Connected, XmppState.Error, XmppState.Offline))
					{
						case 0: // Connected
							break;

						case 1: // Error
						case 2: // Offline
						default:
							throw new Exception("Unable to create account for " + this.userName + "@" + this.domain);
					}
				}
			}
		}

		private Task Client_CustomPresenceXml(object Sender, CustomPresenceEventArgs e)
		{
			if (this.client.TryGetTag("CutomPresenceXML", out object Obj) &&
				Obj is string Xml)
			{
				e.Stanza.Append(Xml);
			}

			return Task.CompletedTask;
		}

		private Task Client_OnRosterItemUpdated(object Sender, RosterItem Item)
		{
			this.Model.ExternalEvent(this, "RosterItemUpdated",
				new KeyValuePair<string, object>("Item", Item),
				new KeyValuePair<string, object>("Client", this.client));

			return Task.CompletedTask;
		}

		private Task Client_OnRosterItemRemoved(object Sender, RosterItem Item)
		{
			this.Model.ExternalEvent(this, "RosterItemRemoved",
				new KeyValuePair<string, object>("Item", Item),
				new KeyValuePair<string, object>("Client", this.client));

			return Task.CompletedTask;
		}

		private Task Client_OnRosterItemAdded(object Sender, RosterItem Item)
		{
			this.Model.ExternalEvent(this, "RosterItemAdded",
				new KeyValuePair<string, object>("Item", Item),
				new KeyValuePair<string, object>("Client", this.client));

			return Task.CompletedTask;
		}

		private Task Client_OnPresenceSubscribe(object Sender, PresenceEventArgs e)
		{
			if (!this.Model.ExternalEvent(this, "PresenceSubscribe",
				new KeyValuePair<string, object>("e", e),
				new KeyValuePair<string, object>("Client", this.client)))
			{
				e.Accept();
			}

			return Task.CompletedTask;
		}

		private Task Client_OnPresence(object Sender, PresenceEventArgs e)
		{
			this.Model.ExternalEvent(this, "Presence",
				new KeyValuePair<string, object>("e", e),
				new KeyValuePair<string, object>("Client", this.client));

			return Task.CompletedTask;
		}

		private Task Client_OnNormalMessage(object Sender, MessageEventArgs e)
		{
			this.Model.ExternalEvent(this, "NormalMessage",
				new KeyValuePair<string, object>("e", e),
				new KeyValuePair<string, object>("Client", this.client));

			return Task.CompletedTask;
		}

		private Task Client_OnHeadlineMessage(object Sender, MessageEventArgs e)
		{
			this.Model.ExternalEvent(this, "HeadlineMessage",
				new KeyValuePair<string, object>("e", e),
				new KeyValuePair<string, object>("Client", this.client));

			return Task.CompletedTask;
		}

		private Task Client_OnGroupChatMessage(object Sender, MessageEventArgs e)
		{
			this.Model.ExternalEvent(this, "GroupChatMessage",
				new KeyValuePair<string, object>("e", e),
				new KeyValuePair<string, object>("Client", this.client));

			return Task.CompletedTask;
		}

		private Task Client_OnErrorMessage(object Sender, MessageEventArgs e)
		{
			this.Model.ExternalEvent(this, "ErrorMessage",
				new KeyValuePair<string, object>("e", e),
				new KeyValuePair<string, object>("Client", this.client));

			return Task.CompletedTask;
		}

		private Task Client_OnError(object Sender, Exception Exception)
		{
			this.Model.ExternalEvent(this, "Error",
				new KeyValuePair<string, object>("Exception", Exception),
				new KeyValuePair<string, object>("Client", this.client));

			return Task.CompletedTask;
		}

		private Task Client_OnConnectionError(object Sender, Exception Exception)
		{
			this.Model.ExternalEvent(this, "ConnectionError",
				new KeyValuePair<string, object>("Exception", Exception),
				new KeyValuePair<string, object>("Client", this.client));

			return Task.CompletedTask;
		}

		private Task Client_OnChatMessage(object Sender, MessageEventArgs e)
		{
			this.Model.ExternalEvent(this, "ChatMessage",
				new KeyValuePair<string, object>("e", e),
				new KeyValuePair<string, object>("Client", this.client));

			return Task.CompletedTask;
		}

		private Task Client_OnStateChanged(object Sender, XmppState NewState)
		{
			switch (NewState)
			{
				case XmppState.Connected:
					this.isOnline = true;

					if (this.credentials is null)
					{
						this.credentials = new AccountCredentials()
						{
							Domain = this.domain,
							UserName = this.userName,
							PasswordHash = this.client.PasswordHash,
							PasswordHashMethod = this.client.PasswordHashMethod
						};

						Database.Insert(this.credentials);
					}
					break;

				case XmppState.Error:
				case XmppState.Offline:
					this.isOnline = false;
					break;
			}

			this.Model.ExternalEvent(this, "OnStateChanged",
				new KeyValuePair<string, object>("NewState", NewState),
				new KeyValuePair<string, object>("Client", this.client));

			return Task.CompletedTask;
		}

		/// <summary>
		/// Starts an instance of an actor.
		/// </summary>
		public override async Task StartInstance()
		{
			if (this.alwaysConnected && !this.xmppCredentials.AllowRegistration)
			{
				switch (await this.client.WaitStateAsync(30000, XmppState.Connected, XmppState.Error, XmppState.Offline))
				{
					case 0: // Connected
						break;

					case 1: // Error
					case 2: // Offline
					default:
						throw new Exception("Unable to connect " + this.userName + "@" + this.domain);
				}
			}
		}

		/// <summary>
		/// Finalizes an instance of an actor.
		/// </summary>
		public override Task FinalizeInstance()
		{
			this.client?.Dispose();
			this.client = null;

			if (!(this.sniffer is null))
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
		protected async virtual Task<XmppCredentials> GetInstanceCredentials()
		{
			this.credentials = await Database.FindFirstIgnoreRest<AccountCredentials>(new FilterAnd(
				new FilterFieldEqualTo("Domain", this.domain),
				new FilterFieldEqualTo("UserName", this.userName)));

			if (!(this.credentials is null) && string.IsNullOrEmpty(this.credentials.PasswordHash))
			{
				await Database.Delete(this.credentials);
				this.credentials = null;
			}

			XmppCredentials Result = new XmppCredentials()
			{
				Account = this.userName,
				AllowCramMD5 = this.allowCramMD5,
				AllowDigestMD5 = this.allowDigestMD5,
				AllowEncryption = this.allowEncryption,
				AllowPlain = this.allowPlain,
				AllowRegistration = false,
				AllowScramSHA1 = this.allowScramSHA1,
				AllowScramSHA256 = this.allowScramSHA256,
				Host = this.host,
				Port = this.port.Value,
				RequestRosterOnStartup = this.requestRosterOnStartup,
				TrustServer = this.trustServer
			};

			if (this.credentials is null)
			{
				Result.AllowRegistration = true;
				Result.FormSignatureKey = await this.Model.GetKey(this.apiKey, string.Empty);
				Result.FormSignatureSecret = await this.Model.GetKey(this.secret, string.Empty);
				Result.Password = Convert.ToBase64String(this.Model.GetRandomBytes(32));
			}
			else
			{
				Result.Password = this.credentials.PasswordHash;
				Result.PasswordType = this.credentials.PasswordHashMethod;
			}

			return Result;
		}

		/// <summary>
		/// Returns the object that will be used by the actor for actions during an activity.
		/// </summary>
		public override object ActivityObject
		{
			get
			{
				return new XmppActivityObject()
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
