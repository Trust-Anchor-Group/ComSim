using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel.Events;
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
		private LinkedList<IEvent> events = null;
		private string id;
		private int executionCount = 0;
		private bool logStart;
		private bool logEnd;

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
		/// If event should be logged at each start of the activity.
		/// </summary>
		public bool LogStart => this.logStart;

		/// <summary>
		/// If event should be logged at the end of each activity.
		/// </summary>
		public bool LogEnd => this.logEnd;

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(Activity);

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
			this.logStart = XML.Attribute(Definition, "logStart", true);
			this.logEnd = XML.Attribute(Definition, "logEnd", true);

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
		/// Registers an event that calls the activity.
		/// </summary>
		/// <param name="Event">Event.</param>
		public void Register(IEvent Event)
		{
			if (this.events is null)
				this.events = new LinkedList<IEvent>();

			this.events.AddLast(Event);
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
		/// <param name="Output">Output</param>
		public override async Task ExportMarkdown(StreamWriter Output)
		{
			Output.WriteLine(this.id);
			Output.WriteLine(new string('-', this.id.Length + 3));
			Output.WriteLine();

			await base.ExportMarkdown(Output);

			Output.WriteLine("```uml: Use Case chart for " + this.id);
			Output.WriteLine("@startuml");

			int Index = 0;

			if (!(this.events is null))
			{
				foreach (IEvent Event in this.events)
				{
					string Desc = Event.Description;

					Output.Write("usecase UC");
					Output.Write((++Index).ToString());
					Output.Write(" as \"");
					Output.Write(this.id);

					if (!string.IsNullOrEmpty(Desc))
					{
						Output.WriteLine();
						Output.WriteLine("==");
						Output.Write(BreakWords(Desc, 25));
					}

					Output.WriteLine("\"");

					Event.ExportUseCaseData(Output, Index);
				}
			}

			if (Index == 0)
			{
				Output.Write("usecase \"");
				Output.Write(this.id);
				Output.WriteLine("\" as UC1");
			}

			Output.WriteLine("@enduml");
			Output.WriteLine("```");
			Output.WriteLine();


			Output.WriteLine("```uml: Activity chart for " + this.id);
			Output.WriteLine("@startuml");

			foreach (IActivityNode Node in this.activityNodes)
				Node.ExportPlantUml(Output, 0, '"');

			Output.WriteLine("@enduml");
			Output.WriteLine("```");
			Output.WriteLine();

			this.Model.ExportActivityCharts(this.id, Output, this.events);
		}

		private static string BreakWords(string s, int Width)
		{
			StringBuilder sb = new StringBuilder();
			int c, l = 0;

			foreach (string Word in s.Split(new char[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
			{
				c = Word.Length;

				if (l > 0)
				{
					if (l + c < Width)
					{
						sb.Append(' ');
						l++;
					}
					else
					{
						sb.AppendLine();
						l = 0;
					}
				}

				sb.Append(Word);
				l += c;
			}

			return sb.ToString();
		}

	}
}
