using System;
using System.Collections.Generic;
using System.IO;
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
		private LinkedList<IEventPreparation> preparationNodes = null;
		private LinkedList<ExternalEvent> externalEvents = null;
		private IActivity activity;
		private string activityId;
		private string id;

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
		public void Register(ExternalEvent ExternalEvent)
		{
			if (this.externalEvents is null)
				this.externalEvents = new LinkedList<ExternalEvent>();

			this.externalEvents.AddLast(ExternalEvent);
		}

		/// <summary>
		/// Triggers the event.
		/// </summary>
		/// <param name="Variables">Event variables</param>
		public async void Trigger(Variables Variables)
		{
			KeyValuePair<string, object>[] Tags2 = null;
			DateTime Start = DateTime.Now;

			try
			{
				List<KeyValuePair<string, object>> Tags = new List<KeyValuePair<string, object>>();

				if (!(this.preparationNodes is null))
				{
					foreach (IEventPreparation Node in this.preparationNodes)
						Node.Prepare(Variables, Tags);
				}

				Tags2 = Tags.ToArray();

				try
				{
					this.Model.IncActivityStartCount(this.activityId, this.id, Tags2);
					await this.activity.ExecuteTask(Variables);
					this.Model.IncActivityFinishedCount(this.activityId, this.id, DateTime.Now - Start, Tags2);
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
					Tags2 = new KeyValuePair<string, object>[0];

				this.Model.IncActivityErrorCount(this.activityId, this.id, ex.Message, DateTime.Now - Start, Tags2);
			}
		}

		/// <summary>
		/// Exports PlantUML
		/// </summary>
		/// <param name="Output">Output node</param>
		public override async Task ExportMarkdown(StreamWriter Output)
		{
			this.Model.ExportUseCasesIntroduction(Output);

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

				foreach (ExternalEvent ExternalEvent in this.externalEvents)
				{
					if (ExternalEvent.Actor is null)
						continue;

					Output.Write("actor \"");

					if (string.IsNullOrEmpty(ExternalEvent.ActorName))
					{
						Output.Write(ExternalEvent.Actor.Id);
						Output.Write("\" as ");
						Output.Write(ExternalEvent.Actor.Id);
						Output.Write("_B");
						Output.Write(i.ToString());

						Output.Write(ExternalEvent.Actor.Id);
					}
					else
					{
						Output.Write(ExternalEvent.ActorName);
						Output.Write("\" as ");
						Output.Write(ExternalEvent.ActorName);
						Output.Write("_B");
						Output.Write(i.ToString());
						Output.Write(" <<");
						Output.Write(ExternalEvent.Actor.Id);
						Output.WriteLine(">>");

						Output.Write(ExternalEvent.ActorName);
					}

					Output.Write("_B");
					Output.Write(i.ToString());
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

							if (!string.IsNullOrEmpty(P.Variable))
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

	}
}
