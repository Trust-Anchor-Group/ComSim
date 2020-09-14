using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Activities
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
		public Otherwise(ISimulationNode Parent)
			: base(Parent)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "Otherwise";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent)
		{
			return new Otherwise(Parent);
		}

		/// <summary>
		/// Initialized the node before simulation.
		/// </summary>
		/// <param name="Model">Model being executed.</param>
		public override Task Initialize(Model Model)
		{
			if (this.Parent is Conditional Conditional)
				Conditional.Register(this);

			return base.Initialize(Model);
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

		/// <summary>
		/// If the node condition is true.
		/// </summary>
		/// <param name="Model">Current model</param>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>If embedded nodes are to be executed.</returns>
		public bool IsTrue(Model Model, Variables Variables)
		{
			return true;
		}

		/// <summary>
		/// Exports PlantUML
		/// </summary>
		/// <param name="Output">Output node</param>
		/// <param name="Indentation">Number of tabs to indent.</param>
		/// <param name="First">If the condition is the first condition.</param>
		public void ExportPlantUml(StreamWriter Output, int Indentation, bool First)
		{
			Indent(Output, Indentation);
			Output.Write("else (otherwise)");

			base.ExportPlantUml(Output, Indentation + 1);
		}

	}
}
