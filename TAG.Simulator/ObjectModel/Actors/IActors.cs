using System;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using TAG.Simulator.ObjectModel.Events;

namespace TAG.Simulator.ObjectModel.Actors
{
	/// <summary>
	/// Interface for collections of actors.
	/// </summary>
	public interface IActors : ISimulationNode
	{
		/// <summary>
		/// Registers an actor with the collection of actors.
		/// </summary>
		/// <param name="Actor">Actor</param>
		void Register(IActor Actor);
	}
}
