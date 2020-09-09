using System;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using TAG.Simulator.ObjectModel.Events;

namespace TAG.Simulator
{
	/// <summary>
	/// Basic interface for simulator nodes. Implementing this interface allows classes with default contructors to be used
	/// in simulator models.
	/// </summary>
	public interface IActorNode : ISimulationNode
	{
		/// <summary>
		/// Registers an external event on the actor.
		/// </summary>
		/// <param name="ExternalEvent"></param>
		void Register(ExternalEvent ExternalEvent);

		/// <summary>
		/// Tries to get an external event, given its name.
		/// </summary>
		/// <param name="Name">Name of external event.</param>
		/// <param name="ExternalEvent">External event object.</param>
		/// <returns>If an external event with the corresponding name was found.</returns>
		bool TryGetExternalEvent(string Name, out ExternalEvent ExternalEvent);
	}
}
