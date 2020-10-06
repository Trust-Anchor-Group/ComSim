using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TAG.Simulator.ObjectModel.Activities
{
	/// <summary>
	/// Container for activities.
	/// </summary>
	public class Activities : SimulationNodeChildren
	{
		/// <summary>
		/// Container for activities.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public Activities(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "Activities";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new Activities(Parent, Model);
		}

		/// <summary>
		/// Gets the declared order of activities in the model.
		/// </summary>
		/// <returns>Order</returns>
		public string[] ActivityOrder()
		{
			List<string> Order = new List<string>();

			foreach (ISimulationNode Node in this.Children)
			{
				if (Node is IActivity Activity)
					Order.Add(Activity.Id);
			}

			return Order.ToArray();
		}

		/// <summary>
		/// Exports Markdown
		/// </summary>
		/// <param name="Output">Output</param>
		public override Task ExportMarkdown(StreamWriter Output)
		{
			Output.WriteLine("Activities");
			Output.WriteLine("=============");
			Output.WriteLine();

			this.Model.ExportActivityStartStatistics(Output);

			return base.ExportMarkdown(Output);
		}

	}
}
