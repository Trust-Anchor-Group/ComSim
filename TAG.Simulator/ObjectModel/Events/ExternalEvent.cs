using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel.Actors;
using Waher.Content.Xml;
using Waher.Events;
using Waher.Runtime.Collections;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Events
{
	/// <summary>
	/// Handles an incoming event raised from an external source
	/// </summary>
	public class ExternalEvent : SimulationNodeChildren, IExternalEvent
	{
		private Dictionary<string, Parameter> parameters = null;
		private IEvent eventReference;
		private IExternalEventsNode events;
		private string name;
		private string eventId;
		private string actorName;

		/// <summary>
		/// Handles an incoming event raised from an external source
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public ExternalEvent(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
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
		/// Reference to collection of external events.
		/// </summary>
		public IExternalEventsNode Events => this.events;

		/// <summary>
		/// Referenced event object.
		/// </summary>
		public IEvent Event => this.eventReference;

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(ExternalEvent);

		/// <summary>
		/// Parameters
		/// </summary>
		public IEnumerable<Parameter> Parameters => this.parameters?.Values;

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new ExternalEvent(Parent, Model);
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
		public override Task Initialize()
		{
			ISimulationNode Loop = this.Parent;

			while (!(Loop is null))
			{
				if (Loop is IExternalEventsNode Events)
				{
					this.events = Events;
					this.events.Register(this);
					break;
				}
				else
					Loop = Loop.Parent;
			}

			if (Loop is null)
				throw new Exception("External event registered on a node that is not hosted by an actor.");

			return Task.CompletedTask;
		}

		/// <summary>
		/// Starts the node.
		/// </summary>
		public override async Task Start()
		{
			if (!this.Model.TryGetEvent(this.eventId, out this.eventReference))
				throw new Exception("Event node not found: " + this.eventId);

			this.eventReference.Register(this);

			await base.Start();

			if (this.hasQueued && this.eventReference.IsInitialized)
				await this.TriggerQueued();
		}

		/// <summary>
		/// Called when all nodes have been started.
		/// </summary>
		public override async Task Started()
		{
			if (this.hasQueued && this.eventReference.IsInitialized)
				await this.TriggerQueued();
		}

		/// <summary>
		/// Registers a parameter with the external event.
		/// </summary>
		/// <param name="Parameter">Parameter</param>
		public void Register(Parameter Parameter)
		{
			string Name = Parameter.Name;

			this.parameters ??= new Dictionary<string, Parameter>();

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
		public async Task Trigger(IActor Source, params KeyValuePair<string, object>[] Arguments)
		{
			Variables Variables = this.Model.GetEventVariables(Source);

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

			if (this.eventReference is null || !this.eventReference.IsInitialized)
			{
				Log.Informational("External event queued.", this.eventId, Source.InstanceId);

				lock (this.syncObj)
				{
					this.queued ??= new ChunkedList<QueuedEvent>(16);
					this.queued.Add(new QueuedEvent()
					{
						Source = Source,
						Variables = Variables
					});
					this.hasQueued = true;
				}
			}
			else
			{
				if (this.hasQueued)
					await this.TriggerQueued();

				await this.eventReference.Trigger(Variables);
			}
		}

		private async Task TriggerQueued()
		{
			QueuedEvent[] Queued;

			lock (this.syncObj)
			{
				Queued = this.queued?.ToArray() ?? Array.Empty<QueuedEvent>();
				this.queued = null;
				this.hasQueued = false;
			}

			foreach (QueuedEvent Item in Queued)
			{
				Log.Informational("Queued event executed.", this.eventId,
					Item.Source.InstanceId);

				await this.eventReference.Trigger(Item.Variables);
			}
		}

		private ChunkedList<QueuedEvent> queued = null;
		private readonly object syncObj = new object();
		private bool hasQueued = false;

		private class QueuedEvent
		{
			public IActor Source;
			public Variables Variables;
		}
	}
}
