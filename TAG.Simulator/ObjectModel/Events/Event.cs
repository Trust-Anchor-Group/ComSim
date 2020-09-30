using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel.Activities;
using Waher.Content.Xml;
using Waher.Events;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Events
{
	/// <summary>
	/// Abstract base class for events
	/// </summary>
	public abstract class Event : SimulationNodeChildren, IEvent
	{
		private readonly List<TaskCompletionSource<bool>> triggers = new List<TaskCompletionSource<bool>>();
		private LinkedList<IEventPreparation> preparationNodes = null;
		private LinkedList<IExternalEvent> externalEvents = null;
		private IActivity activity;
		private string activityId;
		private string id;
		private bool hasTriggers = false;

		/// <summary>
		/// Abstract base class for events
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public Event(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// ID of event.
		/// </summary>
		public string Id => this.id;

		/// <summary>
		/// ID of Activity to execute when event is triggered.
		/// </summary>
		public string ActivityId => this.activityId;

		/// <summary>
		/// Activity to execute when event is triggered.
		/// </summary>
		public IActivity Activity => this.activity;

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.id = XML.Attribute(Definition, "id");
			this.activityId = XML.Attribute(Definition, "activity");

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
		/// Starts the node.
		/// </summary>
		public override Task Start()
		{
			if (!this.Model.TryGetActivity(this.activityId, out this.activity))
				throw new Exception("Activity not found: " + this.activityId);

			return base.Start();
		}

		/// <summary>
		/// Registers an event preparation node.
		/// </summary>
		/// <param name="PreparationNode">Preparation node.</param>
		public void Register(IEventPreparation PreparationNode)
		{
			if (this.preparationNodes is null)
				this.preparationNodes = new LinkedList<IEventPreparation>();

			this.preparationNodes.AddLast(PreparationNode);
		}

		/// <summary>
		/// Registers an external event.
		/// </summary>
		/// <param name="ExternalEvent">External event.</param>
		public void Register(IExternalEvent ExternalEvent)
		{
			if (this.externalEvents is null)
				this.externalEvents = new LinkedList<IExternalEvent>();

			this.externalEvents.AddLast(ExternalEvent);
		}

		/// <summary>
		/// Triggers the event.
		/// </summary>
		/// <param name="Variables">Event variables</param>
		/// <param name="Guard">Optional guard expression.</param>
		/// <param name="GuardLimit">Maximum number of times to apply guard expression in search of suitable candidates.</param>
		public async void Trigger(Variables Variables, Expression Guard = null, int GuardLimit = int.MaxValue)
		{
			if (this.activity is null)
				return; // Not initialized yet.

			List<KeyValuePair<string, object>> Tags = new List<KeyValuePair<string, object>>();
			KeyValuePair<string, object>[] Tags2 = null;
			DateTime Start = DateTime.Now;

			try
			{
				if (!(this.preparationNodes is null))
				{
					foreach (IEventPreparation Node in this.preparationNodes)
						Node.Prepare(Variables, Tags);
				}

				try
				{
					if (!(Guard is null))
					{
						object Obj = Guard.Evaluate(Variables);
						if (!(Obj is bool b))
							throw new Exception("Guard expression did not evaluate to a boolean value.");

						while (!b)
						{
							if (--GuardLimit < 0)
								throw new Exception("Guard limit exceeded. Event could not be triggered properly.");

							if (!(this.preparationNodes is null))
							{
								foreach (IEventPreparation Node in this.preparationNodes)
									Node.Release(Variables);

								Tags.Clear();

								foreach (IEventPreparation Node in this.preparationNodes)
									Node.Prepare(Variables, Tags);
							}

							Obj = Guard.Evaluate(Variables);
							if (!(Obj is bool b2))
								throw new Exception("Guard expression did not evaluate to a boolean value.");

							b = b2;
						}
					}

					Tags2 = Tags.ToArray();

					TaskCompletionSource<bool>[] Triggers;

					lock (this.triggers)
					{
						if (this.hasTriggers)
						{
							Triggers = this.triggers.ToArray();
							this.triggers.Clear();
							this.hasTriggers = false;
						}
						else
							Triggers = null;
					}

					if (!(Triggers is null))
					{
						foreach (TaskCompletionSource<bool> T in Triggers)
							T.TrySetResult(true);
					}

					this.Model.IncActivityStartCount(this.activityId, this.id, this.activity.LogStart, Tags2);
					await this.activity.ExecuteTask(Variables);
					this.Model.IncActivityFinishedCount(this.activityId, this.id, DateTime.Now - Start, this.activity.LogEnd, Tags2);
				}
				finally
				{
					if (!(this.preparationNodes is null))
					{
						foreach (IEventPreparation Node in this.preparationNodes)
							Node.Release(Variables);
					}
				}
			}
			catch (Exception ex)
			{
				ex = Log.UnnestException(ex);

				if (Tags2 is null)
					Tags2 = Tags.ToArray();

				this.Model.IncActivityErrorCount(this.activityId, this.id, ex, DateTime.Now - Start, Tags2);
			}
		}

		/// <summary>
		/// Exports PlantUML
		/// </summary>
		/// <param name="Output">Output node</param>
		public override async Task ExportMarkdown(StreamWriter Output)
		{
			this.Model.ExportActivitiesIntroduction(Output);

			string Header = string.IsNullOrEmpty(this.Id) ? this.ActivityId : this.Id;
			Output.WriteLine(Header);
			Output.WriteLine(new string('-', Header.Length + 3));
			Output.WriteLine();

			await base.ExportMarkdown(Output);

			Output.WriteLine("```uml: Use Case chart for " + Header);
			Output.WriteLine("@startuml");

			this.ExportUseCaseData(Output);

			Output.WriteLine("@enduml");
			Output.WriteLine("```");
			Output.WriteLine();

			if (!(this.activity is null))
				await this.activity.ExportMarkdown(Output, false, this);
		}

		/// <summary>
		/// Exports use case diagram data.
		/// </summary>
		/// <param name="Output">Output stream.</param>
		public virtual void ExportUseCaseData(StreamWriter Output)
		{
			Output.Write("usecase \"");
			Output.Write(this.activityId);
			Output.WriteLine("\" as UC1");

			if (!(this.preparationNodes is null))
			{
				foreach (IEventPreparation Node in this.preparationNodes)
					Node.ExportPlantUml(Output);
			}

			if (!(this.externalEvents is null))
			{
				int i = 0;
				string s;

				foreach (IExternalEvent ExternalEvent in this.externalEvents)
				{
					if (ExternalEvent.Actor is null)
						continue;

					Output.Write("actor \"");

					if (string.IsNullOrEmpty(ExternalEvent.ActorName))
					{
						s = ExternalEvent.Actor.Id + "_B" + i.ToString();

						Output.Write(ExternalEvent.Actor.Id);
						Output.Write("\" as ");
						Output.WriteLine(s);
					}
					else
					{
						s = ExternalEvent.ActorName + "_B" + i.ToString();

						Output.Write(ExternalEvent.ActorName);
						Output.Write("\" as ");
						Output.Write(s);
						Output.Write(" <<");
						Output.Write(ExternalEvent.Actor.Id);
						Output.WriteLine(">>");
					}

					ExternalEvent.Actor.AnnotateActorUseCaseUml(Output, s);

					Output.Write(s);
					Output.Write(" --> UC1 : ");
					Output.Write(ExternalEvent.Name);
					Output.Write("(");

					IEnumerable<Parameter> Parameters = ExternalEvent.Parameters;
					if (!(Parameters is null))
					{
						bool First = true;

						foreach (Parameter P in Parameters)
						{
							if (First)
								First = false;
							else
								Output.Write(", ");

							if (!string.IsNullOrEmpty(P.Variable) && P.Variable != P.Name)
							{
								Output.Write(P.Variable);
								Output.Write('=');
							}

							Output.Write(P.Name);
						}
					}

					Output.WriteLine(")");

					i++;
				}
			}
		}

		/// <summary>
		/// Exports Probability Script graph.
		/// </summary>
		/// <param name="Output">Output</param>
		/// <returns>If a PDF graph was added.</returns>
		public virtual bool ExportPdfScript(StringBuilder Output)
		{
			return false;
		}

		/// <summary>
		/// Gets a <see cref="Task"/> object, that will be completed when the event is triggered.
		/// </summary>
		/// <returns>Trigger task object.</returns>
		public Task GetTrigger()
		{
			TaskCompletionSource<bool> Trigger = new TaskCompletionSource<bool>();

			lock (this.triggers)
			{
				this.triggers.Add(Trigger);
				this.hasTriggers = true;
			}

			return Trigger.Task;
		}

	}
}
