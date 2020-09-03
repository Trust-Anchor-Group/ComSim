using System;
using System.Threading.Tasks;
using TAG.Simulator.ObjectModel;

namespace TAG.Simulator
{
	/// <summary>
	/// Root node of a simulation model
	/// </summary>
	public class Model : SimulationNodeChildren
	{
		/// <summary>
		/// Root node of a simulation model
		/// </summary>
		public Model()
			: base()
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
		/// Creates a new instance of the node.
		/// </summary>
		/// <returns>New instance</returns>
		public override ISimulationNode Create()
		{
			return new Model();
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
