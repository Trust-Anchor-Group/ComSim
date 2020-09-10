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
	/// Abstract base class for event preparation nodes (with children).
	/// </summary>
	public abstract class EventPreparation : SimulationNodeChildren, IEventPreparation
	{
		/// <summary>
		/// Abstract base class for event preparation nodes (with children).
		/// </summary>
		/// <param name="Parent">Parent node</param>
		public EventPreparation(ISimulationNode Parent)
			: base(Parent)
		{
		}

		/// <summary>
		/// Initialized the node before simulation.
		/// </summary>
		/// <param name="Model">Model being executed.</param>
		public override Task Initialize(Model Model)
		{
			if (this.Parent is IEvent Event)
				Event.Register(this);

			return base.Initialize(Model);
		}

		/// <summary>
		/// Prepares <paramref name="Variables"/> for the execution of an event.
		/// </summary>
		/// <param name="Model">Current model</param>
		/// <param name="Variables">Event variables</param>
		public abstract void Prepare(Model Model, Variables Variables);
	}
}
