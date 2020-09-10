using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Activities
{
	/// <summary>
	/// Executes multiple threads in parallel.
	/// </summary>
	public class Parallel : ActivityNode
	{
		/// <summary>
		/// Executes multiple threads in parallel.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		public Parallel(ISimulationNode Parent)
			: base(Parent)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "Parallel";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent)
		{
			return new Parallel(Parent);
		}

		/// <summary>
		/// Executes a node.
		/// </summary>
		/// <param name="Model">Current model</param>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>Next node of execution, if different from the default, otherwise null (for default).</returns>
		public override async Task<LinkedListNode<IActivityNode>> Execute(Model Model, Variables Variables)
		{
			Task[] Tasks = new Task[this.Count];
			LinkedListNode<IActivityNode> Loop = this.FirstNode;
			int i = 0;

			while (!(Loop is null))
				Tasks[i++] = Loop.Value.Execute(Model, Variables);

			await Task.WhenAll(Tasks);

			return null;
		}

	}
}
