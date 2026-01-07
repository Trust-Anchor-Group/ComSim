using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.Extensions;
using TAG.Simulator.ObjectModel;
using TAG.Simulator.ObjectModel.Activities;
using TAG.Simulator.XMPP.Actors;
using TAG.Simulator.XMPP.Legal.Extensions;
using Waher.Content;
using Waher.Content.Xml;
using Waher.Networking.XMPP.Contracts;
using Waher.Script;

namespace TAG.Simulator.XMPP.Legal.Activities
{
	/// <summary>
	/// Petitions another user for a digital signature.
	/// </summary>
	public class PetitionSignature : ActivityNode
	{
		private StringAttribute actor;
		private StringAttribute legalId;
		private StringAttribute purpose;
		private StringAttribute variable;
		private ObjectAttribute content;

		/// <summary>
		/// Petitions another user for a digital signature.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public PetitionSignature(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(PetitionSignature);

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
			return new PetitionSignature(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.actor = new StringAttribute(XML.Attribute(Definition, "actor"));
			this.legalId = new StringAttribute(XML.Attribute(Definition, "legalId"));
			this.purpose = new StringAttribute(XML.Attribute(Definition, "purpose"));
			this.variable = new StringAttribute(XML.Attribute(Definition, "variable"));
			this.content = new ObjectAttribute(XML.Attribute(Definition, "content"));

			return base.FromXml(Definition);
		}

		/// <summary>
		/// Executes a node.
		/// </summary>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>Next node of execution, if different from the default, otherwise null (for default).</returns>
		public override async Task<LinkedListNode<IActivityNode>> Execute(Variables Variables)
		{
			string LegalId = await this.legalId.GetValueAsync(Variables);
			string Purpose = await this.purpose.GetValueAsync(Variables);
			string Variable = await this.variable.GetValueAsync(Variables);
			object Content = await this.variable.GetValueAsync(Variables);

			if (!(await this.GetActorObjectAsync(this.actor, Variables) is XmppActivityObject XmppActor))
				throw new Exception("Actor not an XMPP client.");

			if (!XmppActor.Client.TryGetExtension(out ContractsClient Contracts))
				throw new Exception("Actor does not have a registered legal extension.");

			string PetitionId = Guid.NewGuid().ToString();
			byte[] ContentEncoded;

			if (Content is string s)
				ContentEncoded = Encoding.UTF8.GetBytes(s);
			else if (Content is byte[] Bin)
				ContentEncoded = Bin;
			else
			{
				ContentResponse Encoded = await InternetContent.EncodeAsync(Content, Encoding.UTF8);
				Encoded.AssertOk();

				ContentEncoded = Encoded.Encoded;
			}

			await Contracts.PetitionSignatureAsync(LegalId, ContentEncoded, PetitionId, Purpose);

			Variables[Variable] = PetitionId;

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
			Output.Write(".PetitionSignature");
			Output.Write("(");

			Indentation++;

			Output.AppendUmlArgument(Indentation, "LegalId", this.legalId.Value, true, QuoteChar);
			Output.AppendUmlArgument(Indentation, "Purpose", this.purpose.Value, true, QuoteChar);
			Output.AppendUmlArgument(Indentation, "Content", this.content.ValueString, true, QuoteChar);
			Output.AppendUmlArgument(Indentation, "Variable", this.variable.Value, true, QuoteChar);

			Output.WriteLine(");");
		}
	}
}
