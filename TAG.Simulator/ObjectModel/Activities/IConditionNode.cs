using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Activities
{
	/// <summary>
	/// Interface for condition nodes
	/// </summary>
	public interface IConditionNode : IActivityNode
	{
		/// <summary>
		/// If the node condition is true.
		/// </summary>
		/// <param name="Model">Current model</param>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>If embedded nodes are to be executed.</returns>
		bool IsTrue(Model Model, Variables Variables);
	}
}
