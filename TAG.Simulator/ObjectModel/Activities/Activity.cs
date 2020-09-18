using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Waher.Content.Xml;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Activities
{
	/// <summary>
	/// Represents an activity that can be executed as the result of triggered events.
	/// </summary>
	public class Activity : SimulationNodeChildren, IActivity
	{
		private LinkedList<IActivityNode> activityNodes = null;
		private string id;
		private int executionCount = 0;

		/// <summary>
		/// Represents an activity that can be executed as the result of triggered events.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public Activity(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// ID of activity.
		/// </summary>
		public string Id => this.id;

		/// <summary>
		/// Execution count
		/// </summary>
		public int ExecutionCount => this.executionCount;

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "Activity";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new Activity(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.id = XML.Attribute(Definition, "id");

			return base.FromXml(Definition);
		}

		/// <summary>
		/// Initialized the node before simulation.
		/// </summary>
		public override Task Initialize()
		{
			this.Model.Register(this);
			return base.Initialize();
		}

		/// <summary>
		/// Registers a child activity node.
		/// </summary>
		/// <param name="Node">Activity node.</param>
		public void Register(IActivityNode Node)
		{
			if (this.activityNodes is null)
				this.activityNodes = new LinkedList<IActivityNode>();

			this.Model.Register(this.activityNodes.AddLast(Node));
		}

		/// <summary>
		/// Executes the activity.
		/// </summary>
		/// <param name="Variables">Set of variables for the activity.</param>
		public virtual async Task ExecuteTask(Variables Variables)
		{
			this.executionCount++;

			if (!(this.activityNodes is null))
			{
				try
				{
					await ExecuteActivity(Variables, this.activityNodes.First);
				}
				catch (FinishedException)
				{
					// Execution finished.
				}
			}
		}

		/// <summary>
		/// Executes an activity by executing a possibly branching sequence of nodes.
		/// </summary>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <param name="Start">Node to start execution with.</param>
		public static async Task ExecuteActivity(Variables Variables, LinkedListNode<IActivityNode> Start)
		{
			LinkedListNode<IActivityNode> Next;

			while (!(Start is null))
			{
				Next = await Start.Value.Execute(Variables);
				if (Next is null)
					Next = Start.Next;

				Start = Next;
			}
		}

		/// <summary>
		/// Exports PlantUML
		/// </summary>
		/// <param name="Output">Output node</param>
		public override async Task ExportMarkdown(StreamWriter Output)
		{
			this.Model.ExportActivitiesIntroduction(Output);

			Output.WriteLine(this.id);
			Output.WriteLine(new string('-', this.id.Length + 3));
			Output.WriteLine();

			await base.ExportMarkdown(Output);

			Output.WriteLine("```uml: Activity chart for " + this.id);
			Output.WriteLine("@startuml");

			foreach (IActivityNode Node in this.activityNodes)
				Node.ExportPlantUml(Output, 0, '"');

			Output.WriteLine("@enduml");
			Output.WriteLine("```");
			Output.WriteLine();

			this.Model.ExportActivityCharts(this.id, Output);
		}

	}
}
