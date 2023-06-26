using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel;
using Waher.Content.Xml;
using Waher.Networking.MQTT;

namespace TAG.Simulator.MQTT.Actors
{
	/// <summary>
	/// Represents a topic subscription
	/// </summary>
	public class Subscribe : SimulationNode
	{
		private MqttQualityOfService qos;
		private string topic;

		/// <summary>
		/// Represents a topic subscription
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public Subscribe(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(Subscribe);

		/// <summary>
		/// XML Namespace where the element is defined.
		/// </summary>
		public override string Namespace => MqttActorTcp.MqttNamespace;

		/// <summary>
		/// Points to the embedded XML Schema resource defining the semantics of the XML namespace.
		/// </summary>
		public override string SchemaResource => MqttActorTcp.MqttSchema;

		/// <summary>
		/// Topic
		/// </summary>
		public string Topic => this.topic;

		/// <summary>
		/// Quality of Service
		/// </summary>
		public MqttQualityOfService QoS => this.qos;

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new Subscribe(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.topic = XML.Attribute(Definition, "topic");
			this.qos = (MqttQualityOfService)XML.Attribute(Definition, "qos", MqttQualityOfService.AtMostOnce);

			return Task.CompletedTask;
		}
	}
}
