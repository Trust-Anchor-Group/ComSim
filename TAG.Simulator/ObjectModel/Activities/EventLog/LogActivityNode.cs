using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Waher.Content.Xml;
using Waher.Events;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Activities.EventLog
{
	/// <summary>
	/// Abstract base class for log activity nodes.
	/// </summary>
	public abstract class LogActivityNode : ActivityNode
	{
		/// <summary>
		/// Message attribute
		/// </summary>
		protected StringAttribute message;

		/// <summary>
		/// Event level attribute.
		/// </summary>
		protected EnumAttribute<EventLevel> level;

		/// <summary>
		/// Event ID attribute
		/// </summary>
		protected StringAttribute eventId;

		/// <summary>
		/// Object attribute
		/// </summary>
		protected StringAttribute @object;

		/// <summary>
		/// Actor attribute
		/// </summary>
		protected StringAttribute actor;

		/// <summary>
		/// Abstract base class for log activity nodes.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public LogActivityNode(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Executes a node.
		/// </summary>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>Next node of execution, if different from the default, otherwise null (for default).</returns>
		public override sealed async Task<LinkedListNode<IActivityNode>> Execute(Variables Variables)
		{
			string Message = await this.message.GetValueAsync(Variables);
			string EventId = await this.eventId.GetValueAsync(Variables);
			string Object = await this.@object.GetValueAsync(Variables);
			string Actor = await this.actor.GetValueAsync(Variables);
			EventLevel Level = await this.level.GetValueAsync(Variables);

			Log.Event(new Event(this.EventType, Message, Object, Actor, EventId, Level,
				string.Empty, string.Empty, string.Empty));

			return null;
		}

		/// <summary>
		/// Type of event.
		/// </summary>
		public abstract EventType EventType { get; }

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.message = new StringAttribute(XML.Attribute(Definition, "message"));
			this.level = new EnumAttribute<EventLevel>(XML.Attribute(Definition, "level"));
			this.eventId = new StringAttribute(XML.Attribute(Definition, "eventId"));
			this.@object = new StringAttribute(XML.Attribute(Definition, "object"));
			this.actor = new StringAttribute(XML.Attribute(Definition, "actor"));

			return Task.CompletedTask;
		}

		/// <summary>
		/// Exports PlantUML
		/// </summary>
		/// <param name="Output">Output</param>
		/// <param name="Indentation">Number of tabs to indent.</param>
		/// <param name="QuoteChar">Quote character.</param>
		public override sealed void ExportPlantUml(StreamWriter Output, int Indentation, char QuoteChar)
		{
			Indent(Output, Indentation);
			Output.Write(':');
			Output.Write(this.GetType().Name);
			Output.Write('(');
			Output.Write(this.message.Value);
			Output.WriteLine(");");
		}

	}
}
