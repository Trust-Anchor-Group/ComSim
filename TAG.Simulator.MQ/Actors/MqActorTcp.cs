using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using IBM.WMQ;
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

		private MqClient client;
		private ISniffer sniffer;
		private string queueManager;
		private string channel;
		private string queue;
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
			this.queue = XML.Attribute(Definition, "queue");
			this.cipher = XML.Attribute(Definition, "cipher", "TLS_RSA_WITH_AES_128_CBC_SHA256");
			this.cipherSuite = XML.Attribute(Definition, "cipherSuite", "SSL_RSA_WITH_AES_128_CBC_SHA256");
			this.certificateStore = XML.Attribute(Definition, "certificateStore", "*USER");

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
			MqActorTcp Result = new MqActorTcp(this, this.Model, InstanceIndex, InstanceId)
			{
				host = this.host,
				port = this.port,
				queueManager = this.queueManager,
				channel = this.channel,
				queue = this.queue + InstanceIndex.ToString(),
				cipher = this.cipher,
				cipherSuite = this.cipherSuite,
				certificateStore = this.certificateStore
			};

			return Result;
		}

		/// <summary>
		/// Initializes an instance of an actor.
		/// </summary>
		public override Task InitializeInstance()
		{
			this.sniffer = this.Model.GetSniffer(this.queue);

			if (this.sniffer is null)
				this.client = new MqClient(this.queueManager, this.channel, this.cipher, this.cipherSuite, this.certificateStore, this.host, this.port);
			else
				this.client = new MqClient(this.queueManager, this.channel, this.cipher, this.cipherSuite, this.certificateStore, this.host, this.port, this.sniffer);

			return Task.CompletedTask;
		}

		/// <summary>
		/// Starts an instance of an actor.
		/// </summary>
		public override Task StartInstance()
		{
			this.client.Connect("USER", "PASSWORD");
			return Task.CompletedTask;
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
				return this;
			}
		}

	}
}
