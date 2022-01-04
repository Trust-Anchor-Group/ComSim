using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Activities
{
	/// <summary>
	/// Conditional execution in an activity.
	/// </summary>
	public class Conditional : ActivityNode, IConditional
	{
		private readonly LinkedList<IConditionNode> conditions = new LinkedList<IConditionNode>();

		/// <summary>
		/// Conditional execution in an activity.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public Conditional(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
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
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new Conditional(Parent, Model);
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
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>Next node of execution, if different from the default, otherwise null (for default).</returns>
		public override async Task<LinkedListNode<IActivityNode>> Execute(Variables Variables)
		{
			foreach (IConditionNode Condition in this.conditions)
			{
				if (await Condition.IsTrue(Variables))
					return await Condition.Execute(Variables);
			}

			return null;
		}

		/// <summary>
		/// Exports PlantUML
		/// </summary>
		/// <param name="Output">Output</param>
		/// <param name="Indentation">Number of tabs to indent.</param>
		/// <param name="QuoteChar">Quote character.</param>
		public override void ExportPlantUml(StreamWriter Output, int Indentation, char QuoteChar)
		{
			bool First = true;

			foreach (IConditionNode Node in this.conditions)
			{
				Node.ExportPlantUml(Output, Indentation, First, QuoteChar);
				First = false;
			}

			if (!First)
			{
				Indent(Output, Indentation);
				Output.WriteLine("endif");
			}
		}

	}
}
