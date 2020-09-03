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
		public Actors()
			: base()
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "Actors";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <returns>New instance</returns>
		public override ISimulationNode Create()
		{
			return new Actors();
		}
	}
}
