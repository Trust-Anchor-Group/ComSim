namespace TAG.Simulator.ObjectModel.Graphs
{
	/// <summary>
	/// Combines counter graphs
	/// </summary>
	public class CombinedCounterGraph : CombinedGraph
	{
		/// <summary>
		/// Combines counter graphs
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public CombinedCounterGraph(ISimulationNode Parent, Model Model) 
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(CombinedCounterGraph);

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new CombinedCounterGraph(Parent, Model);
		}

		/// <summary>
		/// Gets a graph from its reference.
		/// </summary>
		/// <param name="Reference">Source reference.</param>
		/// <returns>Graph object if found, null otherwise.</returns>
		public override IGraph GetGraph(string Reference)
		{
			if (this.Model.TryGetCounterGraph(Reference, out IGraph Graph))
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
			this.Model.RegisterCustomCounterGraph(Source.Reference, this);
		}
	}
}
