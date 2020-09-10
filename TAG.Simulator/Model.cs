using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.Events;
using TAG.Simulator.ObjectModel;
using TAG.Simulator.ObjectModel.Actors;
using TAG.Simulator.ObjectModel.Activities;
using TAG.Simulator.ObjectModel.Distributions;
using TAG.Simulator.ObjectModel.Events;
using Waher.Content;
using Waher.Content.Xml;
using Waher.Networking.Sniffers;
using Waher.Runtime.Settings;

namespace TAG.Simulator
{
	/// <summary>
	/// Base of simulation time
	/// </summary>
	public enum TimeBase
	{
		/// <summary>
		/// 00:00:00 equals start of simulation.
		/// </summary>
		StartOfSimulation,

		/// <summary>
		/// Time/Date equals computer time/date.
		/// </summary>
		ComputerClock
	}

	/// <summary>
	/// Root node of a simulation model
	/// </summary>
	public class Model : SimulationNodeChildren
	{
		private readonly Dictionary<string, Distribution> distributions = new Dictionary<string, Distribution>();
		private readonly Dictionary<string, Actor> actors = new Dictionary<string, Actor>();
		private readonly Dictionary<string, Activity> activities = new Dictionary<string, Activity>();
		private readonly Dictionary<string, ActivityNode> activityNodes = new Dictionary<string, ActivityNode>();
		private readonly Dictionary<string, Event> eventsWithId = new Dictionary<string, Event>();
		private readonly Dictionary<string, string> keyValues = new Dictionary<string, string>();
		private readonly LinkedList<ITimeTriggerEvent> timeTriggeredEvents = new LinkedList<ITimeTriggerEvent>();
		private readonly RandomNumberGenerator rnd = RandomNumberGenerator.Create();
		private TimeBase timeBase;
		private Duration timeUnit;
		private Duration timeCycle;
		private Duration duration;
		private DateTime start;
		private DateTime end;
		private string snifferFolder;
		private string snifferTransformFileName;
		private double timeUnitMs;
		private double timeCycleMs;
		private double timeCycleUnits;

		/// <summary>
		/// Root node of a simulation model
		/// </summary>
		/// <param name="Parent">Parent node</param>
		public Model(ISimulationNode Parent)
			: base(Parent)
		{
		}

		/// <summary>
		/// http://trustanchorgroup.com/Schema/ComSim.xsd
		/// </summary>
		public const string ComSimNamespace = "http://trustanchorgroup.com/Schema/ComSim.xsd";

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "Model";

		/// <summary>
		/// Base of simulation time
		/// </summary>
		public TimeBase TimeBase => this.timeBase;

		/// <summary>
		/// Time unit
		/// </summary>
		public Duration TimeUnit => this.timeUnit;

		/// <summary>
		/// Time unit, in milliseconds
		/// </summary>
		public double TimeUnitMs => this.timeUnitMs;

		/// <summary>
		/// Time cycle
		/// </summary>
		public Duration TimeCycle => this.timeCycle;

		/// <summary>
		/// Time cycle, in milliseconds
		/// </summary>
		public double TimeCycleMs => this.timeCycleMs;

		/// <summary>
		/// Time cycle, in number of <see cref="TimeUnit"/>.
		/// </summary>
		public double TimeCycleUnits => this.timeCycleUnits;

		/// <summary>
		/// Simulation duration
		/// </summary>
		public Duration Duration => this.duration;

		/// <summary>
		/// Folder used for sniffer output.
		/// </summary>
		public string SnifferFolder
		{
			get => this.snifferFolder;
			set => this.snifferFolder = value;
		}

		/// <summary>
		/// Sniffer XSLT file to use to transform sniffer output.
		/// </summary>
		public string SnifferTransformFileName
		{
			get => this.snifferTransformFileName;
			set => this.snifferTransformFileName = value;
		}

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent)
		{
			return new Model(Parent);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.timeBase = (TimeBase)XML.Attribute(Definition, "timeBase", TimeBase.ComputerClock);
			this.timeUnit = XML.Attribute(Definition, "timeUnit", Duration.FromHours(1));
			this.timeCycle = XML.Attribute(Definition, "timeCycle", Duration.FromDays(1));
			this.duration = XML.Attribute(Definition, "duration", Duration.FromDays(1));

			return base.FromXml(Definition);
		}

		/// <summary>
		/// Initialized the node before simulation.
		/// </summary>
		/// <param name="Model">Model being executed.</param>
		public override Task Initialize(Model Model)
		{
			this.start = DateTime.Now;
			this.end = this.start + this.duration;

			this.timeUnitMs = ((this.start + this.timeUnit) - this.start).TotalMilliseconds;
			this.timeCycleMs = ((this.start + this.timeCycle) - this.start).TotalMilliseconds;
			this.timeCycleUnits = this.timeCycleMs / this.timeUnitMs;

			return base.Initialize(Model);
		}

		/// <summary>
		/// Registers a distribution with the runtime environment of the model.
		/// </summary>
		/// <param name="Distribution">Distribution object.</param>
		public void Register(Distribution Distribution)
		{
			if (this.distributions.ContainsKey(Distribution.Id))
				throw new Exception("A distribution with ID " + Distribution.Id + " already registered.");

			this.distributions[Distribution.Id] = Distribution;
		}

		/// <summary>
		/// Tries to get a registered distribution from the model.
		/// </summary>
		/// <param name="Id">ID of distribution.</param>
		/// <param name="Distribution">Distribution if found.</param>
		/// <returns>If a distribution was found.</returns>
		public bool TryGetDistribution(string Id, out Distribution Distribution)
		{
			return this.distributions.TryGetValue(Id, out Distribution);
		}

		/// <summary>
		/// Registers a actor with the runtime environment of the model.
		/// </summary>
		/// <param name="Actor">Actor object.</param>
		public void Register(Actor Actor)
		{
			if (this.actors.ContainsKey(Actor.Id))
				throw new Exception("An actor with ID " + Actor.Id + " already registered.");

			this.actors[Actor.Id] = Actor;
		}

		/// <summary>
		/// Tries to get a registered actor from the model.
		/// </summary>
		/// <param name="Id">ID of actor.</param>
		/// <param name="Actor">Actor if found.</param>
		/// <returns>If an actor was found.</returns>
		public bool TryGetActor(string Id, out Actor Actor)
		{
			return this.actors.TryGetValue(Id, out Actor);
		}

		/// <summary>
		/// Registers a event with the runtime environment of the model.
		/// </summary>
		/// <param name="Event">Event object.</param>
		public void Register(Event Event)
		{
			if (!string.IsNullOrEmpty(Event.Id))
			{
				if (this.eventsWithId.ContainsKey(Event.Id))
					throw new Exception("An event with ID " + Event.Id + " already registered.");

				this.eventsWithId[Event.Id] = Event;
			}

			if (Event is ITimeTriggerEvent TimeTriggeredEvent)
				this.timeTriggeredEvents.AddLast(TimeTriggeredEvent);
		}

		/// <summary>
		/// Tries to get a registered event from the model.
		/// </summary>
		/// <param name="Id">ID of event.</param>
		/// <param name="Event">Event if found.</param>
		/// <returns>If an event was found.</returns>
		public bool TryGetEvent(string Id, out Event Event)
		{
			return this.eventsWithId.TryGetValue(Id, out Event);
		}

		/// <summary>
		/// Registers a activity with the runtime environment of the model.
		/// </summary>
		/// <param name="Activity">Activity object.</param>
		public void Register(Activity Activity)
		{
			if (this.activities.ContainsKey(Activity.Id))
				throw new Exception("An activity with ID " + Activity.Id + " already registered.");

			this.activities[Activity.Id] = Activity;
		}

		/// <summary>
		/// Tries to get a registered activity from the model.
		/// </summary>
		/// <param name="Id">ID of activity.</param>
		/// <param name="Activity">Activity if found.</param>
		/// <returns>If an activity was found.</returns>
		public bool TryGetActivity(string Id, out Activity Activity)
		{
			return this.activities.TryGetValue(Id, out Activity);
		}

		/// <summary>
		/// Registers a activity node with the runtime environment of the model.
		/// </summary>
		/// <param name="ActivityNode">ActivityNode object.</param>
		public void Register(ActivityNode ActivityNode)
		{
			if (!string.IsNullOrEmpty(ActivityNode.Id))
			{
				if (this.activityNodes.ContainsKey(ActivityNode.Id))
					throw new Exception("An activity node with ID " + ActivityNode.Id + " already registered.");

				this.activityNodes[ActivityNode.Id] = ActivityNode;
			}
		}

		/// <summary>
		/// Tries to get a registered activity node from the model.
		/// </summary>
		/// <param name="Id">ID of activity node.</param>
		/// <param name="ActivityNode">ActivityNode if found.</param>
		/// <returns>If an activity node was found.</returns>
		public bool TryGetActivityNode(string Id, out ActivityNode ActivityNode)
		{
			return this.activityNodes.TryGetValue(Id, out ActivityNode);
		}

		/// <summary>
		/// Runs the simulation.
		/// </summary>
		/// <param name="Done">Task completion source, that can be set by external events.</param>
		/// <returns>If simulation completed successfully.</returns>
		public async Task<bool> Run(TaskCompletionSource<bool> Done)
		{
			Console.Out.WriteLine("Initializing...");
			await this.ForEach(async (Node) => await Node.Initialize(this), true);

			try
			{
				Console.Out.WriteLine("Starting...");
				await this.ForEach(async (Node) => await Node.Start(), true);

				Console.Out.WriteLine("Running...");

				DateTime TP;
				double t1;
				double t2 = 0;
				double t;
				int NrCycles = 0;
				bool Result = true;

				while ((TP = DateTime.Now) <= this.end)
				{
					t = (TP - this.start).TotalMilliseconds;
					t1 = t2;
					t2 = Math.IEEERemainder(t, this.timeCycleMs) / this.timeUnitMs;
					if (t2 < t1)
						NrCycles++;

					foreach (ITimeTriggerEvent Event in this.timeTriggeredEvents)
						Event.CheckTrigger(t1, t2, NrCycles);

					if (Task.WaitAny(Done.Task, Task.Delay(1)) == 0)
					{
						Result = false;
						break;
					}
				}

				return Result;
			}
			finally
			{
				Console.Out.WriteLine("Finalizing...");
				await this.ForEach(async (Node) => await Node.Finalize(), true);
			}
		}

		/// <summary>
		/// Gets an array of random bytes.
		/// </summary>
		/// <param name="NrBytes">Number of random bytes to generate.</param>
		/// <returns>Random bytes.</returns>
		public byte[] GetRandomBytes(int NrBytes)
		{
			byte[] Result = new byte[NrBytes];

			lock (this.rnd)
			{
				this.rnd.GetBytes(Result);
			}

			return Result;
		}

		/// <summary>
		/// Generates a new floating-point value between 0 and 1, using a cryptographic random number generator.
		/// </summary>
		/// <returns>Random number.</returns>
		public double GetRandomDouble()
		{
			byte[] b = this.GetRandomBytes(8);
			double d = BitConverter.ToUInt64(b, 0);
			d /= ulong.MaxValue;

			return d;
		}

		/// <summary>
		/// Gets a key from the database. If it does not exist, it prompts the user for input.
		/// </summary>
		/// <param name="KeyName">Name of key.</param>
		/// <returns>Value of key.</returns>
		public async Task<string> GetKey(string KeyName)
		{
			string Result;

			lock (this.keyValues)
			{
				if (this.keyValues.TryGetValue(KeyName, out Result))
					return Result;
			}

			Result = await RuntimeSettings.GetAsync("KEY." + KeyName, string.Empty);

			if (string.IsNullOrEmpty(Result))
			{
				KeyEventArgs e = new KeyEventArgs(KeyName);
				this.OnGetKey?.Invoke(this, e);

				if (string.IsNullOrEmpty(e.Value))
					throw new Exception("Unable to get value for key " + KeyName);

				Result = e.Value;
				await RuntimeSettings.SetAsync("KEY." + KeyName, Result);
			}

			lock (this.keyValues)
			{
				this.keyValues[KeyName] = Result;
			}

			return Result;
		}

		/// <summary>
		/// Event raised when the model needs a key from the system.
		/// </summary>
		public event KeyEventHandler OnGetKey;

		/// <summary>
		/// Gets a sniffer, if sniffer output is desired.
		/// </summary>
		/// <param name="Actor"></param>
		/// <returns>Sniffer, if any, null otherwise.</returns>
		public ISniffer GetSniffer(string Actor)
		{
			if (string.IsNullOrEmpty(this.snifferFolder))
				return null;
			else
				return new XmlFileSniffer(Path.Combine(this.snifferFolder, Actor + ".xml"), this.snifferTransformFileName, BinaryPresentationMethod.Base64);
		}

		/// <summary>
		/// Method called when an external event has been received.
		/// </summary>
		/// <param name="Source">Actor receiving the event.</param>
		/// <param name="Name">Name of event.</param>
		/// <param name="Arguments">Event arguments.</param>
		/// <returns>If event was handled</returns>
		public bool ExternalEvent(IActorNode Source, string Name, params KeyValuePair<string, object>[] Arguments)
		{
			if (Source.TryGetExternalEvent(Name, out ExternalEvent ExternalEvent))
			{
				ExternalEvent.Trigger(Source, Arguments);
				return true;
			}
			else
				return false;
		}

		/// <summary>
		/// Called when an activity is started.
		/// </summary>
		/// <param name="ActivityId">Activity ID</param>
		/// <param name="SourceId">ID of node activating activity.</param>
		public void IncActivityStartCount(string ActivityId, string SourceId)
		{
			Waher.Events.Log.Informational("Activity started.", ActivityId, SourceId, "ActivityStarted");
		}

		/// <summary>
		/// Called when an activity is finished.
		/// </summary>
		/// <param name="ActivityId">Activity ID</param>
		/// <param name="SourceId">ID of node activating activity.</param>
		public void IncActivityFinishedCount(string ActivityId, string SourceId)
		{
			Waher.Events.Log.Informational("Activity finished.", ActivityId, SourceId, "ActivityFinished");
		}

		/// <summary>
		/// Called when an activity is stopped, due to error.
		/// </summary>
		/// <param name="ActivityId">Activity ID</param>
		/// <param name="SourceId">ID of node activating activity.</param>
		/// <param name="ErrorMessage">Error message.</param>
		public void IncActivityErrorCount(string ActivityId, string SourceId, string ErrorMessage)
		{
			Waher.Events.Log.Error("Activity stopped due to error: " + ErrorMessage, ActivityId, SourceId, "ActivityError");
		}

	}
}
