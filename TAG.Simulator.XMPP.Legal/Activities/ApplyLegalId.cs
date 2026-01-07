using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.Extensions;
using TAG.Simulator.ObjectModel;
using TAG.Simulator.ObjectModel.Activities;
using TAG.Simulator.XMPP.Actors;
using TAG.Simulator.XMPP.Legal.Extensions;
using Waher.Content.Xml;
using Waher.Networking.XMPP.Contracts;
using Waher.Runtime.Collections;
using Waher.Script;

namespace TAG.Simulator.XMPP.Legal.Activities
{
	/// <summary>
	/// Applies for a new Legal ID
	/// </summary>
	public class ApplyLegalId : ActivityNode
	{
		private readonly ChunkedList<Property> properties = new ChunkedList<Property>();
		private StringAttribute actor;
		private StringAttribute variable;

		/// <summary>
		/// Checks a user's identity.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public ApplyLegalId(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(ApplyLegalId);

		/// <summary>
		/// Points to the embedded XML Schema resource defining the semantics of the XML namespace.
		/// </summary>
		public override string SchemaResource => LegalExtension.XmppSchema;

		/// <summary>
		/// XML Namespace where the element is defined.
		/// </summary>
		public override string Namespace => LegalExtension.XmppNamespace;

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new ApplyLegalId(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.actor = new StringAttribute(XML.Attribute(Definition, "actor"));
			this.variable = new StringAttribute(XML.Attribute(Definition, "variable"));

			return base.FromXml(Definition);
		}

		/// <summary>
		/// Registers a property node.
		/// </summary>
		/// <param name="Property">Property node.</param>
		public void Register(Property Property)
		{
			this.properties.Add(Property);
		}

		/// <summary>
		/// Executes a node.
		/// </summary>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>Next node of execution, if different from the default, otherwise null (for default).</returns>
		public override async Task<LinkedListNode<IActivityNode>> Execute(Variables Variables)
		{
			string Variable = await this.variable.GetValueAsync(Variables);

			if (!(await this.GetActorObjectAsync(this.actor, Variables) is XmppActivityObject XmppActor))
				throw new Exception("Actor not an XMPP client.");

			if (!XmppActor.Client.TryGetExtension(out ContractsClient Contracts))
				throw new Exception("Actor does not have a registered legal extension.");

			int i, c = this.properties.Count;
			Waher.Networking.XMPP.Contracts.Property[] Properties = new Waher.Networking.XMPP.Contracts.Property[c];

			for (i = 0; i < c; i++)
				Properties[i] = await this.properties[i].Evaluate(Variables);

			LegalIdentity Id = await Contracts.ApplyAsync(Properties);
			Variables[Variable] = Id;

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
			Output.Indent(Indentation);
			Output.Write(':');
			Output.Write(this.actor.Value);
			Output.Write(".ApplyLegalId");
			Output.Write("(");

			Indentation++;

			foreach (Property P in this.properties)
				P.ExportPlantUml(Output, Indentation, QuoteChar);

			Output.WriteLine(");");
		}
	}
}
