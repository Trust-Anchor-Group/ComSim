using System.Collections.Generic;

namespace TAG.Simulator.ObjectModel.Actors
{
	/// <summary>
	/// Container for actors.
	/// </summary>
	public class Actors : SimulationNodeChildren, IActors
	{
		private readonly List<IActor> actors = new List<IActor>();

		/// <summary>
		/// Container for actors.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public Actors(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(Actors);

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new Actors(Parent, Model);
		}

		/// <summary>
		/// Registers an actor with the collection of actors.
		/// </summary>
		/// <param name="Actor">Actor</param>
		public void Register(IActor Actor)
		{
			this.actors.Add(Actor);
		}
	}
}
