using System;
using System.Threading.Tasks;
using System.Xml;
using Waher.Content.Xml;

namespace TAG.Simulator.ObjectModel.Activities
{
	/// <summary>
	/// Conditional execution in an activity.
	/// </summary>
	public class Conditional : ActivityNode
	{
		/// <summary>
		/// Conditional execution in an activity.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		public Conditional(ISimulationNode Parent)
			: base(Parent)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "Conditional";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent)
		{
			return new Conditional(Parent);
		}

	}
}
