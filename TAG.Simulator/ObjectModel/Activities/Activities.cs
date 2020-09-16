using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.Statistics;

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
	}
}
