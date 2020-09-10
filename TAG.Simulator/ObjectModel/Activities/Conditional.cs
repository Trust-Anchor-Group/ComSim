using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Activities
{
	/// <summary>
	/// Conditional execution in an activity.
	/// </summary>
	public class Conditional : ActivityNode
	{
		private readonly LinkedList<IConditionNode> conditions = new LinkedList<IConditionNode>();

		/// <summary>
		/// Conditional execution in an activity.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		public Conditional(ISimulationNode Parent)
			: base(Parent)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "Conditional";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent)
		{
			return new Conditional(Parent);
		}

		/// <summary>
		/// Register a conditional node.
		/// </summary>
		/// <param name="Node">Conditional node</param>
		public void Register(IConditionNode Node)
		{
			this.conditions.AddLast(Node);
		}

		/// <summary>
		/// Executes a node.
		/// </summary>
		/// <param name="Model">Current model</param>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>Next node of execution, if different from the default, otherwise null (for default).</returns>
		public override async Task<LinkedListNode<IActivityNode>> Execute(Model Model, Variables Variables)
		{
			foreach (IConditionNode Condition in this.conditions)
			{
				if (Condition.IsTrue(Model, Variables))
					return await Condition.Execute(Model, Variables);
			}

			return null;
		}

	}
}
