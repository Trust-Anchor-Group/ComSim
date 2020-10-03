using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel;
using TAG.Simulator.ObjectModel.Actors;
using TAG.Simulator.ObjectModel.Events;
using TAG.Simulator.XMPP.Actors;
using Waher.Content.Xml;
using Waher.Networking.XMPP;
using Waher.Script;

namespace TAG.Simulator.XMPP.Events
{
	/// <summary>
	/// Abstract base class for handler nodes
	/// </summary>
	public abstract class HandlerNode : SimulationNode, IExternalEvent
	{
		private IEvent _event;
		private IActor actor;
		private string name;
		private string _namespace;
		private string eventId;
		private string actorName;
		private string eventArgs;

		/// <summary>
		/// Abstract base class for handler nodes
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public HandlerNode(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Event reference.
		/// </summary>
		public IEvent Event => this._event;

		/// <summary>
		/// Local element name for the handler
		/// </summary>
		public string Name => this.name;

		/// <summary>
		/// Namespace for the handler
		/// </summary>
		public string HandlerNamespace => this._namespace;

		/// <summary>
		/// Event to trigger
		/// </summary>
		public string EventId => this.eventId;

		/// <summary>
		/// Variable name of the actor
		/// </summary>
		public string ActorName => this.actorName;

		/// <summary>
		/// Actor reference.
		/// </summary>
		public IActor Actor => this.actor;

		/// <summary>
		/// Variable name for the event arguments
		/// </summary>
		public string EventArgs => this.eventArgs;

		/// <summary>
		/// Points to the embedded XML Schema resource defining the semantics of the XML namespace.
		/// </summary>
		public override string SchemaResource => XmppActor.XmppSchema;

		/// <summary>
		/// XML Namespace where the element is defined.
		/// </summary>
		public override string Namespace => XmppActor.XmppNamespace;

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.name = XML.Attribute(Definition, "name");
			this._namespace = XML.Attribute(Definition, "namespace");
			this.eventId = XML.Attribute(Definition, "event");
			this.actorName = XML.Attribute(Definition, "actorName");
			this.eventArgs = XML.Attribute(Definition, "eventArgs");

			return Task.CompletedTask;
		}

		/// <summary>
		/// Initialized the node before simulation.
		/// </summary>
		public override Task Initialize()
		{
			this.actor = this.Parent as IActor;
			if (!(this.actor is null))
				this.actor.Register(this);
			else
				throw new Exception("External event registered on a node that is not an actor.");

			return base.Initialize();
		}

		/// <summary>
		/// Starts the node.
		/// </summary>
		public override Task Start()
		{
			if (!this.Model.TryGetEvent(this.eventId, out this._event))
				throw new Exception("Event not found: " + this.eventId);

			this._event.Register(this);

			return base.Start();
		}

		/// <summary>
		/// Method called when an external event has been received.
		/// </summary>
		/// <param name="Source">Actor receiving the event.</param>
		/// <param name="Arguments">Event arguments.</param>
		/// <returns>If event was handled</returns>
		public virtual void Trigger(IActor Source, params KeyValuePair<string, object>[] Arguments)
		{
			Variables Variables = this.Model.GetEventVariables(Source);

			if (!string.IsNullOrEmpty(this.actorName))
				Variables[this.actorName] = Source.ActivityObject;

			if (!(Arguments is null))
			{
				foreach (KeyValuePair<string, object> Argument in Arguments)
					Variables[Argument.Key] = Argument.Value;
			}

			this._event?.Trigger(Variables);
		}

		/// <summary>
		/// Parameters
		/// </summary>
		public IEnumerable<Parameter> Parameters
		{
			get
			{
				return new Parameter[] { new Parameter(this, this.Model, "e", string.IsNullOrEmpty(this.eventArgs) ? "e" : this.eventArgs) };
			}
		}

		/// <summary>
		/// Registers handlers on the XMPP Client.
		/// </summary>
		/// <param name="Actor">Actor instance reference</param>
		/// <param name="Client">XMPP Client</param>
		public abstract void RegisterHandlers(IActor Actor, XmppClient Client);

	}
}
