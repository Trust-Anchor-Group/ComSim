﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Threading.Tasks;
using TAG.Simulator.ObjectModel;
using TAG.Simulator.ObjectModel.Activities;
using TAG.Simulator.ObjectModel.Values;
using TAG.Simulator.MQTT.Actors;
using Waher.Content.Xml;
using Waher.Script;
using Waher.Networking.MQTT;
using System.IO;
using Waher.Content;

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
			base.ExportPlantUml(Output, Indentation, QuoteChar);

			Indent(Output, Indentation);
			Output.Write(':');
			Output.Write(this.actor.Value);
			Output.Write(".Publish");
			Output.Write("(");

			Indentation++;

			this.AppendArgument(Output, Indentation, "Topic", this.topic.Value, true, QuoteChar);
			this.AppendArgument(Output, Indentation, "QoS", this.qos.ToString(), false, QuoteChar);

			if (!this.retain)
				this.AppendArgument(Output, Indentation, "Retain", "true", false, QuoteChar);

			if (!(this.value is null))
			{
				if (this.value is Xml Xml && !string.IsNullOrEmpty(Xml.RootName))
					this.AppendArgument(Output, Indentation, "Content", Xml.RootName, false, QuoteChar);
				else
					this.AppendArgument(Output, Indentation, "Content", this.value, QuoteChar);
			}

			Output.WriteLine(");");
		}

		private void AppendArgument(StreamWriter Output, int Indentation, string Name, string Value, bool Quotes, char QuoteChar)
		{
			this.AppendArgument(Output, Indentation, Name);

			if (Quotes)
				Eval.ExportPlantUml("\"" + Value.Replace("\"", "\\\"") + "\"", Output, Indentation, QuoteChar, false);
			else
				Eval.ExportPlantUml(Value, Output, Indentation, QuoteChar, false);
		}

		private void AppendArgument(StreamWriter Output, int Indentation, string Name, IValue Value, char QuoteChar)
		{
			this.AppendArgument(Output, Indentation, Name);
			Value.ExportPlantUml(Output, Indentation, QuoteChar);
		}

		private void AppendArgument(StreamWriter Output, int Indentation, string Name)
		{
			Output.WriteLine();
			Indent(Output, Indentation);

			Output.Write(Name);
			Output.Write(": ");
		}

	}
}
