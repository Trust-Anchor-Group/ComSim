using System;

namespace TAG.Simulator.ObjectModel.Graphs
{
	/// <summary>
	/// Container for graphs.
	/// </summary>
	public class Graphs : SimulationNodeChildren
	{
		/// <summary>
		/// Container for graphs.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public Graphs(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "Graphs";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new Graphs(Parent, Model);
		}

	}
}
