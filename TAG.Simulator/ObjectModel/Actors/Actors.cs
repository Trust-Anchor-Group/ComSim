using System;
using System.Collections.Generic;
using System.Text;

namespace TAG.Simulator.ObjectModel.Actors
{
	/// <summary>
	/// Container for actors.
	/// </summary>
	public class Actors : SimulationNodeChildren
	{
		/// <summary>
		/// Container for actors.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		public Actors(ISimulationNode Parent)
			: base(Parent)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "Actors";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent)
		{
			return new Actors(Parent);
		}
	}
}
