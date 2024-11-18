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
using System.IO;

namespace TAG.Simulator.XMPP.Activities
{
	/// <summary>
	/// Sends a custom IQ response to a recipient
	/// </summary>
	public class Respond : ActivityNode, IValueRecipient
	{
		private IValue value;
		private StringAttribute actor;
		private StringAttribute to;
		private StringAttribute requestId;

		/// <summary>
		/// Sends a custom IQ response to a recipient
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public Respond(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(Respond);

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
			return new Respond(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.actor = new StringAttribute(XML.Attribute(Definition, "actor"));
			this.to = new StringAttribute(XML.Attribute(Definition, "to"));
			this.requestId = new StringAttribute(XML.Attribute(Definition, "requestId"));

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
			string RequestId = await this.requestId.GetValueAsync(Variables);

			if (!(await this.GetActorObjectAsync(this.actor, Variables) is XmppActivityObject XmppActor))
				throw new Exception("Actor not an XMPP client.");

			object Content = this.value is null ? string.Empty : await this.value.EvaluateAsync(Variables) ?? string.Empty;
			string Xml;

			if (Content is XmlDocument Doc)
				Xml = Doc.OuterXml;
			else if (Content is XmlElement E)
				Xml = E.OuterXml;
			else if (Content is null)
				Xml = null;
			else
				throw new Exception("Responses must be XML, or empty.");

			await XmppActor.Client.SendIqResult(RequestId, To, Xml);

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
			Output.Write(".Respond");
			Output.Write("(");

			Indentation++;

			SendMessage.AppendArgument(Output, Indentation, "Id", this.requestId.Value, true, QuoteChar);
			SendMessage.AppendArgument(Output, Indentation, "To", this.to.Value, true, QuoteChar);

			if (!(this.value is null))
			{
				if (this.value is Xml Xml && !string.IsNullOrEmpty(Xml.RootName))
					SendMessage.AppendArgument(Output, Indentation, "Content", Xml.RootName, false, QuoteChar);
				else
					SendMessage.AppendArgument(Output, Indentation, "Content", this.value, QuoteChar);
			}

			Output.WriteLine(");");
		}

	}
}
