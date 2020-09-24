using System;
using System.Threading.Tasks;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Values
{
	/// <summary>
	/// Interface for nodes holding a value node
	/// </summary>
	public interface IValueRecipient : ISimulationNodeChildren
	{
		/// <summary>
		/// Registers a value for the argument.
		/// </summary>
		/// <param name="Value">Value node</param>
		void Register(IValue Value);
	}
}
