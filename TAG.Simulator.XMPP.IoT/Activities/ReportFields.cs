using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel.Activities;
using TAG.Simulator.XMPP.IoT.Activities.Fields;
using Waher.Content.Xml;
using Waher.Events;
using Waher.Networking.XMPP.Sensor;
using Waher.Script;
using Waher.Things;
using Waher.Things.SensorData;

namespace TAG.Simulator.XMPP.IoT.Activities
{
	/// <summary>
	/// Reports sensor data fields.
	/// </summary>
	public class ReportFields : XmppIoTActivityNode
	{
		private FieldNode[] fields;
		private string eventArgs;
		private bool more;

		/// <summary>
		/// Reports sensor data fields.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public ReportFields(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(ReportFields);

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new ReportFields(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override async Task FromXml(XmlElement Definition)
		{
			this.eventArgs = XML.Attribute(Definition, "eventArgs", "e");
			this.more = XML.Attribute(Definition, "more", false);

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
			if (!Variables.TryGetVariable(this.eventArgs, out Variable v) ||
				!(v.ValueObject is SensorDataServerRequest e))
			{
				throw new Exception("Sensor data event arguments not found.");
			}

			LinkedList<Field> Fields = new LinkedList<Field>();

			foreach (FieldNode Field in this.fields)
			{
				try
				{
					Field.AddFields(Fields, Variables);
				}
				catch (Exception ex)
				{
					ex = Log.UnnestException(ex);
					e.ReportErrors(false, new ThingError(Field.ThingReference, DateTime.Now, ex.Message));
				}
			}

			e.ReportFields(!this.more, Fields);

			return Task.FromResult<LinkedListNode<IActivityNode>>(null);
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
			Output.Write(":ReportFields(");
			Output.Write(NewMomentaryValues.Join(NewMomentaryValues.GetThingReferences(this.fields)));
			Output.WriteLine(");");
		}

	}
}
