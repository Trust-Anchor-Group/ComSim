using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TAG.Simulator.Extensions;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Activities.Execution
{
	/// <summary>
	/// Jumps to another node in the activity.
	/// </summary>
	public class GoTo : ReferenceActivityNode 
	{
		private LinkedListNode<IActivityNode> node;

		/// <summary>
		/// Jumps to another node in the activity.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public GoTo(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Referenced node
		/// </summary>
		public LinkedListNode<IActivityNode> Node => this.node;

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(GoTo);

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new GoTo(Parent, Model);
		}

		/// <summary>
		/// Starts the node.
		/// </summary>
		public override Task Start()
		{
			if (!this.Model.TryGetActivityNode(this.Reference, out this.node))
				throw new Exception("Activity node not found: " + this.Reference);

			return base.Start();
		}

		/// <summary>
		/// Executes a node.
		/// </summary>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>Next node of execution, if different from the default, otherwise null (for default).</returns>
		public override Task<LinkedListNode<IActivityNode>> Execute(Variables Variables)
		{
			return Task.FromResult(this.node);
		}

		/// <summary>
		/// Exports PlantUML
		/// </summary>
		/// <param name="Output">Output</param>
		/// <param name="Indentation">Number of tabs to indent.</param>
		/// <param name="QuoteChar">Quote character.</param>
		public override void ExportPlantUml(StreamWriter Output, int Indentation, char QuoteChar)
		{
			Output.Indent(Indentation);
			Output.Write('(');
			Output.Write(this.Reference);
			Output.WriteLine(')');

			Output.Indent(Indentation);
			Output.WriteLine("detach");
		}
	}
}
