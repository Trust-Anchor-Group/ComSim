using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel.Actors;
using Waher.Content.Xml;
using Waher.Networking.Sniffers;
using Waher.Networking.MQTT;
using Waher.Persistence;
using Waher.Persistence.Filters;
using Waher.Script;
using Waher.Script.Persistence.SQL;

namespace TAG.Simulator.MQTT.Actors
{
	/// <summary>
	/// MQTT Actor connecting to the MQTT network using traditional TCP.
	/// </summary>
	public class MqttActorTcp : Actor
	{
		/// <summary>
		/// http://trustanchorgroup.com/Schema/ComSim/MQTT.xsd
		/// </summary>
		public const string MqttNamespace = "http://trustanchorgroup.com/Schema/ComSim/MQTT.xsd";

		/// <summary>
		/// TAG.Simulator.MQTT.Schema.ComSimMqtt.xsd
		/// </summary>
		public const string MqttSchema = "TAG.Simulator.MQTT.Schema.ComSimMqtt.xsd";

		private KeyValuePair<string, MqttQualityOfService>[] subscriptions;
		private AccountCredentials credentials;
		private MqttClient client;
		private ISniffer sniffer;
		private string domain;
		private string userName;
		private string password;
		private int port;
		private bool encrypted;
		private bool trustServer;
		private bool isOnline = false;
		private TaskCompletionSource<bool> connected;

		/// <summary>
		/// MQTT Actor connecting to the MQTT network using traditional TCP.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public MqttActorTcp(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Abstract base class for MQTT actors.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <param name="InstanceIndex">Instance index.</param>
		/// <param name="InstanceId">ID of instance</param>
		public MqttActorTcp(ISimulationNode Parent, Model Model, int InstanceIndex, string InstanceId)
			: base(Parent, Model, InstanceIndex, InstanceId)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "MqttActorTcp";

		/// <summary>
		/// XML Namespace where the element is defined.
		/// </summary>
		public override string Namespace => MqttNamespace;

		/// <summary>
		/// Points to the embedded XML Schema resource defining the semantics of the XML namespace.
		/// </summary>
		public override string SchemaResource => MqttSchema;

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
		/// MQTT Client
		/// </summary>
		public MqttClient Client => this.client;

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new MqttActorTcp(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.domain = XML.Attribute(Definition, "domain");
			this.encrypted = XML.Attribute(Definition, "encrypted", false);
			this.port = XML.Attribute(Definition, "port", this.encrypted ? 8883 : 1883);
			this.userName = XML.Attribute(Definition, "userName");
			this.password = XML.Attribute(Definition, "password");
			this.trustServer = XML.Attribute(Definition, "trustServer", false);

			return base.FromXml(Definition);
		}

		/// <summary>
		/// Creates an instance of the actor.
		/// 
		/// Note: Parent of newly created actor should point to this node (the creator of the instance object).
		/// </summary>
		/// <param name="InstanceIndex">Instance index.</param>
		/// <param name="InstanceId">ID of instance</param>
		/// <returns>Actor instance.</returns>
		public override Actor CreateInstance(int InstanceIndex, string InstanceId)
		{
			MqttActorTcp Result = new MqttActorTcp(this, this.Model, InstanceIndex, InstanceId)
			{
				domain = this.domain,
				encrypted = this.encrypted,
				port = this.port,
				userName = this.userName + InstanceIndex.ToString(),
				password = this.password,
				trustServer = this.trustServer
			};

			Variables Variables = new Variables();
			ObjectProperties Properties = new ObjectProperties(Result, Variables);
			List<KeyValuePair<string, MqttQualityOfService>> Topics = new List<KeyValuePair<string, MqttQualityOfService>>();

			foreach (ISimulationNode Node in this.Children)
			{
				if (Node is Subscribe Subscribe)
				{
					string Topic = Expression.Transform(Subscribe.Topic, "{", "}", Properties);
					Topics.Add(new KeyValuePair<string, MqttQualityOfService>(Topic, Subscribe.QoS));
				}
			}

			Result.subscriptions = Topics.ToArray();

			return Result;
		}

		/// <summary>
		/// Initializes an instance of an actor.
		/// </summary>
		public override async Task InitializeInstance()
		{
			this.credentials = await Database.FindFirstIgnoreRest<AccountCredentials>(new FilterAnd(
				new FilterFieldEqualTo("Domain", this.domain),
				new FilterFieldEqualTo("UserName", this.userName)));

			if (this.credentials is null)
			{
				this.credentials = new AccountCredentials()
				{
					Domain = this.domain,
					UserName = this.userName,
					Password = string.IsNullOrEmpty(this.password) ? string.Empty : await this.Model.GetKey(this.password, this.userName)
				};
			}

			this.sniffer = this.Model.GetSniffer(this.userName);

			if (this.sniffer is null)
				this.client = new MqttClient(this.domain, this.port, this.encrypted, this.userName, this.credentials.Password);
			else
				this.client = new MqttClient(this.domain, this.port, this.encrypted, this.userName, this.credentials.Password, this.sniffer);

			this.client.TrustServer = this.trustServer;

			this.client.OnStateChanged += this.Client_OnStateChanged;
			this.client.OnConnectionError += Client_OnConnectionError;
			this.client.OnError += Client_OnError;
			this.client.OnContentReceived += Client_OnContentReceived;
			this.client.OnPing += Client_OnPing;
			this.client.OnPingResponse += Client_OnPingResponse;
			this.client.OnPublished += Client_OnPublished;
			this.client.OnSubscribed += Client_OnSubscribed;
			this.client.OnUnsubscribed += Client_OnUnsubscribed;

			this.connected = new TaskCompletionSource<bool>();
		}

		private Task Client_OnUnsubscribed(object Sender, ushort PacketIdentifier)
		{
			this.Model.ExternalEvent(this, "OnUnsubscribed",
				new KeyValuePair<string, object>("PacketIdentifier", PacketIdentifier),
				new KeyValuePair<string, object>("Client", this.client));

			return Task.CompletedTask;
		}

		private Task Client_OnSubscribed(object Sender, ushort PacketIdentifier)
		{
			this.Model.ExternalEvent(this, "OnSubscribed",
				new KeyValuePair<string, object>("PacketIdentifier", PacketIdentifier),
				new KeyValuePair<string, object>("Client", this.client));

			return Task.CompletedTask;
		}

		private Task Client_OnPublished(object Sender, ushort PacketIdentifier)
		{
			this.Model.ExternalEvent(this, "OnPublished",
				new KeyValuePair<string, object>("PacketIdentifier", PacketIdentifier),
				new KeyValuePair<string, object>("Client", this.client));

			return Task.CompletedTask;
		}

		private Task Client_OnPingResponse(object Sender, EventArgs e)
		{
			this.Model.ExternalEvent(this, "OnPingResponse",
				new KeyValuePair<string, object>("e", e),
				new KeyValuePair<string, object>("Client", this.client));

			return Task.CompletedTask;
		}

		private Task Client_OnPing(object Sender, EventArgs e)
		{
			this.Model.ExternalEvent(this, "OnPing",
				new KeyValuePair<string, object>("e", e),
				new KeyValuePair<string, object>("Client", this.client));

			return Task.CompletedTask;
		}

		private Task Client_OnContentReceived(object Sender, MqttContent Content)
		{
			this.Model.ExternalEvent(this, "OnContentReceived",
				new KeyValuePair<string, object>("Content", Content),
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

		private Task Client_OnStateChanged(object Sender, MqttState NewState)
		{
			switch (NewState)
			{
				case MqttState.Connected:
					this.isOnline = true;

					if (string.IsNullOrEmpty(this.credentials.ObjectId))
						Database.Insert(this.credentials);

					this.client.SUBSCRIBE(this.subscriptions);

					this.connected?.TrySetResult(true);
					break;

				case MqttState.Error:
				case MqttState.Offline:
					this.isOnline = false;
					this.connected?.TrySetResult(false);
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
			if (!(await this.connected.Task))
				throw new Exception("Unable to connect " + this.userName + "@" + this.domain);

			this.connected = null;
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
		/// Returns the object that will be used by the actor for actions during an activity.
		/// </summary>
		public override object ActivityObject
		{
			get
			{
				return new MqttActivityObject()
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
