using System;
using System.Collections.Generic;
using System.Xml;
using System.Threading.Tasks;
using TAG.Simulator.ObjectModel;
using TAG.Simulator.ObjectModel.Activities;
using TAG.Simulator.ObjectModel.Values;
using TAG.Simulator.XMPP.Actors;
using Waher.Content.Xml;
using Waher.Script;
using Waher.Networking.XMPP;
using System.IO;

namespace TAG.Simulator.XMPP.Activities
{
	/// <summary>
	/// Sends a custom message to a recipient
	/// </summary>
	public class SendMessage : ActivityNode, IValueRecipient
	{
		private IValue value;
		private MessageType type;
		private StringAttribute actor;
		private StringAttribute to;
		private StringAttribute subject;
		private StringAttribute language;
		private StringAttribute threadId;
		private StringAttribute parentThreadId;

		/// <summary>
		/// Sends a custom message to a recipient
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public SendMessage(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "SendMessage";

		/// <summary>
		/// Points to the embedded XML Schema resource defining the semantics of the XML namespace.
		/// </summary>
		public override string SchemaResource => XmppActor.XmppSchema;

		/// <summary>
		/// XML Namespace where the element is defined.
		/// </summary>
		public override string Namespace => XmppActor.XmppNamespace;

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new SendMessage(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.actor = new StringAttribute(XML.Attribute(Definition, "actor"));
			this.to = new StringAttribute(XML.Attribute(Definition, "to"));
			this.subject = new StringAttribute(XML.Attribute(Definition, "subject"));
			this.language = new StringAttribute(XML.Attribute(Definition, "language"));
			this.threadId = new StringAttribute(XML.Attribute(Definition, "threadId"));
			this.parentThreadId = new StringAttribute(XML.Attribute(Definition, "parentThreadId"));
			this.type = (MessageType)XML.Attribute(Definition, "type", MessageType.Normal);

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
			string To = await this.to.GetValueAsync(Variables);
			string Subject = await this.subject.GetValueAsync(Variables);
			string Language = await this.language.GetValueAsync(Variables);
			string ThreadId = await this.threadId.GetValueAsync(Variables);
			string ParentThreadId = await this.parentThreadId.GetValueAsync(Variables);
			object Content = this.value is null ? string.Empty : await this.value.EvaluateAsync(Variables) ?? string.Empty;
			string Xml;
			string Body;

			if (!(await this.GetActorObjectAsync(this.actor, Variables) is XmppActivityObject XmppActor))
				throw new Exception("Actor not an XMPP client.");

			if (Content is XmlDocument Doc)
			{
				Xml = Doc.OuterXml;
				Body = string.Empty;
			}
			else if (Content is XmlElement E)
			{
				Xml = E.OuterXml;
				Body = string.Empty;
			}
			else
			{
				Xml = string.Empty;
				Body = Content.ToString();
			}

			XmppActor.Client.SendMessage(this.type, To, Xml, Body, Subject, Language, ThreadId, ParentThreadId);

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
			Output.Write(".SendMessage");
			Output.Write("(");

			Indentation++;

			AppendArgument(Output, Indentation, "To", this.to.Value, true, QuoteChar);
			AppendArgument(Output, Indentation, "Type", this.type.ToString(), false, QuoteChar);

			if (!string.IsNullOrEmpty(this.subject.Value))
				AppendArgument(Output, Indentation, "Subject", this.subject.Value, true, QuoteChar);

			if (!string.IsNullOrEmpty(this.language.Value))
				AppendArgument(Output, Indentation, "Language", this.language.Value, true, QuoteChar);

			if (!string.IsNullOrEmpty(this.threadId.Value))
				AppendArgument(Output, Indentation, "ThreadId", this.threadId.Value, true, QuoteChar);

			if (!string.IsNullOrEmpty(this.parentThreadId.Value))
				AppendArgument(Output, Indentation, "ParentThreadId", this.parentThreadId.Value, true, QuoteChar);

			if (!(this.value is null))
			{
				if (this.value is Xml Xml && !string.IsNullOrEmpty(Xml.RootName))
					AppendArgument(Output, Indentation, "Content", Xml.RootName, false, QuoteChar);
				else
					AppendArgument(Output, Indentation, "Content", this.value, QuoteChar);
			}

			Output.WriteLine(");");
		}

		/// <summary>
		/// Appends an argument
		/// </summary>
		/// <param name="Output">Output</param>
		/// <param name="Indentation">Indentation</param>
		/// <param name="Name">Name to append</param>
		/// <param name="Value">Value</param>
		/// <param name="Quotes">Quotes</param>
		/// <param name="QuoteChar">Quote character</param>
		public static void AppendArgument(StreamWriter Output, int Indentation, string Name, string Value, bool Quotes, char QuoteChar)
		{
			AppendArgument(Output, Indentation, Name);

			if (Quotes)
				Eval.ExportPlantUml("\"" + Value.Replace("\"", "\\\"") + "\"", Output, Indentation, QuoteChar, false);
			else
				Eval.ExportPlantUml(Value, Output, Indentation, QuoteChar, false);
		}

		/// <summary>
		/// Appends an argument
		/// </summary>
		/// <param name="Output">Output</param>
		/// <param name="Indentation">Indentation</param>
		/// <param name="Name">Name to append</param>
		/// <param name="Value">Value</param>
		/// <param name="QuoteChar">Quote character</param>
		public static void AppendArgument(StreamWriter Output, int Indentation, string Name, IValue Value, char QuoteChar)
		{
			AppendArgument(Output, Indentation, Name);
			Value.ExportPlantUml(Output, Indentation, QuoteChar);
		}

		/// <summary>
		/// Appends an argument
		/// </summary>
		/// <param name="Output">Output</param>
		/// <param name="Indentation">Indentation</param>
		/// <param name="Name">Name to append</param>
		public static void AppendArgument(StreamWriter Output, int Indentation, string Name)
		{
			Output.WriteLine();
			Indent(Output, Indentation);

			Output.Write(Name);
			Output.Write(": ");
		}

	}
}
