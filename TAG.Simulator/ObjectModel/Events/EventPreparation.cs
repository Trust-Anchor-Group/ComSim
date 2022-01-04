using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
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
		/// <param name="Model">Model in which the node is defined.</param>
		public EventPreparation(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Initialized the node before simulation.
		/// </summary>
		public override Task Initialize()
		{
			if (this.Parent is IEvent Event)
				Event.Register(this);

			return base.Initialize();
		}

		/// <summary>
		/// Prepares <paramref name="Variables"/> for the execution of an event.
		/// </summary>
		/// <param name="Variables">Event variables</param>
		/// <param name="Tags">Extensible list of meta-data tags related to the event.</param>
		public abstract Task Prepare(Variables Variables, List<KeyValuePair<string, object>> Tags);

		/// <summary>
		/// Releases resources at the end of an event.
		/// </summary>
		/// <param name="Variables">Event variables</param>
		public abstract void Release(Variables Variables);

		/// <summary>
		/// Exports the node to PlantUML script in a markdown document.
		/// </summary>
		/// <param name="Output">Output stream.</param>
		/// <param name="Name">Optional name for the association.</param>
		/// <param name="Index">Chart Index</param>
		public abstract void ExportPlantUml(StreamWriter Output, string Name, int Index);
	}
}
