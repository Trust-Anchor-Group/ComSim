using System;
using System.Collections.Generic;
using System.Text;

namespace TAG.Simulator.ObjectModel.Activities
{
	/// <summary>
	/// Container for activities.
	/// </summary>
	public class Activities : SimulationNodeChildren
	{
		/// <summary>
		/// Container for activities.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		public Activities(ISimulationNode Parent)
			: base(Parent)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "Activities";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent)
		{
			return new Activities(Parent);
		}
	}
}
