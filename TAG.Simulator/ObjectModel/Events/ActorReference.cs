using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel.Actors;
using Waher.Content.Xml;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Events
{
	/// <summary>
	/// References a population of actors.
	/// </summary>
	public class ActorReference : EventPreparation, IActors
	{
		private readonly List<IActor> actors = new List<IActor>();
		private IActor[] actorsStat;
		private int count;
		private string name;
		private string name2;
		private bool exclusive;

		/// <summary>
		/// References a population of actors.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public ActorReference(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
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
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new ActorReference(Parent, Model);
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

		/// <summary>
		/// Registers an actor with the collection of actors.
		/// </summary>
		/// <param name="Actor">Actor</param>
		public void Register(IActor Actor)
		{
			this.actors.Add(Actor);
		}

		/// <summary>
		/// Starts the node.
		/// </summary>
		public override Task Start()
		{
			this.actorsStat = this.actors.ToArray();
			this.count = this.actorsStat.Length;
			this.name2 = this.name + " Actor";

			return base.Start();
		}

		/// <summary>
		/// Prepares <paramref name="Variables"/> for the execution of an event.
		/// </summary>
		/// <param name="Variables">Event variables</param>
		/// <param name="Tags">Extensible list of meta-data tags related to the event.</param>
		public override Task Prepare(Variables Variables, List<KeyValuePair<string, object>> Tags)
		{
			IActor Actor;
			int[] P = new int[this.count];
			int i, j;

			lock (this.Model)
			{
				for (i = j = 0; i < this.count; i++)
				{
					j += this.actorsStat[i].FreeCount;
					P[i] = j;
				}

				if (j <= 0)
					throw new Exception("No free individual available in population.");

				j = this.Model.GetRandomInteger(j);
				i = 0;

				while (P[i] <= j)
					i++;

				if (i > 0)
					j -= P[i - 1];

				Actor = this.actorsStat[i].GetFreeIndividual(j, this.exclusive);
			}

			Variables[this.name2] = Actor;
			Variables[this.name] = Actor.ActivityObject;

			Tags.Add(new KeyValuePair<string, object>(this.name, Actor.InstanceId));

			return Task.CompletedTask;
		}

		/// <summary>
		/// Releases resources at the end of an event.
		/// </summary>
		/// <param name="Variables">Event variables</param>
		public override void Release(Variables Variables)
		{
			if (this.exclusive &&
				Variables.TryGetVariable(this.name2, out Variable v) &&
				v.ValueObject is IActor InstanceActor &&
				InstanceActor.Parent is IActor ActorPopulation)
			{
				lock (this.Model)
				{
					ActorPopulation.ReturnIndividual(InstanceActor);
				}

				Variables.Remove(this.name2);
			}
		}

		/// <summary>
		/// Exports the node to PlantUML script in a markdown document.
		/// </summary>
		/// <param name="Output">Output stream.</param>
		/// <param name="Name">Optional name for the association.</param>
		/// <param name="Index">Chart Index</param>
		public override void ExportPlantUml(StreamWriter Output, string Name, int Index)
		{
			int i = 0;

			foreach (IActor Actor in this.actors)
			{
				string s = this.name + "_" + Index.ToString() + "_A" + i.ToString();

				Output.Write("actor \"");
				Output.Write(this.name);
				Output.Write("\" as ");
				Output.Write(s);
				Output.Write(" <<");
				Output.Write(Actor.Id);
				Output.WriteLine(">>");

				Actor.AnnotateActorUseCaseUml(Output, s);

				Output.Write(s);
				Output.Write(" --> UC");
				Output.Write(Index.ToString());

				if (!string.IsNullOrEmpty(Name))
				{
					Output.Write(" : ");
					Output.Write(Name);
				}

				Output.WriteLine();

				i++;
			}
		}

	}
}
