using System;
using System.Collections.Generic;
using System.IO;
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
		/// <param name="Model">Model in which the node is defined.</param>
		public Parallel(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(Parallel);

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new Parallel(Parent, Model);
		}

		/// <summary>
		/// Executes a node.
		/// </summary>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>Next node of execution, if different from the default, otherwise null (for default).</returns>
		public override async Task<LinkedListNode<IActivityNode>> Execute(Variables Variables)
		{
			Task[] Tasks = new Task[this.Count];
			LinkedListNode<IActivityNode> Loop = this.FirstNode;
			int i = 0;

			while (!(Loop is null))
			{
				Tasks[i++] = Loop.Value.Execute(Variables);
				Loop = Loop.Next;
			}

			await Task.WhenAll(Tasks);

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
			LinkedListNode<IActivityNode> Loop = this.FirstNode;

			while (!(Loop is null))
			{
				Indent(Output, Indentation);

				if (First)
				{
					First = false;
					Output.WriteLine("fork");
				}
				else
					Output.WriteLine("fork again");

				Loop.Value.ExportPlantUml(Output, Indentation + 1, QuoteChar);
				Loop = Loop.Next;
			}

			if (!First)
			{
				Indent(Output, Indentation);
				Output.WriteLine("end fork");
			}
		}

	}
}
