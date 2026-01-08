using System;
using System.Collections.Generic;
using System.IO;

namespace TAG.Simulator.ObjectModel.Events
{
    /// <summary>
    /// Abstract base class for nodes that registers external events.
    /// </summary>
    public abstract class ExternalEventsNode : SimulationNodeChildren, IExternalEventsNode
	{
        /// <summary>
        /// Registered external events.
        /// </summary>
        protected Dictionary<string, IExternalEvent> externalEvents = null;

		/// <summary>
		/// Abstract base class for nodes that registers external events.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public ExternalEventsNode(ISimulationNode Parent, Model Model)
            : base(Parent, Model)
        {
        }

		/// <summary>
		/// ID of collection node.
		/// </summary>
		public abstract string Id
		{
			get;
		}

		/// <summary>
		/// Registers an external event on the actor.
		/// </summary>
		/// <param name="ExternalEvent">External event</param>
		public void Register(IExternalEvent ExternalEvent)
        {
            string Name = ExternalEvent.Name;

            this.externalEvents ??= new Dictionary<string, IExternalEvent>();

            if (this.externalEvents.ContainsKey(Name))
                throw new Exception("External event named " + Name + " already registered.");
            else
				this.externalEvents[Name] = ExternalEvent;
        }

        /// <summary>
        /// Tries to get an external event, given its name.
        /// </summary>
        /// <param name="Name">Name of external event.</param>
        /// <param name="ExternalEvent">External event object.</param>
        /// <returns>If an external event with the corresponding name was found.</returns>
        public bool TryGetExternalEvent(string Name, out IExternalEvent ExternalEvent)
        {
            if (this.externalEvents is null)
            {
                ExternalEvent = null;
                return false;
            }
            else
                return this.externalEvents.TryGetValue(Name, out ExternalEvent);
        }

		/// <summary>
		/// Allows the actor to add notes related to the actor in use case diagrams.
		/// </summary>
		/// <param name="Output">Use Case diagram output.</param>
		/// <param name="Id">ID of actor in use case diagram.</param>
		public virtual void AnnotateActorUseCaseUml(StreamWriter Output, string Id)
		{
		}
	}
}
