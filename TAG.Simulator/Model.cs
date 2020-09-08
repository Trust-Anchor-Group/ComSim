using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.Events;
using TAG.Simulator.ObjectModel;
using TAG.Simulator.ObjectModel.Actors;
using TAG.Simulator.ObjectModel.Distributions;
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
		private readonly Dictionary<string, string> keyValues = new Dictionary<string, string>();
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
				double dt;
				bool Result = true;

				while ((TP = DateTime.Now) <= this.end)
				{
					t1 = t2;
					t2 = Math.IEEERemainder((TP - this.start).TotalMilliseconds, this.timeCycleMs) / this.timeUnitMs;
					dt = t2 - t1;

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
	}
}
