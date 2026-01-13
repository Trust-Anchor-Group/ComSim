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
using Waher.Networking.XMPP.Contracts;
using Waher.Networking.XMPP.Contracts.EventArguments;
using Waher.Script;

namespace TAG.Simulator.XMPP.Legal.Activities
{
	/// <summary>
	/// Sends a response to an identity petition.
	/// </summary>
	public class IdentityPetitionResponse : LegalActivityNode
	{
		private StringAttribute actor;
		private StringAttribute variable;
		private BooleanAttribute response;

		/// <summary>
		/// Sends a response to an identity petition.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public IdentityPetitionResponse(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(IdentityPetitionResponse);

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new IdentityPetitionResponse(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.actor = new StringAttribute(XML.Attribute(Definition, "actor"));
			this.variable = new StringAttribute(XML.Attribute(Definition, "variable"));
			this.response = new BooleanAttribute(XML.Attribute(Definition, "response"));

			return base.FromXml(Definition);
		}

		/// <summary>
		/// Executes a node.
		/// </summary>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>Next node of execution, if different from the default, otherwise null (for default).</returns>
		public override async Task<LinkedListNode<IActivityNode>> Execute(Variables Variables)
		{
			string Variable = await this.variable.GetValueAsync(Variables);
			bool Response = await this.response.GetValueAsync(Variables);

			if (!(await this.GetActorObjectAsync(this.actor, Variables) is XmppActivityObject XmppActor))
				throw new Exception("Actor not an XMPP client.");

			if (XmppActor.Client is null)
				throw new Exception("XMPP connection closed.");

			if (!(XmppActor.Client?.TryGetExtension(out ContractsClient Contracts) ?? false))
				throw new Exception("Actor does not have a registered legal extension.");

			if (!Variables.TryGetVariable(Variable, out Variable v))
				throw new Exception("Variable not found: " + Variable);

			if (!(v.ValueObject is LegalIdentityPetitionEventArgs e))
				throw new Exception("Variable does not contain a legal identity petition event arguments: " + Variable);

			await Contracts.PetitionIdentityResponseAsync(e.RequestedIdentityId, e.PetitionId, e.RequestorFullJid, Response);

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
			Output.Write(".IdentityPetitionResponse");
			Output.Write('(');

			Indentation++;

			Output.AppendUmlArgument(Indentation, "Variable", this.variable.Value, true, QuoteChar);
			Output.AppendUmlArgument(Indentation, "Response", this.response.Value, true, QuoteChar);

			Output.WriteLine(");");
		}
	}
}
