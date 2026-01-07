using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Activities.Conditions
{
	/// <summary>
	/// Represents a condition that is always true.
	/// </summary>
	public class Otherwise : ActivityNode, IConditionNode
	{
		/// <summary>
		/// Represents a condition that is always true.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public Otherwise(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(Otherwise);

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new Otherwise(Parent, Model);
		}

		/// <summary>
		/// Initialized the node before simulation.
		/// </summary>
		public override Task Initialize()
		{
			if (this.Parent is Conditional Conditional)
				Conditional.Register(this);

			return base.Initialize();
		}

		/// <summary>
		/// Executes a node.
		/// </summary>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>Next node of execution, if different from the default, otherwise null (for default).</returns>
		public override async Task<LinkedListNode<IActivityNode>> Execute(Variables Variables)
		{
			await Activity.ExecuteActivity(Variables, this.FirstNode);
			return null;
		}

		/// <summary>
		/// If the node condition is true.
		/// </summary>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>If embedded nodes are to be executed.</returns>
		public Task<bool> IsTrue(Variables Variables)
		{
			return Task.FromResult(true);
		}

		/// <summary>
		/// Exports PlantUML
		/// </summary>
		/// <param name="Output">Output</param>
		/// <param name="Indentation">Number of tabs to indent.</param>
		/// <param name="First">If the condition is the first condition.</param>
		/// <param name="QuoteChar">Quote character.</param>
		public void ExportPlantUml(StreamWriter Output, int Indentation, bool First, char QuoteChar)
		{
			Indent(Output, Indentation);
			Output.WriteLine("else (otherwise)");

			base.ExportPlantUml(Output, Indentation + 1, QuoteChar);
		}

	}
}
