using System.IO;

namespace TAG.Simulator.ObjectModel.Events
{
    /// <summary>
    /// Interface for nodes that can register external events.
    /// </summary>
    public interface IExternalEventsNode : ISimulationNodeChildren
    {
        /// <summary>
        /// ID of collection node.
        /// </summary>
        string Id
        {
            get;
        }

        /// <summary>
        /// Registers an external event on the actor.
        /// </summary>
        /// <param name="ExternalEvent">External event</param>
        void Register(IExternalEvent ExternalEvent);

        /// <summary>
        /// Tries to get an external event, given its name.
        /// </summary>
        /// <param name="Name">Name of external event.</param>
        /// <param name="ExternalEvent">External event object.</param>
        /// <returns>If an external event with the corresponding name was found.</returns>
        bool TryGetExternalEvent(string Name, out IExternalEvent ExternalEvent);

		/// <summary>
		/// Allows the actor to add notes related to the actor in use case diagrams.
		/// </summary>
		/// <param name="Output">Use Case diagram output.</param>
		/// <param name="Id">ID of actor in use case diagram.</param>
		void AnnotateActorUseCaseUml(StreamWriter Output, string Id);
	}
}
