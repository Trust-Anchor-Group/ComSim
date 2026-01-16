using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.Extensions;
using TAG.Simulator.ObjectModel;
using TAG.Simulator.ObjectModel.Activities;
using Waher.Content.Xml;
using Waher.Networking.XMPP;
using Waher.Networking.XMPP.Sensor;
using Waher.Script;
using Waher.Things;
using Waher.Things.SensorData;

namespace TAG.Simulator.XMPP.IoT.Activities
{
	/// <summary>
	/// How sensor data is to be returned.
	/// </summary>
	public enum SensorDataResponseType
	{
		/// <summary>
		/// As an array of fields.
		/// </summary>
		Array,

		/// <summary>
		/// As an object ex-nihilo
		/// </summary>
		Object
	}

	/// <summary>
	/// Sends a custom IQ request to a recipient
	/// </summary>
	public class ReadSensor : XmppIoTActivityNode
	{
		private ThingReference[] nodeReferences;
		private string[] fields;
		private Waher.Things.SensorData.FieldType fieldTypes;
		private StringAttribute actor;
		private StringAttribute to;
		private string responseVariable;
		private SensorDataResponseType responseType;

		/// <summary>
		/// Sends a custom IQ request to a recipient
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public ReadSensor(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(ReadSensor);

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new ReadSensor(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override async Task FromXml(XmlElement Definition)
		{
			this.actor = new StringAttribute(XML.Attribute(Definition, "actor"));
			this.to = new StringAttribute(XML.Attribute(Definition, "to"));
			this.responseVariable = XML.Attribute(Definition, "responseVariable");
			this.responseType = (SensorDataResponseType)XML.Attribute(Definition, "responseType", SensorDataResponseType.Object);
			this.fieldTypes = 0;

			await base.FromXml(Definition);

			List<ThingReference> Nodes = null;
			List<string> Fields = null;

			foreach (ISimulationNode Node in this.Children)
			{
				if (Node is NodeReference NodeRef)
				{
					Nodes ??= new List<ThingReference>();
					Nodes.Add(NodeRef.ThingReference);
				}
				else if (Node is FieldReference FieldRef)
				{
					Fields ??= new List<string>();
					Fields.Add(FieldRef.Name);
				}
				else if (Node is FieldType FieldType)
					this.fieldTypes |= FieldType.Type;
			}

			this.nodeReferences = Nodes?.ToArray();
			this.fields = Fields?.ToArray();
		}

		/// <summary>
		/// Executes a node.
		/// </summary>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>Next node of execution, if different from the default, otherwise null (for default).</returns>
		public override async Task<LinkedListNode<IActivityNode>> Execute(Variables Variables)
		{
			string To = await this.to.GetValueAsync(Variables);

			if (!(await this.GetActorObjectAsync(this.actor, Variables) is SensorClient SensorClient))
				throw new Exception("Actor not an XMPP Sensor Client.");

			if (XmppClient.BareJidRegEx.IsMatch(To))
			{
				RosterItem Item = SensorClient.Client[To]
					?? throw new Exception("No connection in roster with Bare JID: " + To);

				if (!Item.HasLastPresence || !Item.LastPresence.IsOnline)
					throw new Exception("Contact not online: " + To);

				To = Item.LastPresenceFullJid;
			}

			TaskCompletionSource<bool> T = new TaskCompletionSource<bool>();
			Dictionary<string, Field> FieldsAsObject = this.responseType == SensorDataResponseType.Object ? new Dictionary<string, Field>() : null;
			List<Field> FieldsAsArray = this.responseType == SensorDataResponseType.Array ? new List<Field>() : null;
			List<ThingError> Errors = new List<ThingError>();
			SensorDataClientRequest Request = await SensorClient.RequestReadout(To, this.nodeReferences, this.fields, this.fieldTypes);

			Request.OnErrorsReceived += (Sender, NewErrors) =>
			{
				lock (Errors)
				{
					Errors.AddRange(NewErrors);
				}

				return Task.CompletedTask;
			};

			Request.OnFieldsReceived += (Sender, NewFields) =>
			{
				if (this.responseType == SensorDataResponseType.Object)
				{
					lock (FieldsAsObject)
					{
						foreach (Field F in NewFields)
							FieldsAsObject[F.Name] = F;
					}
				}
				else
				{
					lock (FieldsAsArray)
					{
						FieldsAsArray.AddRange(NewFields);
					}
				}

				return Task.CompletedTask;
			};

			Request.OnStateChanged += (Sender, NewState) =>
			{
				switch (NewState)
				{
					case SensorDataReadoutState.Done:
						T.TrySetResult(true);
						break;

					case SensorDataReadoutState.Failure:
					case SensorDataReadoutState.Cancelled:
						T.TrySetResult(false);
						break;
				}

				return Task.CompletedTask;
			};

			if (!await T.Task)
			{
				StringBuilder sb = new StringBuilder();

				sb.AppendLine("Sensor Data readout failed. Errors reported: ");
				sb.AppendLine();

				foreach (ThingError Error in Errors)
					sb.AppendLine(Error.ErrorMessage);  // TODO: Node reference, if available.

				throw new Exception(sb.ToString());
			}

			Variables[this.responseVariable] = (object)FieldsAsObject ?? FieldsAsArray.ToArray();

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
			bool First = true;

			base.ExportPlantUml(Output, Indentation, QuoteChar);

			Output.Indent(Indentation);
			Output.Write(':');
			Output.Write(this.actor.Value);
			Output.Write(".ReadSensorData");
			Output.Write('(');

			Indentation++;

			Output.AppendUmlArgument(Indentation, "To", this.to.Value, true, QuoteChar, ref First);

			Output.WriteLine(");");
		}

	}
}
