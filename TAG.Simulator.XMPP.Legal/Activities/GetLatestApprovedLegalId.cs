using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel;
using TAG.Simulator.ObjectModel.Activities;
using TAG.Simulator.ObjectModel.Activities.Execution;
using TAG.Simulator.ObjectModel.Values;
using TAG.Simulator.XMPP.Actors;
using TAG.Simulator.XMPP.Legal.Extensions;
using Waher.Content.Xml;
using Waher.Networking.XMPP.Contracts;
using Waher.Script;

namespace TAG.Simulator.XMPP.Legal.Activities
{
	/// <summary>
	/// Checks a user's identity.
	/// </summary>
	public class GetLatestApprovedLegalId : ActivityNode
	{
		private StringAttribute actor;
		private StringAttribute variable;

		/// <summary>
		/// Checks a user's identity.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public GetLatestApprovedLegalId(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(GetLatestApprovedLegalId);

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
			return new GetLatestApprovedLegalId(Parent, Model);
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

			Variables[Variable] = await Contracts.GetLatestApprovedLegalId();

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
			Output.Write(".GetLatestApprovedLegalId");
			Output.Write("(");

			Indentation++;

			AppendArgument(Output, Indentation, "Variable", this.variable.Value, true, QuoteChar);

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
