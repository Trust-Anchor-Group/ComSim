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
		: WebNode(Parent, Model)
	{
		private readonly ChunkedList<Header> headers = [];
		private Payload payload;
		private StringAttribute actor;
		private StringAttribute url;
		private StringAttribute variable;
		private DurationAttribute timeout;

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
		public override async Task FromXml(XmlElement Definition)
		{
			this.actor = new StringAttribute(XML.Attribute(Definition, "actor"));
			this.url = new StringAttribute(XML.Attribute(Definition, "url"));
			this.variable = new StringAttribute(XML.Attribute(Definition, "variable"));
			this.timeout = new DurationAttribute(XML.Attribute(Definition, "timeout"));

			await base.FromXml(Definition);

			foreach (ISimulationNode Node in this.Children)
			{
				if (Node is Payload Payload)
				{
					if (this.payload is null)
						this.payload = Payload;
					else
						throw new Exception("Only one payload node allowed.");
				}
			}
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
			Waher.Content.Duration Timeout = this.timeout.IsEmpty ? Waher.Content.Duration.FromSeconds(10) : await this.timeout.GetValueAsync(Variables);
			object Content = this.payload?.Value is null ? null : await this.payload.Value.EvaluateAsync(Variables);
			int i, c = this.headers.Count;
			KeyValuePair<string, string>[] Headers = new KeyValuePair<string, string>[c];

			for (i = 0; i < c; i++)
				Headers[i] = await this.headers[i].Evaluate(Variables);

			if (await this.GetActorObjectAsync(this.actor, Variables) is not WebActorActivityObject WebActor)
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

			ContentResponse Response = await this.CallMethod(WebActor.Client, Url, Data,
				Timeout, Headers);

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
		/// <param name="Timeout">Duration before a request times out.</param>
		/// <param name="Headers">HTTP Headers.</param>
		/// <returns>Response.</returns>
		public abstract Task<ContentResponse> CallMethod(CookieWebClient Client, string Url,
			byte[] Content, Waher.Content.Duration Timeout, KeyValuePair<string, string>[] Headers);

		/// <summary>
		/// Exports PlantUML
		/// </summary>
		/// <param name="Output">Output</param>
		/// <param name="Indentation">Number of tabs to indent.</param>
		/// <param name="QuoteChar">Quote character.</param>
		public override void ExportPlantUml(StreamWriter Output, int Indentation, char QuoteChar)
		{
			bool First = true;

			Output.Indent(Indentation);
			Output.Write(':');
			Output.Write(this.actor.Value);
			Output.Write('.');
			Output.Write(this.GetType().Name);
			Output.Write('(');

			Indentation++;

			Output.AppendUmlArgument(Indentation, "Url", this.url.Value, true, QuoteChar, ref First);

			if (!string.IsNullOrEmpty(this.variable.Value))
				Output.AppendUmlArgument(Indentation, "Variable", this.variable.Value, false, QuoteChar, ref First);

			if (this.payload?.Value is not null)
			{
				if (this.payload.Value is Xml Xml && !string.IsNullOrEmpty(Xml.RootName))
					Output.AppendUmlArgument(Indentation, "Payload", Xml.RootName, false, QuoteChar, ref First);
				else
					Output.AppendUmlArgument(Indentation, "Payload", this.payload.Value, QuoteChar, ref First);
			}

			foreach (Header Header in this.headers)
				Output.AppendUmlArgument(Indentation, Header.Name, Header.Value, true, QuoteChar, ref First);

			Output.WriteLine(");");
		}
	}
}
