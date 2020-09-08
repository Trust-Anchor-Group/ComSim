using System;
using System.Threading.Tasks;
using System.Xml;
using Waher.Content.Xml;

namespace TAG.Simulator.ObjectModel.Activities
{
	/// <summary>
	/// Executes multiple threads in parallel.
	/// </summary>
	public class Parallel : ActivityNode
	{
		/// <summary>
		/// Executes multiple threads in parallel.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		public Parallel(ISimulationNode Parent)
			: base(Parent)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "Parallel";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent)
		{
			return new Parallel(Parent);
		}

	}
}
