using System;
using System.Threading.Tasks;
using System.Xml;
using Waher.Content.Xml;

namespace TAG.Simulator.ObjectModel.Activities
{
	/// <summary>
	/// Represents an action on an actor.
	/// </summary>
	public class Action : ActivityNode
	{
		private string actor;
		private string action;

		/// <summary>
		/// Represents an action on an actor.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		public Action(ISimulationNode Parent)
			: base(Parent)
		{
		}

		/// <summary>
		/// Actor ID
		/// </summary>
		public string ActorId => this.actor;

		/// <summary>
		/// Action
		/// </summary>
		public string ActionName => this.action;

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "Action";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent)
		{
			return new Action(Parent);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.actor = XML.Attribute(Definition, "actor");
			this.action = XML.Attribute(Definition, "action");

			return base.FromXml(Definition);
		}
	}
}
