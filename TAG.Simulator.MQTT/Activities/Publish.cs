using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.Extensions;
using TAG.Simulator.MQTT.Actors;
using TAG.Simulator.ObjectModel;
using TAG.Simulator.ObjectModel.Activities;
using TAG.Simulator.ObjectModel.Values;
using Waher.Content;
using Waher.Content.Xml;
using Waher.Networking.MQTT;
using Waher.Script;

namespace TAG.Simulator.MQTT.Activities
{
	/// <summary>
	/// Publishes content to a topic.
	/// </summary>
	public class Publish : ActivityNode, IValueRecipient
	{
		private IValue value;
		private MqttQualityOfService qos;
		private StringAttribute actor;
		private StringAttribute topic;
		private bool retain;

		/// <summary>
		/// Publishes content to a topic.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public Publish(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(Publish);

		/// <summary>
		/// Points to the embedded XML Schema resource defining the semantics of the XML namespace.
		/// </summary>
		public override string SchemaResource => MqttActorTcp.MqttSchema;

		/// <summary>
		/// XML Namespace where the element is defined.
		/// </summary>
		public override string Namespace => MqttActorTcp.MqttNamespace;

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new Publish(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.actor = new StringAttribute(XML.Attribute(Definition, "actor"));
			this.topic = new StringAttribute(XML.Attribute(Definition, "topic"));
			this.qos = (MqttQualityOfService)XML.Attribute(Definition, "qos", MqttQualityOfService.AtMostOnce);
			this.retain = XML.Attribute(Definition, "retain", false);

			return base.FromXml(Definition);
		}

		/// <summary>
		/// Registers a value for the argument.
		/// </summary>
		/// <param name="Value">Value node</param>
		public void Register(IValue Value)
		{
			if (this.value is null)
				this.value = Value;
			else
				throw new Exception("Value already registered.");
		}

		/// <summary>
		/// Executes a node.
		/// </summary>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>Next node of execution, if different from the default, otherwise null (for default).</returns>
		public override async Task<LinkedListNode<IActivityNode>> Execute(Variables Variables)
		{
			string Topic = await this.topic.GetValueAsync(Variables);
			object Content = this.value is null ? string.Empty : await this.value.EvaluateAsync(Variables) ?? string.Empty;

			if (!(Content is byte[] Bin))
			{
				ContentResponse Content2 = await InternetContent.EncodeAsync(Content, Encoding.UTF8);
				Content2.AssertOk();

				Bin = Content2.Encoded;
			}

			if (!(await this.GetActorObjectAsync(this.actor, Variables) is MqttActivityObject MqttActor))
				throw new Exception("Actor not an MQTT actor.");

			await MqttActor.Client.PUBLISH(Topic, this.qos, this.retain, Bin);

			return null;
		}

		/// <summary>
		/// Exports PlantUML
		/// </summary>
		/// <param name="Output">Output</param>
		/// <param name="Indentation">Number of tabs to indent.</param>
		/// <param name="QuoteChar">Quote character.</param>
		public override void ExportPlantUml(StreamWriter Output, int Indentation, char QuoteChar)
		{
			bool First = true;

			base.ExportPlantUml(Output, Indentation, QuoteChar);

			Output.Indent(Indentation);
			Output.Write(':');
			Output.Write(this.actor.Value);
			Output.Write(".Publish");
			Output.Write('(');

			Indentation++;

			Output.AppendUmlArgument(Indentation, "Topic", this.topic.Value, true, QuoteChar, ref First);
			Output.AppendUmlArgument(Indentation, "QoS", this.qos.ToString(), false, QuoteChar, ref First);

			if (!this.retain)
				Output.AppendUmlArgument(Indentation, "Retain", "true", false, QuoteChar, ref First);

			if (!(this.value is null))
			{
				if (this.value is Xml Xml && !string.IsNullOrEmpty(Xml.RootName))
					Output.AppendUmlArgument(Indentation, "Content", Xml.RootName, false, QuoteChar, ref First);
				else
					Output.AppendUmlArgument(Indentation, "Content", this.value, QuoteChar, ref First);
			}

			Output.WriteLine(");");
		}

	}
}
