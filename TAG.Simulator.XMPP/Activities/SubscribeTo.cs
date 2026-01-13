using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.Extensions;
using TAG.Simulator.ObjectModel;
using TAG.Simulator.ObjectModel.Activities;
using TAG.Simulator.XMPP.Actors;
using Waher.Content.Xml;
using Waher.Script;

namespace TAG.Simulator.XMPP.Activities
{
	/// <summary>
	/// Subscribes to the presence of another entity.
	/// </summary>
	public class SubscribeTo : ActivityNode
	{
		private StringAttribute actor;
		private StringAttribute to;

		/// <summary>
		/// Subscribes to the presence of another entity.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public SubscribeTo(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(SubscribeTo);

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
			return new SubscribeTo(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.actor = new StringAttribute(XML.Attribute(Definition, "actor"));
			this.to = new StringAttribute(XML.Attribute(Definition, "to"));

			return base.FromXml(Definition);
		}

		/// <summary>
		/// Executes a node.
		/// </summary>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>Next node of execution, if different from the default, otherwise null (for default).</returns>
		public override async Task<LinkedListNode<IActivityNode>> Execute(Variables Variables)
		{
			string To = await this.to.GetValueAsync(Variables);

			object Obj = await this.GetActorObjectAsync(this.actor, Variables);
			if (!(Obj is XmppActivityObject XmppActor))
				throw new Exception("Actor not an XMPP client.");

			await XmppActor.Client.RequestPresenceSubscription(To);

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

			Output.Indent(Indentation);
			Output.Write(':');
			Output.Write(this.actor.Value);
			Output.Write(".SubscribeTo");
			Output.Write('(');

			Indentation++;

			Output.AppendUmlArgument(Indentation, "To", this.to.Value, true, QuoteChar);
			
			Output.WriteLine(");");
		}

	}
}
