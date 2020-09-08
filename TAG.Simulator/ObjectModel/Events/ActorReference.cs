using System;
using System.Threading.Tasks;
using System.Xml;
using Waher.Content.Xml;

namespace TAG.Simulator.ObjectModel.Events
{
	/// <summary>
	/// References a population of actors.
	/// </summary>
	public class ActorReference : SimulationNodeChildren
	{
		private string name;
		private bool exclusive;

		/// <summary>
		/// References a population of actors.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		public ActorReference(ISimulationNode Parent)
			: base(Parent)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "ActorReference";

		/// <summary>
		/// Name of actor within the scope of the event.
		/// </summary>
		public string Name => this.name;

		/// <summary>
		/// If the actor is referenced for exclusive use in the event (i.e. cannot participate in another event at the same time).
		/// </summary>
		public bool Exclusive => this.exclusive;

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent)
		{
			return new ActorReference(Parent);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.name = XML.Attribute(Definition, "name");
			this.exclusive = XML.Attribute(Definition, "exclusive", true);

			return base.FromXml(Definition);
		}

	}
}
