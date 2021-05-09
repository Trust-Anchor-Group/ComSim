using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel.Activities;
using TAG.Simulator.XMPP.IoT.Activities.Fields;
using Waher.Content.Xml;
using Waher.Events;
using Waher.Networking.XMPP;
using Waher.Networking.XMPP.PEP;
using Waher.Networking.XMPP.Sensor;
using Waher.Script;
using Waher.Things;
using Waher.Things.SensorData;

namespace TAG.Simulator.XMPP.IoT.Activities
{
	/// <summary>
	/// Reports new momentary sensor data fields.
	/// </summary>
	public class NewMomentaryValues : XmppIoTActivityNode
	{
		private FieldNode[] fields;
		private string sensor;

		/// <summary>
		/// Reports new momentary sensor data fields.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public NewMomentaryValues(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "NewMomentaryValues";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new NewMomentaryValues(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override async Task FromXml(XmlElement Definition)
		{
			this.sensor = XML.Attribute(Definition, "sensor");

			await base.FromXml(Definition);

			List<FieldNode> Fields = new List<FieldNode>();

			foreach (ISimulationNode Node in this.Children)
			{
				if (Node is FieldNode Field)
					Fields.Add(Field);
			}

			this.fields = Fields.ToArray();
		}

		/// <summary>
		/// Executes a node.
		/// </summary>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>Next node of execution, if different from the default, otherwise null (for default).</returns>
		public override Task<LinkedListNode<IActivityNode>> Execute(Variables Variables)
		{
			if (!Variables.TryGetVariable(this.sensor, out Variable v))
				throw new Exception("Sensor not found: " + this.sensor);

			if (!(v.ValueObject is SensorServer Sensor))
				throw new Exception("Not a sensor server object: " + this.sensor);

			Dictionary<ThingReference, LinkedList<Field>> FieldsByThing = new Dictionary<ThingReference, LinkedList<Field>>();
			ThingReference Ref;

			foreach (FieldNode Field in this.fields)
			{
				Ref = Field.ThingReference;
				if (!FieldsByThing.TryGetValue(Ref, out LinkedList<Field> Fields))
				{
					Fields = new LinkedList<Field>();
					FieldsByThing[Ref] = Fields;
				}

				try
				{
					Field.AddFields(Fields, Variables);
				}
				catch (Exception ex)
				{
					Log.Critical(ex);
				}
			}

			foreach (KeyValuePair<ThingReference, LinkedList<Field>> P in FieldsByThing)
			{
				LinkedList<Field> Fields = P.Value;
				LinkedListNode<Field> Loop = Fields.First;
				LinkedListNode<Field> Next;
				bool Found = false;

				while (Loop != null)
				{
					Next = Loop.Next;
					if (Loop.Value.Type.HasFlag(FieldType.Momentary))
						Found = true;
					else
						Fields.Remove(Loop);

					Loop = Next;
				}

				if (Found)
				{
					Sensor.NewMomentaryValues(P.Key, Fields);

					if (Sensor.Client.TryGetExtension(typeof(PepClient), out IXmppExtension Extension) &&
						Extension is PepClient PepClient)
					{
						PepClient.Publish(new SensorData(Fields), null, null);
					}
				}
			}

			return Task.FromResult<LinkedListNode<IActivityNode>>(null);
		}

		/// <summary>
		/// Gets referenced thing references
		/// </summary>
		/// <param name="FieldNodes">Field nodes</param>
		/// <returns>Thing references</returns>
		public static string[] GetThingReferences(FieldNode[] FieldNodes)
		{
			SortedDictionary<string, bool> Sorted = new SortedDictionary<string, bool>();

			foreach (FieldNode Node in FieldNodes)
				Sorted[GetReference(Node.ThingReference)] = true;

			string[] Result = new string[Sorted.Count];
			Sorted.Keys.CopyTo(Result, 0);

			return Result;
		}

		/// <summary>
		/// Gets a string reference representing a <see cref="ThingReference"/>.
		/// </summary>
		/// <param name="Ref">Thing reference.</param>
		/// <returns>String representation of thing reference.</returns>
		public static string GetReference(ThingReference Ref)
		{
			StringBuilder sb = new StringBuilder();

			if (!string.IsNullOrEmpty(Ref.SourceId))
			{
				sb.Append(Ref.SourceId);
				sb.Append('.');
			}

			if (!string.IsNullOrEmpty(Ref.Partition))
			{
				sb.Append(Ref.Partition);
				sb.Append('.');
			}

			sb.Append(Ref.NodeId);

			return sb.ToString();
		}

		/// <summary>
		/// Joins an array of references, and delimits them with ", ".
		/// </summary>
		/// <param name="References">References</param>
		/// <returns>Joined set of references.</returns>
		public static string Join(string[] References)
		{
			StringBuilder sb = new StringBuilder();
			bool First = true;

			foreach (string Ref in References)
			{
				if (First)
					First = false;
				else
					sb.Append(", ");

				sb.Append(Ref);
			}

			return sb.ToString();
		}

		/// <summary>
		/// Exports PlantUML
		/// </summary>
		/// <param name="Output">Output</param>
		/// <param name="Indentation">Number of tabs to indent.</param>
		/// <param name="QuoteChar">Quote character.</param>
		public override void ExportPlantUml(StreamWriter Output, int Indentation, char QuoteChar)
		{
			Indent(Output, Indentation);
			Output.Write(":NewMomentaryFields(");
			Output.Write(this.sensor);

			string s = Join(GetThingReferences(this.fields));
			if (!string.IsNullOrEmpty(s))
			{
				Output.Write(", ");
				Output.Write(s);
			}

			Output.WriteLine(");");
		}

	}
}
