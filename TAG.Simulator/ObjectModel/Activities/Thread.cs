using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Activities
{
	/// <summary>
	/// Represents a thread in parallel execution.
	/// </summary>
	public class Thread : ActivityNode
	{
		/// <summary>
		/// Represents a thread in parallel execution.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		public Thread(ISimulationNode Parent)
			: base(Parent)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "Thread";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent)
		{
			return new Thread(Parent);
		}

		/// <summary>
		/// Executes a node.
		/// </summary>
		/// <param name="Model">Current model</param>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>Next node of execution, if different from the default, otherwise null (for default).</returns>
		public override async Task<LinkedListNode<IActivityNode>> Execute(Model Model, Variables Variables)
		{
			await Activity.ExecuteActivity(Model, Variables, this.FirstNode);
			return null;
		}

	}
}
