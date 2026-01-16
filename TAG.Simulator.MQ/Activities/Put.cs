using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.Extensions;
using TAG.Simulator.MQ.Actors;
using TAG.Simulator.ObjectModel;
using TAG.Simulator.ObjectModel.Activities;
using TAG.Simulator.ObjectModel.Values;
using Waher.Content.Xml;
using Waher.Script;

namespace TAG.Simulator.MQ.Activities
{
	/// <summary>
	/// Puts content to a topic.
	/// </summary>
	/// <param name="Parent">Parent node</param>
	/// <param name="Model">Model in which the node is defined.</param>
	public class Put(ISimulationNode Parent, Model Model) 
		: ActivityNode(Parent, Model), IValueRecipient
	{
		private IValue value;
		private StringAttribute actor;
		private StringAttribute queue;

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(Put);

		/// <summary>
		/// Points to the embedded XML Schema resource defining the semantics of the XML namespace.
		/// </summary>
		public override string SchemaResource => MqActorTcp.MqSchema;

		/// <summary>
		/// XML Namespace where the element is defined.
		/// </summary>
		public override string Namespace => MqActorTcp.MqNamespace;

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new Put(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.actor = new StringAttribute(XML.Attribute(Definition, "actor"));
			this.queue = new StringAttribute(XML.Attribute(Definition, "queue"));

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
			string Queue = await this.queue.GetValueAsync(Variables);
			object Content = this.value is null ? string.Empty : await this.value.EvaluateAsync(Variables) ?? string.Empty;
			string Message;

			if (Content is XmlDocument Doc)
				Message = Doc.OuterXml;
			else if (Content is XmlElement E)
				Message = E.OuterXml;
			else
				Message = Content?.ToString() ?? string.Empty;

			if (await this.GetActorObjectAsync(this.actor, Variables) is not MqActivityObject MqActor)
				throw new Exception("Actor not an MQ actor.");

			await MqActor.Client.PutAsync(Queue, Message);

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
			Output.Write(".Put");
			Output.Write('(');

			Indentation++;

			Output.AppendUmlArgument(Indentation, "Queue", this.queue.Value, true, QuoteChar, ref First);

			if (this.value is not null)
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
