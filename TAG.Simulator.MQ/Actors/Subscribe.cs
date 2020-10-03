using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel;
using Waher.Content.Xml;

namespace TAG.Simulator.MQ.Actors
{
	/// <summary>
	/// Represents a queue subscription
	/// </summary>
	public class Subscribe : SimulationNode
	{
		private string queue;
		private string _event;
		private string variable;
		private string actorName;

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
		public override string LocalName => "Subscribe";

		/// <summary>
		/// XML Namespace where the element is defined.
		/// </summary>
		public override string Namespace => MqActorTcp.MqNamespace;

		/// <summary>
		/// Points to the embedded XML Schema resource defining the semantics of the XML namespace.
		/// </summary>
		public override string SchemaResource => MqActorTcp.MqSchema;

		/// <summary>
		/// Queue Name
		/// </summary>
		public string Queue => this.queue;

		/// <summary>
		/// Event Name
		/// </summary>
		public string Event => this._event;

		/// <summary>
		/// Message variable name
		/// </summary>
		public string Variable => this.variable;

		/// <summary>
		/// Actor variable name
		/// </summary>
		public string ActorName => this.actorName;

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
			this.queue = XML.Attribute(Definition, "queue");
			this._event = XML.Attribute(Definition, "event");
			this.variable = XML.Attribute(Definition, "variable");
			this.actorName = XML.Attribute(Definition, "actorName");

			return Task.CompletedTask;
		}

	}
}
