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
	/// Interface for events
	/// </summary>
	public interface IEvent : ISimulationNode
	{
		/// <summary>
		/// ID of event.
		/// </summary>
		string Id
		{
			get;
		}

		/// <summary>
		/// Triggers the event.
		/// </summary>
		/// <param name="Variables">Event variables</param>
		void Trigger(Variables Variables);
	}
}
