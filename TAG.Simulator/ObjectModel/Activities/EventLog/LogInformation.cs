using Waher.Events;

namespace TAG.Simulator.ObjectModel.Activities.EventLog
{
	/// <summary>
	/// Logs an informational event.
	/// </summary>
	public class LogInformation : LogActivityNode
	{
		/// <summary>
		/// Logs an informational event.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public LogInformation(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(LogInformation);

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new LogInformation(Parent, Model);
		}

		/// <summary>
		/// Type of event.
		/// </summary>
		public override EventType EventType => EventType.Informational;
	}
}
