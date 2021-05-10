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
	/// Sets the presence of the current actor.
	/// </summary>
	public class SetPresence : ActivityNode, IValueRecipient
	{
		private IValue value;
		private StringAttribute actor;
		private Availability availability;

		/// <summary>
		/// Sets the presence of the current actor.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public SetPresence(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "SetPresence";

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
			return new SetPresence(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.actor = new StringAttribute(XML.Attribute(Definition, "actor"));
			this.availability = (Availability)XML.Attribute(Definition, "availability", Availability.Online);

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
			object Content = this.value?.Evaluate(Variables) ?? string.Empty;
			string Xml;

			if (!(this.GetActorObject(this.actor, Variables) is XmppActivityObject XmppActor))
				throw new Exception("Actor not an XMPP client.");

			if (Content is XmlDocument Doc)
				Xml = Doc.OuterXml;
			else if (Content is XmlElement E)
				Xml = E.OuterXml;
			else
				throw new Exception("Custom content in presence must be XML.");

			XmppActor.Client.SetTag("CutomPresenceXML", Xml);
			XmppActor.Client.SetPresence(this.availability);

			return Task.FromResult<LinkedListNode<IActivityNode>>(null);
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
			Output.Write(".SetPresence");
			Output.Write("(");

			Indentation++;

			SendMessage.AppendArgument(Output, Indentation, "Availability", this.availability.ToString(), true, QuoteChar);

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
