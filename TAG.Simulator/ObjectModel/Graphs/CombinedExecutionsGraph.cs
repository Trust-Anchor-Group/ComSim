﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TAG.Simulator.ObjectModel.Graphs
{
	/// <summary>
	/// Combines execution count graphs
	/// </summary>
	public class CombinedExecutionsGraph : CombinedGraph
	{
		/// <summary>
		/// Combines execution count graphs
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public CombinedExecutionsGraph(ISimulationNode Parent, Model Model) 
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "CombinedExecutionsGraph";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new CombinedExecutionsGraph(Parent, Model);
		}

		/// <summary>
		/// Gets a graph from its reference.
		/// </summary>
		/// <param name="Reference">Source reference.</param>
		/// <returns>Graph object if found, null otherwise.</returns>
		public override IGraph GetGraph(string Reference)
		{
			if (this.Model.TryGetExecutionsGraph(Reference, out IGraph Graph))
				return Graph;
			else
				return null;
		}

		/// <summary>
		/// Registers a source.
		/// </summary>
		/// <param name="Source">Source node</param>
		public override void Register(ISource Source)
		{
			base.Register(Source);
			this.Model.RegisterCustomExecutionsGraph(Source.Reference, this);
		}
	}
}
