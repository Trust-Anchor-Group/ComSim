using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.Extensions;
using TAG.Simulator.ObjectModel;
using TAG.Simulator.ObjectModel.Activities;
using TAG.Simulator.ObjectModel.Values;
using TAG.Simulator.Web.Actors;
using Waher.Content;
using Waher.Content.Xml;
using Waher.Events;
using Waher.Runtime.Collections;
using Waher.Script;

namespace TAG.Simulator.Web.Activities
{
	/// <summary>
	/// Abstract base class for web methods.
	/// </summary>
	/// <param name="Parent">Parent node</param>
	/// <param name="Model">Model in which the node is defined.</param>
	public abstract class WebCall(ISimulationNode Parent, Model Model)
		: WebNode(Parent, Model), IValueRecipient
	{
		private readonly ChunkedList<Header> headers = [];
		private IValue value;
		private StringAttribute actor;
		private StringAttribute url;
		private StringAttribute variable;

		/// <summary>
		/// Registers a header node.
		/// </summary>
		/// <param name="Header">Header node.</param>
		public void Register(Header Header)
		{
			this.headers.Add(Header);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.actor = new StringAttribute(XML.Attribute(Definition, "actor"));
			this.url = new StringAttribute(XML.Attribute(Definition, "url"));
			this.variable = new StringAttribute(XML.Attribute(Definition, "variable"));

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
			string Url = await this.url.GetValueAsync(Variables);
			string Variable = await this.variable.GetValueAsync(Variables);
			object Content = this.value is null ? null : await this.value.EvaluateAsync(Variables);
			int i, c = this.headers.Count;
			KeyValuePair<string, string>[] Headers = new KeyValuePair<string, string>[c];

			for (i = 0; i < c; i++)
				Headers[i] = await this.headers[i].Evaluate(Variables);

			if (await this.GetActorObjectAsync(this.actor, Variables) is not WebActivityObject WebActor)
				throw new Exception("Actor not a web client.");

			byte[] Data;

			if (Content is not null)
			{
				ContentResponse Encoded = await InternetContent.EncodeAsync(Content, Encoding.UTF8);
				Encoded.AssertOk();

				Headers = Headers.Join(new KeyValuePair<string, string>("Content-Type", Encoded.ContentType));
				Data = Encoded.Encoded;
			}
			else
				Data = null;

			ContentResponse Response = await this.CallMethod(WebActor.Client, Url, Data, Headers);
			Response.AssertOk();

			if (!string.IsNullOrEmpty(Variable))
				Variables[Variable] = Response.Decoded;

			return null;
		}

		/// <summary>
		/// Calls the web method.
		/// </summary>
		/// <param name="Client">Client performing the call.</param>
		/// <param name="Url">URL</param>
		/// <param name="Content">Content payload, if any.</param>
		/// <param name="Headers">HTTP Headers.</param>
		/// <returns>Response.</returns>
		public abstract Task<ContentResponse> CallMethod(CookieWebClient Client, string Url,
			byte[] Content, KeyValuePair<string, string>[] Headers);

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
			Output.Write('.');
			Output.Write(this.GetType().Name);
			Output.Write('(');

			Indentation++;

			Output.AppendUmlArgument(Indentation, "Url", this.url.Value, true, QuoteChar);

			if (!string.IsNullOrEmpty(this.variable.Value))
				Output.AppendUmlArgument(Indentation, "Variable", this.variable.Value, false, QuoteChar);

			if (this.value is not null)
			{
				if (this.value is Xml Xml && !string.IsNullOrEmpty(Xml.RootName))
					Output.AppendUmlArgument(Indentation, "Content", Xml.RootName, false, QuoteChar);
				else
					Output.AppendUmlArgument(Indentation, "Content", this.value, QuoteChar);
			}

			foreach (Header P in this.headers)
				P.ExportPlantUml(Output, Indentation, QuoteChar);

			Output.WriteLine(");");
		}
	}
}
