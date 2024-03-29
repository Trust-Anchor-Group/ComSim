﻿using System;

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
		/// <param name="Model">Model in which the node is defined.</param>
		public Assemblies(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(Assemblies);

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new Assemblies(Parent, Model);
		}
	}
}
