using System;
using System.Collections.Generic;
using System.Text;

namespace TAG.Simulator.ObjectModel.Values
{
	/// <summary>
	/// Abstract base class for values
	/// </summary>
	public abstract class Value : SimulationNode
	{
		/// <summary>
		/// Abstract base class for values
		/// </summary>
		/// <param name="Parent">Parent node</param>
		public Value(ISimulationNode Parent)
			: base(Parent)
		{
		}
	}
}
