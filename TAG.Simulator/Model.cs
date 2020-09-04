using System;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel;
using Waher.Content;
using Waher.Content.Xml;

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
		private TimeBase timeBase;
		private Duration timeUnit;
		private Duration timeCycle;
		private Duration duration;
		private DateTime start;
		private DateTime end;
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
		/// Runs the simulation.
		/// </summary>
		/// <param name="Done">Task completion source, that can be set by external events.</param>
		/// <returns>If simulation completed successfully.</returns>
		public async Task<bool> Run(TaskCompletionSource<bool> Done)
		{
			Console.Out.WriteLine("Initializing...");
			await this.ForEach(async (Node) => await Node.Initialize(this), false);

			Console.Out.WriteLine("Starting...");
			await this.ForEach(async (Node) => await Node.Start(), false);

			Console.Out.WriteLine("Running...");

			DateTime TP;
			double t1;
			double t2 = 0;
			double dt;
			bool Result = true;

			while ((TP = DateTime.Now) <= this.end)
			{
				t1 = t2;
				t2 = Math.IEEERemainder((TP - this.start).TotalMilliseconds, this.timeCycleMs);
				dt = t2 - t1;

				if (Task.WaitAny(Done.Task, Task.Delay(1)) == 0)
				{
					Result = false;
					break;
				}
			}

			Console.Out.WriteLine("Finalizing...");
			await this.ForEach(async (Node) => await Node.Finalize(), false);

			return Result;
		}
	}
}
