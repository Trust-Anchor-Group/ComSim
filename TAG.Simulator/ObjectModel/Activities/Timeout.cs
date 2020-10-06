using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Waher.Content;
using Waher.Content.Xml;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Activities
{
	/// <summary>
	/// Adds a timeout limit in a Wait statement.
	/// </summary>
	public class Timeout : ActivityNode, ITriggerNode
	{
		private Duration limit;

		/// <summary>
		/// Adds a timeout limit in a Wait statement.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public Timeout(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "Timeout";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new Timeout(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.limit = XML.Attribute(Definition, "limit", Duration.Zero);

			return base.FromXml(Definition);
		}

		/// <summary>
		/// Initialized the node before simulation.
		/// </summary>
		public override Task Initialize()
		{
			if (this.Parent is Wait Wait)
				Wait.Register(this);

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
		/// Gets a task object.
		/// </summary>
		/// <returns>Task object signalling when trigger is activated.</returns>
		public Task GetTask()
		{
			DateTime Now = DateTime.Now;
			DateTime TP = Now + this.limit;
			double ms = (TP - Now).TotalMilliseconds;
			if (ms < 0)
				ms = 0;

			return Task.Delay((int)ms);
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

			if (First)
				Output.WriteLine("split");
			else
				Output.WriteLine("split again");

			Indentation++;
			Indent(Output, Indentation);

			Output.Write("#FireBrick:");
			Values.Duration.ExportText(this.limit, Output);
			Output.WriteLine("<");

			base.ExportPlantUml(Output, Indentation + 1, QuoteChar);
		}

	}
}
