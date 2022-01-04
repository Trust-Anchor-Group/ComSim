using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Activities
{
	/// <summary>
	/// Conditional execution in an activity, based on external events.
	/// </summary>
	public class Wait : ActivityNode
	{
		private readonly List<ITriggerNode> triggers = new List<ITriggerNode>();
		private Timeout timeout = null;

		/// <summary>
		/// Conditional execution in an activity, based on external events.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public Wait(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "Wait";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new Wait(Parent, Model);
		}

		/// <summary>
		/// Register a trigger node.
		/// </summary>
		/// <param name="Node">Trigger node</param>
		public void Register(ITriggerNode Node)
		{
			if (Node is Timeout Timeout)
			{
				if (this.timeout is null)
					this.timeout = Timeout;
				else
					throw new Exception("A timeout has already been defined.");
			}
			else
				this.triggers.Add(Node);
		}

		/// <summary>
		/// Executes a node.
		/// </summary>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>Next node of execution, if different from the default, otherwise null (for default).</returns>
		public override async Task<LinkedListNode<IActivityNode>> Execute(Variables Variables)
		{
			int i, c = this.triggers.Count;
			if (c == 0)
				throw new Exception("No triggers defined.");

			int d = c;

			if (!(this.timeout is null))
				d++;

			Task[] Tasks = new Task[d];

			for (i = 0; i < c; i++)
				Tasks[i] = this.triggers[i].GetTask();

			if (!(this.timeout is null))
				Tasks[c] = this.timeout.GetTask();

			Task Result = await Task.WhenAny(Tasks);
			i = Array.IndexOf<Task>(Tasks, Result);

			if (i < 0)
				throw new Exception("Unexpected error.");

			if (i < c)
				await this.triggers[i].Execute(Variables);
			else
				await this.timeout.Execute(Variables);

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
			Indent(Output, Indentation);
			Output.WriteLine(":Wait for first:;");
			Indent(Output, Indentation);

			bool First = true;

			foreach (ITriggerNode Node in this.triggers)
			{
				Node.ExportPlantUml(Output, Indentation, First, QuoteChar);
				First = false;
			}

			if (!First)
			{
				Indent(Output, Indentation);
				Output.WriteLine("end split");
			}
		}

	}
}
