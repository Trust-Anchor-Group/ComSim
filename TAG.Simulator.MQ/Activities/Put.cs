using System;
using System.Collections.Generic;
using System.Xml;
using System.Threading.Tasks;
using TAG.Simulator.ObjectModel;
using TAG.Simulator.ObjectModel.Activities;
using TAG.Simulator.ObjectModel.Values;
using TAG.Simulator.MQ.Actors;
using Waher.Content.Xml;
using Waher.Script;
using System.IO;

namespace TAG.Simulator.MQ.Activities
{
	/// <summary>
	/// Puts content to a topic.
	/// </summary>
	public class Put : ActivityNode, IValueRecipient
	{
		private IValue value;
		private StringAttribute actor;
		private StringAttribute queue;

		/// <summary>
		/// Puts content to a topic.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public Put(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "Put";

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
			string Queue = this.queue.GetValue(Variables);
			object Content = this.value?.Evaluate(Variables) ?? string.Empty;
			string Message;

			if (Content is XmlDocument Doc)
				Message = Doc.OuterXml;
			else if (Content is XmlElement E)
				Message = E.OuterXml;
			else
				Message = Content?.ToString() ?? string.Empty;

			if (!(this.GetActorObject(this.actor, Variables) is MqActivityObject MqActor))
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
			base.ExportPlantUml(Output, Indentation, QuoteChar);

			Indent(Output, Indentation);
			Output.Write(':');
			Output.Write(this.actor);
			Output.Write(".Put");
			Output.Write("(");

			Indentation++;

			this.AppendArgument(Output, Indentation, "Queue", this.queue.Value, true, QuoteChar);

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
