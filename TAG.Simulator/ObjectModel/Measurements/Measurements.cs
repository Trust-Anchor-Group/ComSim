using System;

namespace TAG.Simulator.ObjectModel.Measurements
{
	/// <summary>
	/// Container for measurement configuration.
	/// </summary>
	public class Measurements : SimulationNodeChildren
	{
		/// <summary>
		/// Container for measurement configuration.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public Measurements(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(Measurements);

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new Measurements(Parent, Model);
		}

	}
}
