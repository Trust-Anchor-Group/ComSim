using System;
using System.Collections.Generic;
using System.Text;

namespace TAG.Simulator.ObjectModel.MetaData
{
	/// <summary>
	/// Node that contains meta-data about the model.
	/// </summary>
	public class Meta : SimulationNodeChildren
	{
		/// <summary>
		/// Node that contains meta-data about the model.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		public Meta(ISimulationNode Parent)
			: base(Parent)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "Meta";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent)
		{
			return new Meta(Parent);
		}
	}
}
