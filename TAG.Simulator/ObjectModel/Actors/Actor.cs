﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel.Events;
using Waher.Content.Xml;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Actors
{
	/// <summary>
	/// Abstract base class for actors
	/// </summary>
	public abstract class Actor : ExternalEventsNode, IActor
	{
		private readonly Variables variables;
		private List<IActor> freeIndividuals = null;
		private IActor[] instances;
		private string id;
		private int n;
		private readonly string instanceId;
		private readonly int instanceIndex;

		/// <summary>
		/// Abstract base class for actors
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public Actor(ISimulationNode Parent, Model Model)
			: this(Parent, Model, 0, string.Empty)
		{
		}

		/// <summary>
		/// Abstract base class for actors
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <param name="InstanceIndex">Instance index.</param>
		/// <param name="InstanceId">ID of instance</param>
		public Actor(ISimulationNode Parent, Model Model, int InstanceIndex, string InstanceId)
			: base(Parent, Model)
		{
			this.instanceIndex = InstanceIndex;
			this.instanceId = InstanceId;
			this.variables = new Variables(new Variable("this", this));
		}

		/// <summary>
		/// ID of actor.
		/// </summary>
		public override string Id => this.id;

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
		/// Collection of actor-variables.
		/// </summary>
		public Variables Variables => this.variables;

		/// <summary>
		/// References to created instances.
		/// </summary>
		public IActor[] Instances => this.instances;

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
		public override async Task Initialize()
		{
			await base.Initialize();

			this.Model.Register(this);

			if (this.Parent is IActors Actors)
				Actors.Register(this);

			int NrDigits = 1 + (int)Math.Log10(this.N);
			string Format = "D" + NrDigits.ToString();
			int i;

			this.instances = new Actor[this.N];

			for (i = 1; i <= this.n; i++)
			{
				Actor Instance = await this.CreateInstanceAsync(i, this.id + i.ToString(Format));
				Instance.id = this.id;
				Instance.n = this.n;
				Instance.externalEvents = this.externalEvents;

				this.instances[i - 1] = Instance;
				this.Model.Variables[Instance.instanceId] = Instance;

				await Instance.InitializeInstance();
			}

			this.freeIndividuals = new List<IActor>();
			this.freeIndividuals.AddRange(this.instances);
		}

		/// <summary>
		/// Starts the node.
		/// </summary>
		public override async Task Start()
		{
			await base.Start();

			Console.Out.Write(this.id);

			foreach (IActor Actor in this.instances)
			{
				if (Actor is Actor Actor2)
					await Actor2.StartInstance();

				Console.Out.Write('.');
			}

			Console.Out.WriteLine();
		}

		/// <summary>
		/// Finalizes the node after simulation.
		/// </summary>
		public override async Task Finalize()
		{
			foreach (IActor Actor in this.instances)
			{
				if (Actor is Actor Actor2)
					await Actor2.FinalizeInstance();

				if (Actor is IDisposable Disposable)
					Disposable.Dispose();
			}

			this.instances = null;

			await base.Finalize();
		}

		/// <summary>
		/// Creates an instance of the actor.
		/// 
		/// Note: Parent of newly created actor should point to this node (the creator of the instance object).
		/// </summary>
		/// <param name="InstanceIndex">Instance index.</param>
		/// <param name="InstanceId">ID of instance</param>
		/// <returns>Actor instance.</returns>
		public abstract Task<Actor> CreateInstanceAsync(int InstanceIndex, string InstanceId);

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
		/// Number of individuals in population that are free.
		/// </summary>
		public int FreeCount => this.freeIndividuals?.Count ?? 0;

		/// <summary>
		/// Gets a free individual instance from the population.
		/// </summary>
		/// <param name="Index">Zero-based index of individual to get.</param>
		/// <param name="Exclusive">If individual is for exclusive use (i.e. will not be free once gotten, until returned).</param>
		/// <returns>Individual instance returned.</returns>
		public IActor GetFreeIndividual(int Index, bool Exclusive)
		{
			IActor Result = this.freeIndividuals[Index];

			if (Exclusive)
				this.freeIndividuals.RemoveAt(Index);

			return Result;
		}

		/// <summary>
		/// Returns an individual to the population, once free again.
		/// </summary>
		/// <param name="Individual">Individual to return.</param>
		public void ReturnIndividual(IActor Individual)
		{
			this.freeIndividuals.Add(Individual);
		}

		/// <summary>
		/// Returns the object that will be used by the actor for actions during an activity.
		/// </summary>
		public virtual object ActivityObject => this;
	}
}
