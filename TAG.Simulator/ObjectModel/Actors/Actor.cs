using System;
using System.Threading.Tasks;
using System.Xml;
using Waher.Content.Xml;

namespace TAG.Simulator.ObjectModel.Actors
{
	/// <summary>
	/// Abstract base class for actors
	/// </summary>
	public abstract class Actor : SimulationNode
	{
		private string id;
		private int n;

		/// <summary>
		/// Abstract base class for actors
		/// </summary>
		/// <param name="Parent">Parent node</param>
		public Actor(ISimulationNode Parent)
			: base(Parent)
		{
		}

		/// <summary>
		/// ID of actor.
		/// </summary>
		public string Id => this.id;

		/// <summary>
		/// Number of actors of this type specified.
		/// </summary>
		public int N => this.n;

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.id = XML.Attribute(Definition, "id");
			this.n = XML.Attribute(Definition, "N", 0);

			return Task.CompletedTask;
		}

		/// <summary>
		/// Initialized the node before simulation.
		/// </summary>
		/// <param name="Model">Model being executed.</param>
		public override Task Initialize(Model Model)
		{
			Model.Register(this);
			return base.Initialize(Model);
		}
	}
}
