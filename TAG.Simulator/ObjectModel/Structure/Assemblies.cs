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
		public Assemblies()
			: base()
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "Assemblies";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <returns>New instance</returns>
		public override ISimulationNode Create()
		{
			return new Assemblies();
		}
	}
}
