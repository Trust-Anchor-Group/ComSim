using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Waher.Events;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Activities.EventLog
{
	/// <summary>
	/// Logs an warning event.
	/// </summary>
	public class LogWarning : LogActivityNode
	{
		/// <summary>
		/// Logs an warning event.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public LogWarning(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(LogWarning);

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new LogWarning(Parent, Model);
		}

		/// <summary>
		/// Type of event.
		/// </summary>
		public override EventType EventType => EventType.Warning;
	}
}
