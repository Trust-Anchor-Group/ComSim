using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Activities
{
	/// <summary>
	/// Represents the stoping point of the activity.
	/// </summary>
	public class Stop : ActivityNode 
	{
		/// <summary>
		/// Represents the stoping point of the activity.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public Stop(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "Stop";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new Stop(Parent, Model);
		}

		/// <summary>
		/// Executes a node.
		/// </summary>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>Next node of execution, if different from the default, otherwise null (for default).</returns>
		public override Task<LinkedListNode<IActivityNode>> Execute(Variables Variables)
		{
			return Task.FromResult<LinkedListNode<IActivityNode>>(null);
		}

		/// <summary>
		/// Exports PlantUML
		/// </summary>
		/// <param name="Output">Output node</param>
		/// <param name="Indentation">Number of tabs to indent.</param>
		public override void ExportPlantUml(StreamWriter Output, int Indentation)
		{
			Indent(Output, Indentation);
			Output.WriteLine("stop");
		}

	}
}
