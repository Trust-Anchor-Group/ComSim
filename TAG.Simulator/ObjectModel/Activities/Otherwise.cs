using System;
using System.Threading.Tasks;
using System.Xml;
using Waher.Content;
using Waher.Content.Xml;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Activities
{
	/// <summary>
	/// Represents a condition that is always true.
	/// </summary>
	public class Otherwise : SimulationNodeChildren 
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
	}
}
