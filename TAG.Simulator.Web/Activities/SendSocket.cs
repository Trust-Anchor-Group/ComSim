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
using Waher.Content.Json;
using Waher.Content.Xml;
using Waher.Script;
using Waher.Script.Abstraction.Elements;

namespace TAG.Simulator.Web.Activities
{
	/// <summary>
	/// Sends data over a web-socket.
	/// </summary>
	/// <param name="Parent">Parent node</param>
	/// <param name="Model">Model in which the node is defined.</param>
	public class SendSocket(ISimulationNode Parent, Model Model)
		: WebNode(Parent, Model)
	{
		private Payload payload;
		private StringAttribute actor;

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(SendSocket);

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new SendSocket(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override async Task FromXml(XmlElement Definition)
		{
			this.actor = new StringAttribute(XML.Attribute(Definition, "actor"));

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

			if (this.payload is null)
				throw new Exception("Missing payload.");
		}

		/// <summary>
		/// Executes a node.
		/// </summary>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>Next node of execution, if different from the default, otherwise null (for default).</returns>
		public override async Task<LinkedListNode<IActivityNode>> Execute(Variables Variables)
		{
			object Content = this.payload?.Value is null ? null : await this.payload.Value.EvaluateAsync(Variables);

			if (await this.GetActorObjectAsync(this.actor, Variables) is not WebSocketActorActivityObject WebSocketActor)
				throw new Exception("Actor not a web socket actor.");

			if (Content is byte[] Data)
				await WebSocketActor.Client.SendBinary(Data);
			else if (Content is string s)
				await WebSocketActor.Client.SendText(s);
			else if (Content is XmlNode Xml)
				await WebSocketActor.Client.SendText(Xml.OuterXml);
			else if (Content is Dictionary<string, object> ||
				Content is Dictionary<string, IElement> ||
				Content is Array ||
				Content is null)
			{
				await WebSocketActor.Client.SendText(JSON.Encode(Content, false));
			}
			else
			{
				ContentResponse Encoded = await InternetContent.EncodeAsync(Content, Encoding.UTF8);
				Encoded.AssertOk();

				if (Encoded.ContentType.StartsWith(JsonCodec.DefaultContentType))
					await WebSocketActor.Client.SendText(Encoded.Encoded);
				else
					await WebSocketActor.Client.SendBinary(Encoded.Encoded);
			}

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
			Output.Write(".SendSocket(");

			Indentation++;

			if (this.payload?.Value is not null)
			{
				if (this.payload.Value is Xml Xml && !string.IsNullOrEmpty(Xml.RootName))
					Output.AppendUmlArgument(Indentation, "Content", Xml.RootName, false, QuoteChar);
				else
					Output.AppendUmlArgument(Indentation, "Content", this.payload.Value, QuoteChar);
			}

			Output.WriteLine(");");
		}
	}
}
