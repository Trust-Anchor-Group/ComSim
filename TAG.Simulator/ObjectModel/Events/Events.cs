using System;
using System.Collections.Generic;
using System.Text;

namespace TAG.Simulator.ObjectModel.Events
{
	/// <summary>
	/// Container for events.
	/// </summary>
	public class Events : SimulationNodeChildren
	{
		/// <summary>
		/// Container for events.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		public Events(ISimulationNode Parent)
			: base(Parent)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "Events";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent)
		{
			return new Events(Parent);
		}
	}
}
