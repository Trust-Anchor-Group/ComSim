using System;

namespace TAG.Simulator.ObjectModel.Graphs
{
	/// <summary>
	/// Interface for sources
	/// </summary>
	public interface ISource : ISimulationNode
	{
		/// <summary>
		/// Reference
		/// </summary>
		string Reference
		{
			get;
		}
	}
}
