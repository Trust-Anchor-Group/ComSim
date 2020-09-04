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
		private DateTime start;

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
			this.timeBase = (TimeBase)XML.Attribute(Definition, "timeBase", TimeBase.StartOfSimulation);
			this.timeUnit = XML.Attribute(Definition, "timeUnit", Duration.FromHours(1));

			return base.FromXml(Definition);
		}

		/// <summary>
		/// Initialized the node before simulation.
		/// </summary>
		public override Task Initialize()
		{
			this.start = DateTime.Now;
			return base.Initialize();
		}

		/// <summary>
		/// Runs the simulation.
		/// </summary>
		public async Task Run()
		{
			Console.Out.WriteLine("Initializing...");
			await this.ForEach(async (Node) => await Node.Initialize(), false);
			
			Console.Out.WriteLine("Starting...");
			await this.ForEach(async (Node) => await Node.Start(), false);

			// TODO

			Console.Out.WriteLine("Finalizing...");
			await this.ForEach(async (Node) => await Node.Finalize(), false);
		}
	}
}
