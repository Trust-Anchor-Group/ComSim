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
	/// Gets content to a topic.
	/// </summary>
	public class Get : ActivityNode
	{
		private StringAttribute actor;
		private StringAttribute queue;
		private StringAttribute variable;
		private Waher.Content.Duration timeout;

		/// <summary>
		/// Gets content to a topic.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public Get(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "Get";

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
			return new Get(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.actor = new StringAttribute(XML.Attribute(Definition, "actor"));
			this.queue = new StringAttribute(XML.Attribute(Definition, "queue"));
			this.variable = new StringAttribute(XML.Attribute(Definition, "variable"));
			this.timeout = XML.Attribute(Definition, "timeout", Waher.Content.Duration.Zero);

			return base.FromXml(Definition);
		}

		/// <summary>
		/// Executes a node.
		/// </summary>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>Next node of execution, if different from the default, otherwise null (for default).</returns>
		public override async Task<LinkedListNode<IActivityNode>> Execute(Variables Variables)
		{
			string Queue = this.queue.GetValue(Variables);
			string Variable = this.variable.GetValue(Variables);

			if (!(this.GetActorObject(this.actor, Variables) is MqActivityObject MqActor))
				throw new Exception("Actor not an MQ actor.");

			string Message = await MqActor.Client.GetOneAsync(Queue);
			Variables[Variable] = Message;

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
			Output.Write(".Get");
			Output.Write("(");

			Indentation++;

			this.AppendArgument(Output, Indentation, "Queue", this.queue.Value, true, QuoteChar);
			this.AppendArgument(Output, Indentation, "Variable", this.variable.Value, true, QuoteChar);
			this.AppendArgument(Output, Indentation, "Timeout", Duration.ToString(this.timeout), true, QuoteChar);

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

		private void AppendArgument(StreamWriter Output, int Indentation, string Name)
		{
			Output.WriteLine();
			Indent(Output, Indentation);

			Output.Write(Name);
			Output.Write(": ");
		}

	}
}
