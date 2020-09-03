using System;
using System.Collections.Generic;
using System.Text;

namespace TAG.Simulator.ObjectModel.Distributions
{
	/// <summary>
	/// Container for distributions.
	/// </summary>
	public class Distributions : SimulationNodeChildren
	{
		/// <summary>
		/// Container for distributions.
		/// </summary>
		public Distributions()
			: base()
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "Distributions";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <returns>New instance</returns>
		public override ISimulationNode Create()
		{
			return new Distributions();
		}
	}
}
