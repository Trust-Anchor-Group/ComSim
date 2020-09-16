using System;
using System.Collections.Generic;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Events
{
	/// <summary>
	/// Interface for event preparation nodes
	/// </summary>
	public interface IEventPreparation : ISimulationNode
	{
		/// <summary>
		/// Prepares <paramref name="Variables"/> for the execution of an event.
		/// </summary>
		/// <param name="Variables">Event variables</param>
		/// <param name="Tags">Extensible list of meta-data tags related to the event.</param>
		void Prepare(Variables Variables, List<KeyValuePair<string, object>> Tags);

		/// <summary>
		/// Releases resources at the end of an event.
		/// </summary>
		/// <param name="Variables">Event variables</param>
		void Release(Variables Variables);
	}
}
