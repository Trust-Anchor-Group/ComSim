using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel.Actors;
using Waher.Content.Xml;
using Waher.Networking.Sniffers;
using Waher.Persistence;
using Waher.Persistence.Filters;
using Waher.Script;
using Waher.Script.Persistence.SQL;

namespace TAG.Simulator.MQ.Actors
{
	/// <summary>
	/// MQ Actor connecting to the MQ network using traditional TCP.
	/// </summary>
	public class MqActorTcp : Actor
	{
		/// <summary>
		/// http://trustanchorgroup.com/Schema/ComSim/MQ.xsd
		/// </summary>
		public const string MqNamespace = "http://trustanchorgroup.com/Schema/ComSim/MQ.xsd";

		/// <summary>
		/// TAG.Simulator.MQ.Schema.ComSimMq.xsd
		/// </summary>
		public const string MqSchema = "TAG.Simulator.MQ.Schema.ComSimMq.xsd";

		private AccountCredentials credentials;
		private MqClient client;
		private ISniffer sniffer;
		private Task connectionTask;
		private string queueManager;
		private string channel;
		private string userName;
		private string password;
		private string cipher;
		private string cipherSuite;
		private string certificateStore;
		private string host;
		private int port;

		/// <summary>
		/// MQ Actor connecting to the MQ network using traditional TCP.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public MqActorTcp(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Abstract base class for MQ actors.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <param name="InstanceIndex">Instance index.</param>
		/// <param name="InstanceId">ID of instance</param>
		public MqActorTcp(ISimulationNode Parent, Model Model, int InstanceIndex, string InstanceId)
			: base(Parent, Model, InstanceIndex, InstanceId)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "MqActorTcp";

		/// <summary>
		/// XML Namespace where the element is defined.
		/// </summary>
		public override string Namespace => MqNamespace;

		/// <summary>
		/// Points to the embedded XML Schema resource defining the semantics of the XML namespace.
		/// </summary>
		public override string SchemaResource => MqSchema;

		/// <summary>
		/// MQ Client
		/// </summary>
		public MqClient Client => this.client;

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new MqActorTcp(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.host = XML.Attribute(Definition, "host");
			this.port = XML.Attribute(Definition, "port", 1414);
			this.queueManager = XML.Attribute(Definition, "queueManager");
			this.channel = XML.Attribute(Definition, "channel");
			this.userName = XML.Attribute(Definition, "userName");
			this.password = XML.Attribute(Definition, "password");
			this.cipher = XML.Attribute(Definition, "cipher");
			this.cipherSuite = XML.Attribute(Definition, "cipherSuite");
			this.certificateStore = XML.Attribute(Definition, "certificateStore");

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
		public override Task<Actor> CreateInstanceAsync(int InstanceIndex, string InstanceId)
		{
			MqActorTcp Result = new MqActorTcp(this, this.Model, InstanceIndex, InstanceId)
			{
				host = this.host,
				port = this.port,
				queueManager = this.queueManager,
				channel = this.channel,
				userName = this.userName + InstanceIndex.ToString(),
				password = this.password,
				cipher = this.cipher,
				cipherSuite = this.cipherSuite,
				certificateStore = this.certificateStore
			};

			return Task.FromResult<Actor>(Result);
		}

		/// <summary>
		/// Initializes an instance of an actor.
		/// </summary>
		public override async Task InitializeInstance()
		{
			this.credentials = await Database.FindFirstIgnoreRest<AccountCredentials>(new FilterAnd(
				new FilterFieldEqualTo("Host", this.host),
				new FilterFieldEqualTo("UserName", this.userName)));

			if (this.credentials is null)
			{
				this.credentials = new AccountCredentials()
				{
					Host = this.host,
					UserName = this.userName,
					Password = string.IsNullOrEmpty(this.password) ? string.Empty : await this.Model.GetKey(this.password, this.userName)
				};
			}

			this.sniffer = this.Model.GetSniffer(this.InstanceId);

			if (this.sniffer is null)
				this.client = new MqClient(this.queueManager, this.channel, this.cipher, this.cipherSuite, this.certificateStore, this.host, this.port);
			else
				this.client = new MqClient(this.queueManager, this.channel, this.cipher, this.cipherSuite, this.certificateStore, this.host, this.port, this.sniffer);

			this.connectionTask = this.client.ConnectAsync(this.credentials.UserName, this.credentials.Password);
		}

		/// <summary>
		/// Starts an instance of an actor.
		/// </summary>
		public override async Task StartInstance()
		{
			await this.connectionTask;

			if (string.IsNullOrEmpty(this.credentials.ObjectId))
				await Database.Insert(this.credentials);

			Variables Variables = new Variables();
			ObjectProperties Properties = new ObjectProperties(this, Variables);
			List<SubscriptionState> Subscriptions = new List<SubscriptionState>();

			if (this.Parent is ISimulationNodeChildren Parent)
			{
				foreach (ISimulationNode Node in Parent.Children)
				{
					if (Node is Subscribe Subscribe)
					{
						SubscriptionState Subscription = new SubscriptionState()
						{
							ExtEventName = await Expression.TransformAsync(Subscribe.ExtEvent, "{", "}", Properties),
							Queue = await Expression.TransformAsync(Subscribe.Queue, "{", "}", Properties),
							Actor = this,
							Model = this.Model,
							Cancel = new ManualResetEvent(false),
							Stopped = new TaskCompletionSource<bool>()
						};

						Subscription.Subscribe(Client);
						Subscriptions.Add(Subscription);
					}
				}
			}

			this.subscriptions = Subscriptions.ToArray();
		}

		private SubscriptionState[] subscriptions;

		private class SubscriptionState : IDisposable
		{
			public string ExtEventName;
			public string Queue;
			public MqActorTcp Actor;
			public Model Model;
			public ManualResetEvent Cancel;
			public TaskCompletionSource<bool> Stopped;

			public void Dispose()
			{
				this.Cancel.Set();
			}

			public void Subscribe(MqClient Client)
			{
				Client.SubscribeIncoming(this.Queue, this.Cancel, this.Stopped, this.MessageReceived, null);
			}

			private Task MessageReceived(object Sender, MqMessageEventArgs e)
			{
				this.Model.ExternalEvent(this.Actor, this.ExtEventName,
					new KeyValuePair<string, object>("Message", e.Message),
					new KeyValuePair<string, object>("Client", this.Actor.client));

				return Task.CompletedTask;
			}
		}

		/// <summary>
		/// Finalizes an instance of an actor.
		/// </summary>
		public override async Task FinalizeInstance()
		{
			foreach (SubscriptionState State in this.subscriptions)
				State.Dispose();

			foreach (SubscriptionState State in this.subscriptions)
				await State.Stopped.Task;

			this.client?.Dispose();
			this.client = null;

			if (!(this.sniffer is null))
			{
				if (this.sniffer is IDisposable Disposable)
					Disposable.Dispose();

				this.sniffer = null;
			}
		}

		/// <summary>
		/// Returns the object that will be used by the actor for actions during an activity.
		/// </summary>
		public override object ActivityObject
		{
			get
			{
				return new MqActivityObject()
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
