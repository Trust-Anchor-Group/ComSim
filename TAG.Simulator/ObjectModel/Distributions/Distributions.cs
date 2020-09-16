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
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public Distributions(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "Distributions";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new Distributions(Parent, Model);
		}
	}
}
