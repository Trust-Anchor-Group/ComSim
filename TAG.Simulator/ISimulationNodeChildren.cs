using System;

namespace TAG.Simulator
{
	/// <summary>
	/// Basic interface for simulator nodes with child nodes.
	/// </summary>
	public interface ISimulationNodeChildren : ISimulationNode
	{
		/// <summary>
		/// Child nodes.
		/// </summary>
		ISimulationNode[] Children
		{
			get;
		}
	}
}
