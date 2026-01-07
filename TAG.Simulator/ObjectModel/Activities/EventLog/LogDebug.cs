using Waher.Events;

namespace TAG.Simulator.ObjectModel.Activities.EventLog
{
	/// <summary>
	/// Logs a debug event.
	/// </summary>
	public class LogDebug : LogActivityNode
	{
		/// <summary>
		/// Logs a debug event.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public LogDebug(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(LogDebug);

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new LogDebug(Parent, Model);
		}

		/// <summary>
		/// Type of event.
		/// </summary>
		public override EventType EventType => EventType.Debug;
	}
}
