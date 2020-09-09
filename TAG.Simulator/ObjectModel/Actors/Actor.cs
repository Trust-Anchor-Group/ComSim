﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel.Events;
using Waher.Content.Xml;

namespace TAG.Simulator.ObjectModel.Actors
{
	/// <summary>
	/// Abstract base class for actors
	/// </summary>
	public abstract class Actor : SimulationNodeChildren, IActorNode
	{
		private Dictionary<string, ExternalEvent> externalEvents = null;
		private Actor[] instances;
		private string id;
		private int n;
		private readonly string instanceId;
		private readonly int instanceIndex;

		/// <summary>
		/// Abstract base class for actors
		/// </summary>
		/// <param name="Parent">Parent node</param>
		public Actor(ISimulationNode Parent)
			: base(Parent)
		{
		}

		/// <summary>
		/// Abstract base class for actors
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="InstanceIndex">Instance index.</param>
		/// <param name="InstanceId">ID of instance</param>
		public Actor(ISimulationNode Parent, int InstanceIndex, string InstanceId)
			: base(Parent)
		{
			this.instanceIndex = InstanceIndex;
			this.instanceId = InstanceId;
		}

		/// <summary>
		/// ID of actor.
		/// </summary>
		public string Id => this.id;

		/// <summary>
		/// ID of actor instance.
		/// </summary>
		public string InstanceId => this.instanceId;

		/// <summary>
		/// Number of actors of this type specified.
		/// </summary>
		public int N => this.n;

		/// <summary>
		/// Actor instance index.
		/// </summary>
		public int InstanceIndex => this.instanceIndex;

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.id = XML.Attribute(Definition, "id");
			this.n = XML.Attribute(Definition, "N", 0);

			return base.FromXml(Definition);
		}

		/// <summary>
		/// Initialized the node before simulation.
		/// </summary>
		/// <param name="Model">Model being executed.</param>
		public override async Task Initialize(Model Model)
		{
			await base.Initialize(Model);
			Model.Register(this);

			int NrDigits = 1 + (int)Math.Log10(this.N);
			string Format = "D" + NrDigits.ToString();
			int i;

			this.instances = new Actor[this.N];

			for (i = 1; i <= this.n; i++)
			{
				Actor Instance = this.CreateInstance(i, this.id + i.ToString(Format));
				Instance.id = this.id;
				Instance.n = this.n;

				this.instances[i - 1] = Instance;

				await Instance.InitializeInstance();
			}
		}

		/// <summary>
		/// Starts the node.
		/// </summary>
		public override async Task Start()
		{
			await base.Start();

			foreach (Actor Actor in this.instances)
				await Actor.StartInstance();
		}

		/// <summary>
		/// Finalizes the node after simulation.
		/// </summary>
		public override async Task Finalize()
		{
			foreach (Actor Actor in this.instances)
			{
				await Actor.FinalizeInstance();

				if (Actor is IDisposable Disposable)
					Disposable.Dispose();
			}

			this.instances = null;

			await base.Finalize();
		}

		/// <summary>
		/// Creates an instance of the actor.
		/// </summary>
		/// <param name="InstanceIndex">Instance index.</param>
		/// <param name="InstanceId">ID of instance</param>
		/// <returns>Actor instance.</returns>
		public abstract Actor CreateInstance(int InstanceIndex, string InstanceId);

		/// <summary>
		/// Initializes an instance of an actor.
		/// </summary>
		public abstract Task InitializeInstance();

		/// <summary>
		/// Starts an instance of an actor.
		/// </summary>
		public abstract Task StartInstance();

		/// <summary>
		/// Finalizes an instance of an actor.
		/// </summary>
		public abstract Task FinalizeInstance();

		/// <summary>
		/// Registers an external event on the actor.
		/// </summary>
		/// <param name="ExternalEvent"></param>
		public void Register(ExternalEvent ExternalEvent)
		{
			string Name = ExternalEvent.Name;

			if (this.externalEvents is null)
				this.externalEvents = new Dictionary<string, ExternalEvent>();

			if (this.externalEvents.ContainsKey(Name))
				throw new Exception("External event named " + Name + " already registered on actor " + this.id);
			else
				this.externalEvents[Name] = ExternalEvent;
		}

		/// <summary>
		/// Tries to get an external event, given its name.
		/// </summary>
		/// <param name="Name">Name of external event.</param>
		/// <param name="ExternalEvent">External event object.</param>
		/// <returns>If an external event with the corresponding name was found.</returns>
		public bool TryGetExternalEvent(string Name, out ExternalEvent ExternalEvent)
		{
			if (this.externalEvents is null)
			{
				ExternalEvent = null;
				return false;
			}
			else
				return this.externalEvents.TryGetValue(Name, out ExternalEvent);
		}
	}
}