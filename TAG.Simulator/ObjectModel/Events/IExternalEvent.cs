using System.Collections.Generic;
using TAG.Simulator.ObjectModel.Actors;

namespace TAG.Simulator.ObjectModel.Events
{
	/// <summary>
	/// Interface for external events.
	/// </summary>
	public interface IExternalEvent : ISimulationNode
	{
		/// <summary>
		/// Name of external event
		/// </summary>
		string Name
		{
			get;
		}

		/// <summary>
		/// Optional name for actor variable reference of sender of external event.
		/// </summary>
		string ActorName
		{
			get;
		}

		/// <summary>
		/// Reference to collection of external events.
		/// </summary>
		IExternalEventsNode Events
		{
			get;
		}

		/// <summary>
		/// Parameters
		/// </summary>
		IEnumerable<Parameter> Parameters
		{
			get;
		}

		/// <summary>
		/// Method called when an external event has been received.
		/// </summary>
		/// <param name="Source">Actor receiving the event.</param>
		/// <param name="Arguments">Event arguments.</param>
		/// <returns>If event was handled</returns>
		void Trigger(IActor Source, params KeyValuePair<string, object>[] Arguments);

	}
}
