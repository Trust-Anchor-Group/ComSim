using System;
using System.Collections.Generic;
using System.Text;

namespace TAG.Simulator.ObjectModel.Activities
{
	/// <summary>
	/// Represents the starting point of the activity.
	/// </summary>
	public class Start : ActivityNode 
	{
		/// <summary>
		/// Represents the starting point of the activity.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		public Start(ISimulationNode Parent)
			: base(Parent)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "Start";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent)
		{
			return new Start(Parent);
		}
	}
}
