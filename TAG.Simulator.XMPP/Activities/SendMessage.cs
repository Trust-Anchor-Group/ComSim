using System;
using System.Collections.Generic;
using System.Xml;
using System.Threading.Tasks;
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
		private string actor;
		private string to;
		private string subject;
		private string language;
		private string threadId;
		private string parentThreadId;

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
			this.actor = XML.Attribute(Definition, "actor");
			this.to = XML.Attribute(Definition, "to");
			this.subject = XML.Attribute(Definition, "subject");
			this.language = XML.Attribute(Definition, "language");
			this.threadId = XML.Attribute(Definition, "threadId");
			this.parentThreadId = XML.Attribute(Definition, "parentThreadId");
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
		public override Task<LinkedListNode<IActivityNode>> Execute(Variables Variables)
		{
			string Actor = Expression.Transform(this.actor, "{", "}", Variables);
			string To = Expression.Transform(this.to, "{", "}", Variables);
			string Subject = Expression.Transform(this.subject, "{", "}", Variables);
			string Language = Expression.Transform(this.language, "{", "}", Variables);
			string ThreadId = Expression.Transform(this.threadId, "{", "}", Variables);
			string ParentThreadId = Expression.Transform(this.parentThreadId, "{", "}", Variables);
			object Content = this.value?.Evaluate(Variables) ?? string.Empty;
			string Xml;
			string Body;

			if (!Variables.TryGetVariable(Actor, out Waher.Script.Variable v))
				throw new Exception("Actor not found: " + this.actor);

			if (!(v.ValueObject is XmppClient Client))
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

			Client.SendMessage(this.type, To, Xml, Body, Subject, Language, ThreadId, ParentThreadId);

			return Task.FromResult<LinkedListNode<IActivityNode>>(null);
		}

		/// <summary>
		/// Exports PlantUML
		/// </summary>
		/// <param name="Output">Output node</param>
		/// <param name="Indentation">Number of tabs to indent.</param>
		/// <param name="QuoteChar">Quote character.</param>
		public override void ExportPlantUml(StreamWriter Output, int Indentation, char QuoteChar)
		{
			base.ExportPlantUml(Output, Indentation, QuoteChar);

			Indent(Output, Indentation);
			Output.Write(':');
			Output.Write(this.actor);
			Output.Write(".SendMessage");
			Output.Write("(");

			Indentation++;

			this.AppendArgument(Output, Indentation, "To", this.to, QuoteChar);
			this.AppendArgument(Output, Indentation, "Type", this.type.ToString(), QuoteChar);

			if (!string.IsNullOrEmpty(this.subject))
				this.AppendArgument(Output, Indentation, "Subject", this.subject, QuoteChar);

			if (!string.IsNullOrEmpty(this.language))
				this.AppendArgument(Output, Indentation, "Language", this.language, QuoteChar);

			if (!string.IsNullOrEmpty(this.threadId))
				this.AppendArgument(Output, Indentation, "ThreadId", this.threadId, QuoteChar);

			if (!string.IsNullOrEmpty(this.parentThreadId))
				this.AppendArgument(Output, Indentation, "ParentThreadId", this.parentThreadId, QuoteChar);

			Output.WriteLine(");");
		}

		private void AppendArgument(StreamWriter Output, int Indentation, string Name, string Value, char QuoteChar)
		{
			this.AppendArgument(Output, Indentation, Name);

			Eval.ExportPlantUml("\"" + Value.Replace("\"", "\\\"") + "\"", Output, Indentation, QuoteChar, false);
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
