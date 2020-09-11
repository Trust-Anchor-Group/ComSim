using System;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel.Activities;
using Waher.Content.Xml;
using Waher.Events;
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
		/// <param name="Model">Current model</param>
		/// <param name="Variables">Event variables</param>
		void Prepare(Model Model, Variables Variables);

		/// <summary>
		/// Releases resources at the end of an event.
		/// </summary>
		/// <param name="Model">Current model</param>
		/// <param name="Variables">Event variables</param>
		void Release(Model Model, Variables Variables);
	}
}
