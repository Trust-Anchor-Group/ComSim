using System;

namespace TAG.Simulator.ObjectModel.Activities
{
	/// <summary>
	/// Terminates execution of activity.
	/// </summary>
	public class Finished : ActivityNode 
	{
		/// <summary>
		/// Terminates execution of activity.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		public Finished(ISimulationNode Parent)
			: base(Parent)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "Finished";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent)
		{
			return new Finished(Parent);
		}
	}
}
