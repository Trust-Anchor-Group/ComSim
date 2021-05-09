using System;
using System.Collections.Generic;
using System.IO;

namespace TAG.Simulator.ObjectModel.Activities
{
	/// <summary>
	/// Interface for elements taking <see cref="Condition"/> nodes.
	/// </summary>
	public interface IConditional
	{
		/// <summary>
		/// Register a conditional node.
		/// </summary>
		/// <param name="Node">Conditional node</param>
		void Register(IConditionNode Node);
	}
}
