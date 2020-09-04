using System;
using System.Collections.Generic;
using System.Text;

namespace TAG.Simulator.ObjectModel.Structure
{
	/// <summary>
	/// Container for assemblies.
	/// </summary>
	public class Assemblies : SimulationNodeChildren
	{
		/// <summary>
		/// Container for assemblies.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		public Assemblies(ISimulationNode Parent)
			: base(Parent)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "Assemblies";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent)
		{
			return new Assemblies(Parent);
		}
	}
}
