using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Xml;
using SkiaSharp;
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
using Waher.Events;
using TAG.Simulator.Statistics;
using System.Text;
using Waher.Script;
using Waher.Script.Objects;
using Waher.Script.Units;
using TAG.Simulator.ObjectModel.Graphs;

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
		private readonly Dictionary<string, IDistribution> distributions = new Dictionary<string, IDistribution>();
		private readonly Dictionary<string, IActor> actors = new Dictionary<string, IActor>();
		private readonly Dictionary<string, IActivity> activities = new Dictionary<string, IActivity>();
		private readonly Dictionary<string, LinkedListNode<IActivityNode>> activityNodes = new Dictionary<string, LinkedListNode<IActivityNode>>();
		private readonly Dictionary<string, IEvent> eventsWithId = new Dictionary<string, IEvent>();
		private readonly Dictionary<string, IGraph> graphsFor = new Dictionary<string, IGraph>();
		private readonly Dictionary<string, string> keyValues = new Dictionary<string, string>();
		private readonly Dictionary<string, IGraph> customSampleGraph = new Dictionary<string, IGraph>();
		private readonly Dictionary<string, IGraph> customCounterGraph = new Dictionary<string, IGraph>();
		private readonly Dictionary<string, IGraph> customExecutionsGraph = new Dictionary<string, IGraph>();
		private readonly Dictionary<string, IGraph> customExecutionTimesGraph = new Dictionary<string, IGraph>();
		private readonly LinkedList<ITimeTriggerEvent> timeTriggeredEvents = new LinkedList<ITimeTriggerEvent>();
		private readonly LinkedList<IGraph> graphs = new LinkedList<IGraph>();
		private readonly RandomNumberGenerator rnd = RandomNumberGenerator.Create();
		private readonly Variables variables = new Variables();
		private readonly Unit seconds = new Unit("s");
		private EventStatistics eventStatistics;
		private Buckets activityStartStatistics;
		private Buckets activityTimeStatistics;
		private Buckets counters;
		private Buckets samples;
		private TimeBase timeBase;
		private Duration bucketTime;
		private Duration timeUnit;
		private Duration timeCycle;
		private Duration duration;
		private DateTime start;
		private DateTime end;
		private string timeUnitStr;
		private string bucketTimeStr;
		private string snifferFolder;
		private string snifferTransformFileName;
		private double bucketTimeMs;
		private double timeUnitMs;
		private double timeCycleMs;
		private double timeCycleUnits;
		private bool executing;

		/// <summary>
		/// Root node of a simulation model
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public Model(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
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
		/// Time unit string
		/// </summary>
		public string TimeUnitStr => this.timeUnitStr;

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
		/// Time to collect events, for statistical purposes.
		/// </summary>
		public Duration BucketTime => this.bucketTime;

		/// <summary>
		/// Time to collect events, for statistical purposes.
		/// </summary>
		public string BucketTimeStr => this.bucketTimeStr;

		/// <summary>
		/// Bucket time, in milliseconds
		/// </summary>
		public double BucketTimeMs => this.bucketTimeMs;

		/// <summary>
		/// Start time
		/// </summary>
		public DateTime StartTime => this.start;

		/// <summary>
		/// End time
		/// </summary>
		public DateTime EndTime => this.end;

		/// <summary>
		/// Model variables.
		/// </summary>
		public Variables Variables => this.variables;

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
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new Model(Parent, Model);
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
			this.bucketTime = XML.Attribute(Definition, "bucketTime", Duration.FromMinutes(1));

			StringBuilder sb = new StringBuilder();
			ObjectModel.Values.Duration.ExportText(this.timeUnit, sb);
			this.timeUnitStr = sb.ToString();

			sb.Clear();
			ObjectModel.Values.Duration.ExportText(this.bucketTime, sb);
			this.bucketTimeStr = sb.ToString();

			return base.FromXml(Definition);
		}

		/// <summary>
		/// Initialized the node before simulation.
		/// </summary>
		public override async Task Initialize()
		{
			this.start = DateTime.Now;
			if (this.start.Millisecond != 0)
			{
				this.start = this.start.AddMilliseconds(1000 - this.start.Millisecond);

				while (DateTime.Now < this.start)
					await Task.Delay(1);
			}

			this.end = this.start + this.duration;

			this.bucketTimeMs = ((this.start + this.bucketTime) - this.start).TotalMilliseconds;
			this.timeUnitMs = ((this.start + this.timeUnit) - this.start).TotalMilliseconds;
			this.timeCycleMs = ((this.start + this.timeCycle) - this.start).TotalMilliseconds;
			this.timeCycleUnits = this.timeCycleMs / this.timeUnitMs;

			this.counters = new Buckets(this.start, this.bucketTime, "%ID%", "Count (/ " + this.bucketTimeStr + ")", this);
			this.samples = new Buckets(this.start, this.bucketTime, "%ID%", "Mean", this);
			this.activityStartStatistics = new Buckets(this.start, this.bucketTime, "%ID%", "Count (/ " + this.bucketTimeStr + ")", this);
			this.activityTimeStatistics = new Buckets(this.start, this.bucketTime, "Execution time of %ID%", "Mean execution time (s)", this);
			this.eventStatistics = new EventStatistics(this.start, this.bucketTime, this);
			Log.Register(this.eventStatistics);

			await base.Initialize();
		}

		/// <summary>
		/// Finalizes the node after simulation.
		/// </summary>
		public override Task Finalize()
		{
			Log.Unregister(this.eventStatistics);

			return base.Finalize();
		}

		/// <summary>
		/// Registers a distribution with the runtime environment of the model.
		/// </summary>
		/// <param name="Distribution">Distribution object.</param>
		public void Register(IDistribution Distribution)
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
		public bool TryGetDistribution(string Id, out IDistribution Distribution)
		{
			return this.distributions.TryGetValue(Id, out Distribution);
		}

		/// <summary>
		/// Registers a actor with the runtime environment of the model.
		/// </summary>
		/// <param name="Actor">Actor object.</param>
		public void Register(IActor Actor)
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
		public bool TryGetActor(string Id, out IActor Actor)
		{
			return this.actors.TryGetValue(Id, out Actor);
		}

		/// <summary>
		/// Registers a event with the runtime environment of the model.
		/// </summary>
		/// <param name="Event">Event object.</param>
		public void Register(IEvent Event)
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
		public bool TryGetEvent(string Id, out IEvent Event)
		{
			return this.eventsWithId.TryGetValue(Id, out Event);
		}

		/// <summary>
		/// Registers a activity with the runtime environment of the model.
		/// </summary>
		/// <param name="Activity">Activity object.</param>
		public void Register(IActivity Activity)
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
		public bool TryGetActivity(string Id, out IActivity Activity)
		{
			return this.activities.TryGetValue(Id, out Activity);
		}

		/// <summary>
		/// Registers a activity node with the runtime environment of the model.
		/// </summary>
		/// <param name="ActivityNode">ActivityNode object.</param>
		public void Register(LinkedListNode<IActivityNode> ActivityNode)
		{
			string Id = ActivityNode.Value.Id;

			if (!string.IsNullOrEmpty(Id))
			{
				if (this.activityNodes.ContainsKey(Id))
					throw new Exception("An activity node with ID " + Id + " already registered.");

				this.activityNodes[Id] = ActivityNode;
			}
		}

		/// <summary>
		/// Tries to get a registered activity node from the model.
		/// </summary>
		/// <param name="Id">ID of activity node.</param>
		/// <param name="ActivityNode">ActivityNode if found.</param>
		/// <returns>If an activity node was found.</returns>
		public bool TryGetActivityNode(string Id, out LinkedListNode<IActivityNode> ActivityNode)
		{
			return this.activityNodes.TryGetValue(Id, out ActivityNode);
		}

		/// <summary>
		/// Registers a graph with the runtime environment of the model.
		/// </summary>
		/// <param name="Graph">Graph object.</param>
		public void Register(IGraph Graph)
		{
			if (Graph is ICustomGraph CustomGraph)
			{
				string For = CustomGraph.For;
				if (!string.IsNullOrEmpty(For))
				{
					if (this.graphsFor.ContainsKey(For))
						throw new Exception("A graph for " + For + " already registered.");

					this.graphsFor[For] = Graph;
				}
			}

			if (Graph is IBucket Bucket)
				this.samples.Register(Bucket);

			this.graphs.AddLast(Graph);
		}

		/// <summary>
		/// Tries to get a registered graph from the model.
		/// </summary>
		/// <param name="For">ID of entity the graph would be for.</param>
		/// <param name="Graph">Graph if found.</param>
		/// <returns>If a graph was found.</returns>
		public bool TryGetGraph(string For, out IGraph Graph)
		{
			return this.graphsFor.TryGetValue(For, out Graph);
		}

		/// <summary>
		/// Tries to get a registered sample graph from the model.
		/// </summary>
		/// <param name="For">ID of entity the graph would be for.</param>
		/// <param name="Graph">Graph if found.</param>
		/// <returns>If a graph was found.</returns>
		public bool TryGetSampleGraph(string For, out IGraph Graph)
		{
			if (this.samples.TryGetBucket(For, out IBucket Bucket))
			{
				Graph = Bucket;
				return true;
			}
			else
			{
				Graph = null;
				return false;
			}
		}

		/// <summary>
		/// Tries to get a registered counter graph from the model.
		/// </summary>
		/// <param name="For">ID of entity the graph would be for.</param>
		/// <param name="Graph">Graph if found.</param>
		/// <returns>If a graph was found.</returns>
		public bool TryGetCounterGraph(string For, out IGraph Graph)
		{
			if (this.counters.TryGetBucket(For, out IBucket Bucket))
			{
				Graph = Bucket;
				return true;
			}
			else
			{
				Graph = null;
				return false;
			}
		}

		/// <summary>
		/// Tries to get a registered executuions graph from the model.
		/// </summary>
		/// <param name="For">ID of entity the graph would be for.</param>
		/// <param name="Graph">Graph if found.</param>
		/// <returns>If a graph was found.</returns>
		public bool TryGetExecutionsGraph(string For, out IGraph Graph)
		{
			if (this.activityStartStatistics.TryGetBucket(For, out IBucket Bucket))
			{
				Graph = Bucket;
				return true;
			}
			else
			{
				Graph = null;
				return false;
			}
		}

		/// <summary>
		/// Tries to get a registered executuion times graph from the model.
		/// </summary>
		/// <param name="For">ID of entity the graph would be for.</param>
		/// <param name="Graph">Graph if found.</param>
		/// <returns>If a graph was found.</returns>
		public bool TryGetExecutionTimesGraph(string For, out IGraph Graph)
		{
			if (this.activityTimeStatistics.TryGetBucket(For, out IBucket Bucket))
			{
				Graph = Bucket;
				return true;
			}
			else
			{
				Graph = null;
				return false;
			}
		}

		/// <summary>
		/// Registers a custom samples graph
		/// </summary>
		/// <param name="Id">ID of graph</param>
		/// <param name="Graph">Graph</param>
		public void RegisterCustomSampleGraph(string Id, IGraph Graph)
		{
			this.customSampleGraph[Id] = Graph;
		}

		/// <summary>
		/// Registers a custom counter graph
		/// </summary>
		/// <param name="Id">ID of graph</param>
		/// <param name="Graph">Graph</param>
		public void RegisterCustomCounterGraph(string Id, IGraph Graph)
		{
			this.customCounterGraph[Id] = Graph;
		}

		/// <summary>
		/// Registers a custom executions graph
		/// </summary>
		/// <param name="Id">ID of graph</param>
		/// <param name="Graph">Graph</param>
		public void RegisterCustomExecutionsGraph(string Id, IGraph Graph)
		{
			this.customExecutionsGraph[Id] = Graph;
		}

		/// <summary>
		/// Registers a custom execution times graph
		/// </summary>
		/// <param name="Id">ID of graph</param>
		/// <param name="Graph">Graph</param>
		public void RegisterCustomExecutionTimesGraph(string Id, IGraph Graph)
		{
			this.customExecutionTimesGraph[Id] = Graph;
		}

		/// <summary>
		/// Runs the simulation.
		/// </summary>
		/// <param name="Done">Task completion source, that can be set by external events.</param>
		/// <param name="EmitDots">If dots should be emitted to the console to mark the passage of time.</param>
		/// <returns>If simulation completed successfully.</returns>
		public async Task<bool> Run(TaskCompletionSource<bool> Done, bool EmitDots)
		{
			this.executing = true;

			Console.Out.WriteLine("Initializing...");
			await this.ForEach(async (Node) => await Node.Initialize(), true);

			try
			{
				Console.Out.WriteLine("Starting...");
				await this.ForEach(async (Node) => await Node.Start(), true);

				Console.Out.WriteLine("Running...");

				DateTime Prev = DateTime.Now;
				DateTime TP;
				double t1;
				double t2 = 0;
				double t;
				int NrCycles = 0;
				bool Result = true;
				bool Emitted = false;

				while ((TP = DateTime.Now) < this.end)
				{
					t = (TP - this.start).TotalMilliseconds;
					t1 = t2;
					t2 = Math.IEEERemainder(t, this.timeCycleMs);
					if (t2 < 0)
						t2 += this.timeCycleMs;
					t2 /= this.timeUnitMs;

					if (t2 < t1)
						NrCycles++;

					foreach (ITimeTriggerEvent Event in this.timeTriggeredEvents)
						Event.CheckTrigger(t1, t2, NrCycles);

					int Second = TP.Second;

					if (EmitDots && Prev.Second != Second)
					{
						Prev = TP;

						if (Second % 10 != 0)
							Console.Out.Write('.');
						else
						{
							if (Second == 0 && Emitted)
								Console.Out.WriteLine();

							Console.Out.Write((char)(((int)'0') + (Second / 10)));
						}

						Emitted = true;
					}

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
				this.executing = false;

				if (EmitDots)
					Console.Out.WriteLine('.');

				Console.Out.WriteLine("Finalizing...");
				await this.ForEach(async (Node) => await Node.Finalize(), true);
			}
		}

		/// <summary>
		/// Converts a <see cref="DateTime"/> to uncycled (linear) time coordinates.
		/// </summary>
		/// <param name="TP">Timepoint</param>
		/// <returns>Corresponding linear time coordinates.</returns>
		public double GetTimeCoordinates(DateTime TP)
		{
			return (TP - this.start).TotalMilliseconds / this.timeUnitMs;
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
		/// Generates a new random integer between 0 (inclusive) and <paramref name="MaxValueExclusive"/> (exclusive).
		/// </summary>
		/// <param name="MaxValueExclusive">Maximum value (exclusive)</param>
		/// <returns>Random integer value 0 - <paramref name="MaxValueExclusive"/>-1.</returns>
		public int GetRandomInteger(int MaxValueExclusive)
		{
			int Result;

			do
			{
				Result = (int)(MaxValueExclusive * GetRandomDouble());
			}
			while (Result >= MaxValueExclusive);

			return Result;
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
		/// <param name="Actor">Actor</param>
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
		public bool ExternalEvent(IActor Source, string Name, params KeyValuePair<string, object>[] Arguments)
		{
			if (Source.TryGetExternalEvent(Name, out IExternalEvent ExternalEvent))
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
		/// <param name="Tags">Meta-data tags related to the event.</param>
		public void IncActivityStartCount(string ActivityId, string SourceId, params KeyValuePair<string, object>[] Tags)
		{
			if (this.executing)
				this.activityStartStatistics.CountEvent(ActivityId);

			Log.Informational("Activity started.", ActivityId, SourceId, "ActivityStarted", Tags);
		}

		/// <summary>
		/// Called when an activity is finished.
		/// </summary>
		/// <param name="ActivityId">Activity ID</param>
		/// <param name="SourceId">ID of node activating activity.</param>
		/// <param name="ElapsedTime">Elapsed time.</param>
		/// <param name="Tags">Meta-data tags related to the event.</param>
		public void IncActivityFinishedCount(string ActivityId, string SourceId, TimeSpan ElapsedTime, params KeyValuePair<string, object>[] Tags)
		{
			if (this.executing)
			{
				PhysicalQuantity Q = new PhysicalQuantity(ElapsedTime.TotalSeconds, seconds);
				this.activityTimeStatistics.Sample(ActivityId, Q);
			}

			Log.Informational("Activity finished.", ActivityId, SourceId, "ActivityFinished", Tags);
		}

		/// <summary>
		/// Called when an activity is stopped, due to error.
		/// </summary>
		/// <param name="ActivityId">Activity ID</param>
		/// <param name="SourceId">ID of node activating activity.</param>
		/// <param name="ElapsedTime">Elapsed time.</param>
		/// <param name="ErrorMessage">Error message.</param>
		/// <param name="Tags">Meta-data tags related to the event.</param>
		public void IncActivityErrorCount(string ActivityId, string SourceId, string ErrorMessage, TimeSpan ElapsedTime, params KeyValuePair<string, object>[] Tags)
		{
			if (this.executing)
			{
				PhysicalQuantity Q = new PhysicalQuantity(ElapsedTime.TotalSeconds, seconds);
				this.activityTimeStatistics.Sample(ActivityId, Q);
			}

			Log.Error("Activity stopped due to error: " + ErrorMessage, ActivityId, SourceId, "ActivityError", Tags);
		}

		/// <summary>
		/// Counts an event.
		/// </summary>
		/// <param name="CounterName">Counter name</param>
		public void CountEvent(string CounterName)
		{
			if (this.executing)
				this.counters.CountEvent(CounterName);
		}

		/// <summary>
		/// Increments a counter.
		/// </summary>
		/// <param name="CounterName">Counter name</param>
		public void IncrementCounter(string CounterName)
		{
			if (this.executing)
				this.samples.IncrementCounter(CounterName);
		}

		/// <summary>
		/// Decrements a counter.
		/// </summary>
		/// <param name="CounterName">Counter name</param>
		public void DecrementCounter(string CounterName)
		{
			if (this.executing)
				this.samples.DecrementCounter(CounterName);
		}

		/// <summary>
		/// Records a sample
		/// </summary>
		/// <param name="CounterName">Counter name</param>
		/// <param name="Value">Value</param>
		public void Sample(string CounterName, double Value)
		{
			if (this.executing)
				this.samples.Sample(CounterName, Value);
		}

		/// <summary>
		/// Records a sample
		/// </summary>
		/// <param name="CounterName">Counter name</param>
		/// <param name="Value">Value</param>
		public void Sample(string CounterName, PhysicalQuantity Value)
		{
			if (this.executing)
				this.samples.Sample(CounterName, Value);
		}

		/// <summary>
		/// Exports Markdown
		/// </summary>
		/// <param name="Output">Output node</param>
		public override async Task ExportMarkdown(StreamWriter Output)
		{
			this.activitiesIntroduction = false;

			await base.ExportMarkdown(Output);

			if (this.counters.Count > 0)
			{
				Output.WriteLine("Counters");
				Output.WriteLine("===========");
				Output.WriteLine();

				CountTable Table = this.counters.GetTotalCountTable();
				Table.ExportTableGraph(Output, "Counters");

				this.counters.ExportCountHistoryGraph("Counters", null, Output, this, null);
			}

			string s;

			if (this.samples.Count > 0)
			{
				Output.WriteLine("Measurements");
				Output.WriteLine("===============");
				Output.WriteLine();

				foreach (string ID in this.samples.IDs)
				{
					if (this.customSampleGraph.ContainsKey(ID))
						continue;

					if (!this.graphsFor.TryGetValue(ID, out IGraph Graph))
					{
						if (this.samples.TryGetBucket(ID, out IBucket Bucket))
							Graph = Bucket;
						else
							continue;
					}

					if (!string.IsNullOrEmpty(s = Graph.Header))
					{
						Output.WriteLine(s);
						Output.WriteLine(new string('-', s.Length + 3));
						Output.WriteLine();
					}

					Graph.ExportGraph(Output);
				}
			}

			foreach (IGraph Graph in this.graphs)
			{
				if (Graph is ICustomGraph CustomGraph && !string.IsNullOrEmpty(CustomGraph.For))
					continue;

				if (!string.IsNullOrEmpty(s = Graph.Header))
				{
					Output.WriteLine(s);
					Output.WriteLine(new string('-', s.Length + 3));
					Output.WriteLine();
				}

				Graph.ExportGraph(Output);
			}

			this.eventStatistics.ExportMarkdown(Output, this);
		}

		internal void ExportActivitiesIntroduction(StreamWriter Output)
		{
			if (this.activitiesIntroduction)
				return;

			this.activitiesIntroduction = true;

			Output.WriteLine("Activities");
			Output.WriteLine("=============");
			Output.WriteLine();

			if (this.activityStartStatistics.Count > 0)
			{
				string[] Order = this.ActivityOrder();
				CountTable Table = this.activityStartStatistics.GetTotalCountTable(Order);

				Table.ExportTableGraph(Output, "Total activity counts");

				this.activityStartStatistics.ExportCountHistoryGraph("Total Activities", Order, Output, this, null);
			}
		}

		internal void ExportActivityCharts(string ActivityId, StreamWriter Output, IEvent Event)
		{
			if (this.activityStartStatistics.TryGetBucket(ActivityId, out IBucket _))
			{
				string[] Order = this.ActivityOrder();
				int i = -1;
				int Index = 0;

				foreach (string Id in Order)
				{
					if (!this.activityStartStatistics.TryGetBucket(Id, out IBucket _))
						continue;

					if (Id == ActivityId)
						i = Index;

					Index++;
				}

				if (i >= 0)
				{
					SKColor[] Palette = CreatePalette(Index);

					this.activityStartStatistics.ExportCountHistoryGraph("Executions of " + ActivityId,
						new string[] { ActivityId }, Output, this, Event, new SKColor[] { Palette[i] });
				}
			}

			if (this.activityTimeStatistics.TryGetBucket(ActivityId, out IBucket Bucket))
				Bucket.ExportGraph(Output);
		}

		private string[] ActivityOrder()
		{
			foreach (ISimulationNode Node in this.Children)
			{
				if (Node is Activities Activities)
					return Activities.ActivityOrder();
			}

			return new string[0];
		}

		private bool activitiesIntroduction;

		/// <summary>
		/// Exports XML
		/// </summary>
		/// <param name="Output">Output node</param>
		public override async Task ExportXml(XmlWriter Output)
		{
			await base.ExportXml(Output);

			if (this.activityStartStatistics.Count > 0)
				this.activityStartStatistics.ExportXml(Output, "ActivityStarts", "ActivityStart");

			if (this.activityTimeStatistics.Count > 0)
				this.activityTimeStatistics.ExportXml(Output, "ActivityTimes", "ActivityTime");

			if (this.counters.Count > 0)
				this.counters.ExportXml(Output, "Counters", "Counter");

			if (this.samples.Count > 0)
				this.samples.ExportXml(Output, "Samples", "Series");

			this.eventStatistics.ExportXml(Output);
		}

		/// <summary>
		/// Creates a palette for graphs.
		/// </summary>
		/// <param name="N">Number of colors in palette.</param>
		/// <returns>Palette</returns>
		public static SKColor[] CreatePalette(int N)
		{
			SKColor[] Result = new SKColor[N];
			double d = 360.0 / Math.Max(N, 12);
			int i;

			for (i = 0; i < N; i++)
				Result[i] = SKColor.FromHsl((float)(d * i), 100, 75);

			return Result;
		}

		/// <summary>
		/// Gets a collection of variables for a new event.
		/// </summary>
		/// <returns>Variables collection</returns>
		public Variables GetEventVariables()
		{
			return new EventVariables(this.variables, this);
		}

		/// <summary>
		/// Gets the string representation of a color.
		/// </summary>
		/// <param name="Color">Color</param>
		/// <returns>String representation</returns>
		public static string ToString(SKColor Color)
		{
			StringBuilder sb = new StringBuilder();

			sb.Append('#');
			sb.Append(Color.Red.ToString("X2"));
			sb.Append(Color.Green.ToString("X2"));
			sb.Append(Color.Blue.ToString("X2"));

			return sb.ToString();
		}
	}
}
