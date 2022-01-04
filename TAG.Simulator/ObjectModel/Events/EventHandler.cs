using System;

namespace TAG.Simulator.ObjectModel.Events
{
	/// <summary>
	/// External Event
	/// </summary>
	public class EventHandler : Event
	{
		/// <summary>
		/// External Event
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public EventHandler(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "EventHandler";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new EventHandler(Parent, Model);
		}
	}
}
