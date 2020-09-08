using System;
using System.Collections.Generic;
using System.Text;

namespace TAG.Simulator.ObjectModel.Activities
{
	/// <summary>
	/// Represents the stoping point of the activity.
	/// </summary>
	public class Stop : ActivityNode 
	{
		/// <summary>
		/// Represents the stoping point of the activity.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		public Stop(ISimulationNode Parent)
			: base(Parent)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "Stop";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent)
		{
			return new Stop(Parent);
		}
	}
}
