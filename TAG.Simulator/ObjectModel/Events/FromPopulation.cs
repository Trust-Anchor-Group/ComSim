using System;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel.Actors;
using Waher.Content.Xml;

namespace TAG.Simulator.ObjectModel.Events
{
	/// <summary>
	/// References a specific population of actors.
	/// </summary>
	public class FromPopulation : SimulationNode
	{
		private IActor actor;
		private string actorId;

		/// <summary>
		/// References a specific population of actors.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		public FromPopulation(ISimulationNode Parent)
			: base(Parent)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "FromPopulation";

		/// <summary>
		/// Name of actor defining the population.
		/// </summary>
		public string ActorId => this.actorId;

		/// <summary>
		/// Referenced actor.
		/// </summary>
		public IActor Actor => this.actor;

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent)
		{
			return new FromPopulation(Parent);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.actorId = XML.Attribute(Definition, "actor");
			return Task.CompletedTask;
		}

		/// <summary>
		/// Initialized the node before simulation.
		/// </summary>
		/// <param name="Model">Model being executed.</param>
		public override Task Initialize(Model Model)
		{
			if (!Model.TryGetActor(this.actorId, out this.actor))
				throw new Exception("Actor not found: " + this.actorId);

			if (this.Parent is IActors Actors)
				Actors.Register(this.actor);

			return base.Initialize(Model);
		}

	}
}
