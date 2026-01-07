using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel.Events;
using Waher.Content.Xml;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Activities.Execution
{
	/// <summary>
	/// Waits for an event to be triggered
	/// </summary>
	public class Triggered : ActivityNode, ITriggerNode
	{
		private string @event;
		private IEvent eventRef;

		/// <summary>
		/// Waits for an event to be triggered
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public Triggered(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Condition string
		/// </summary>
		public string Event => this.@event;

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(Triggered);

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new Triggered(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.@event = XML.Attribute(Definition, "event");

			return base.FromXml(Definition);
		}

		/// <summary>
		/// Initialized the node before simulation.
		/// </summary>
		public override Task Initialize()
		{
			if (this.Parent is Wait Wait)
				Wait.Register(this);

			if (!this.Model.TryGetEvent(this.@event, out this.eventRef))
				throw new Exception("Event " + this.@event + " not found.");

			return base.Initialize();
		}

		/// <summary>
		/// Executes a node.
		/// </summary>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>Next node of execution, if different from the default, otherwise null (for default).</returns>
		public override async Task<LinkedListNode<IActivityNode>> Execute(Variables Variables)
		{
			await Activity.ExecuteActivity(Variables, this.FirstNode);
			return null;
		}

		/// <summary>
		/// Gets a task object.
		/// </summary>
		/// <returns>Task object signalling when trigger is activated.</returns>
		public Task GetTask()
		{
			return this.eventRef.GetTrigger();
		}

		/// <summary>
		/// Exports PlantUML
		/// </summary>
		/// <param name="Output">Output</param>
		/// <param name="Indentation">Number of tabs to indent.</param>
		/// <param name="First">If the condition is the first condition.</param>
		/// <param name="QuoteChar">Quote character.</param>
		public void ExportPlantUml(StreamWriter Output, int Indentation, bool First, char QuoteChar)
		{
			Indent(Output, Indentation);

			if (First)
				Output.WriteLine("split");
			else
				Output.WriteLine("split again");

			Indentation++;
			Indent(Output, Indentation);

			Output.Write("#FireBrick:");
			Output.Write(this.@event);
			Output.WriteLine("<");

			base.ExportPlantUml(Output, Indentation + 1, QuoteChar);
		}

	}
}
