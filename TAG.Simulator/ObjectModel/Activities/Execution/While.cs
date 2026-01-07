using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TAG.Simulator.ObjectModel.Activities.Conditions;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Activities.Execution
{
	/// <summary>
	/// While Loop construct
	/// </summary>
	public class While : ActivityNode, IConditional
	{
		private IConditionNode condition;

		/// <summary>
		/// While Loop construct
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public While(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(While);

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new While(Parent, Model);
		}

		/// <summary>
		/// Register a conditional node.
		/// </summary>
		/// <param name="Node">Conditional node</param>
		public void Register(IConditionNode Node)
		{
			if (this.condition is null)
				this.condition = Node;
			else
				throw new Exception("While construct already has a condition node defined.");
		}

		/// <summary>
		/// Executes a node.
		/// </summary>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>Next node of execution, if different from the default, otherwise null (for default).</returns>
		public override async Task<LinkedListNode<IActivityNode>> Execute(Variables Variables)
		{
			LinkedListNode<IActivityNode> Result = null;

			if (!(this.condition is null))
			{
				while (await this.condition.IsTrue(Variables))
					Result = await this.condition.Execute(Variables);
			}

			return Result;
		}

		/// <summary>
		/// Exports PlantUML
		/// </summary>
		/// <param name="Output">Output</param>
		/// <param name="Indentation">Number of tabs to indent.</param>
		/// <param name="QuoteChar">Quote character.</param>
		public override void ExportPlantUml(StreamWriter Output, int Indentation, char QuoteChar)
		{
			this.condition?.ExportPlantUml(Output, Indentation, true, QuoteChar);
		}

	}
}
