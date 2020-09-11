using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel.Actors;
using Waher.Content.Xml;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Events
{
	/// <summary>
	/// Handles an incoming event raised from an external source
	/// </summary>
	public class ExternalEvent : SimulationNodeChildren
	{
		private Dictionary<string, Parameter> parameters = null;
		private Model model;
		private IEvent eventReference;
		private string name;
		private string eventId;
		private string actorName;

		/// <summary>
		/// Handles an incoming event raised from an external source
		/// </summary>
		/// <param name="Parent">Parent node</param>
		public ExternalEvent(ISimulationNode Parent)
			: base(Parent)
		{
		}

		/// <summary>
		/// Name of external event
		/// </summary>
		public string Name => this.name;

		/// <summary>
		/// Optional name for variable, if different from <see cref="Name"/>
		/// </summary>
		public string EventId => this.eventId;

		/// <summary>
		/// Optional name for actor variable reference of sender of external event.
		/// </summary>
		public string ActorName => this.actorName;

		/// <summary>
		/// Referenced event object.
		/// </summary>
		public IEvent Event => this.eventReference;

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "ExternalEvent";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent)
		{
			return new ExternalEvent(Parent);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.name = XML.Attribute(Definition, "name");
			this.eventId = XML.Attribute(Definition, "event");
			this.actorName = XML.Attribute(Definition, "actorName");

			return base.FromXml(Definition);
		}

		/// <summary>
		/// Initialized the node before simulation.
		/// </summary>
		/// <param name="Model">Model being executed.</param>
		public override Task Initialize(Model Model)
		{
			this.model = Model;

			if (this.Parent is IActor Actor)
				Actor.Register(this);
			else
				throw new Exception("External event registered on a node that is not an actor.");

			return base.Initialize(Model);
		}

		/// <summary>
		/// Starts the node.
		/// </summary>
		public override Task Start()
		{
			if (!this.model.TryGetEvent(this.eventId, out this.eventReference))
				throw new Exception("Event node not found: " + this.eventId);

			return base.Start();
		}

		/// <summary>
		/// Registers a parameter with the external event.
		/// </summary>
		/// <param name="Parameter">Parameter</param>
		public void Register(Parameter Parameter)
		{
			string Name = Parameter.Name;

			if (this.parameters is null)
				this.parameters = new Dictionary<string, Parameter>();

			if (this.parameters.ContainsKey(Name))
				throw new Exception("A parameter named " + Name + " has already been registered for external event " + this.eventId);

			this.parameters[Name] = Parameter;
		}

		/// <summary>
		/// Method called when an external event has been received.
		/// </summary>
		/// <param name="Source">Actor receiving the event.</param>
		/// <param name="Arguments">Event arguments.</param>
		/// <returns>If event was handled</returns>
		public void Trigger(IActor Source, params KeyValuePair<string, object>[] Arguments)
		{
			Variables Variables = new Variables();

			if (!string.IsNullOrEmpty(this.actorName))
				Variables[this.actorName] = Source.ActivityObject;

			if (!(this.parameters is null))
			{
				string Name;

				foreach (KeyValuePair<string, object> P in Arguments)
				{
					if (this.parameters.TryGetValue(P.Key, out Parameter P2))
					{
						Name = P2.Variable;
						if (string.IsNullOrEmpty(Name))
							Name = P2.Name;

						Variables[Name] = P.Value;
					}
				}
			}

			Event.Trigger(Variables);
		}

	}
}
