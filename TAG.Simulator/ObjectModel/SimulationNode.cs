using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel.Actors;
using Waher.Script;

namespace TAG.Simulator.ObjectModel
{
	/// <summary>
	/// Abstract base class for simulation nodes
	/// </summary>
	public abstract class SimulationNode : ISimulationNode
	{
		private readonly ISimulationNode parent;
		private readonly Model model;

		/// <summary>
		/// Abstract base class for simulation nodes
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public SimulationNode(ISimulationNode Parent, Model Model)
		{
			this.parent = Parent;
			this.model = Model;
		}

		/// <summary>
		/// Parent node in the simulation model.
		/// </summary>
		public ISimulationNode Parent => this.parent;

		/// <summary>
		/// Model in which the node is defined.
		/// </summary>
		public Model Model
		{
			get => this.model;
		}

		/// <summary>
		/// XML Namespace where the element is defined.
		/// </summary>
		public virtual string Namespace => Model.ComSimNamespace;

		/// <summary>
		/// Points to the embedded XML Schema resource defining the semantics of the XML namespace.
		/// </summary>
		public virtual string SchemaResource => "TAG.Simulator.Schema.ComSim.xsd";

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public abstract string LocalName
		{
			get;
		}

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public abstract ISimulationNode Create(ISimulationNode Parent, Model Model);

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public abstract Task FromXml(XmlElement Definition);

		/// <summary>
		/// Evaluates <paramref name="Method"/> on each node in the subtree defined by the current node.
		/// </summary>
		/// <param name="Method">Method to call.</param>
		/// <param name="DepthFirst">If children are iterated before parents.</param>
		public virtual Task ForEach(ForEachCallbackMethod Method, bool DepthFirst)
		{
			return Method(this);
		}

		/// <summary>
		/// Initialized the node before simulation.
		/// </summary>
		public virtual Task Initialize()
		{
			return Task.CompletedTask;
		}

		/// <summary>
		/// Starts the node.
		/// </summary>
		public virtual Task Start()
		{
			return Task.CompletedTask;
		}

		/// <summary>
		/// Finalizes the node after simulation.
		/// </summary>
		public virtual Task Finalize()
		{
			return Task.CompletedTask;
		}

		/// <summary>
		/// Exports Markdown
		/// </summary>
		/// <param name="Output">Output</param>
		public virtual Task ExportMarkdown(StreamWriter Output)
		{
			return Task.CompletedTask;
		}

		/// <summary>
		/// Exports XML
		/// </summary>
		/// <param name="Output">Output</param>
		public virtual Task ExportXml(XmlWriter Output)
		{
			return Task.CompletedTask;
		}

		/// <summary>
		/// Adds indentation to the current row.
		/// </summary>
		/// <param name="Output">Output.</param>
		/// <param name="Indentation">Number of tabs to indent.</param>
		protected static void Indent(StreamWriter Output, int Indentation)
		{
			if (Indentation > 0)
				Output.Write(new string('\t', Indentation));
		}

		/// <summary>
		/// Gets an actor object, given a string representation, possibly containing script, of the actor.
		/// </summary>
		/// <param name="Actor">Actor string representation.</param>
		/// <param name="Variables">Current set of variables.</param>
		/// <returns>Actor</returns>
		/// <exception cref="Exception">If actor could not be found.</exception>
		public async Task<object> GetActorObjectAsync(StringAttribute Actor, Variables Variables)
		{
			string ActorStr = await Actor.GetValueAsync(Variables);
			object Result;

			if (Variables.TryGetVariable(ActorStr, out Variable v))
				Result = v.ValueObject;
			else
			{
				Expression Exp;

				lock (this.synchObj)
				{
					if (this.lastActorExpression is null || ActorStr != this.lastActor)
					{
						this.lastActorExpression = new Expression(ActorStr);
						this.lastActor = ActorStr;
					}

					Exp = this.lastActorExpression;
				}

				Result = await Exp.EvaluateAsync(Variables);
			}

			if (Result is Actor Actor2)
				return Actor2.ActivityObject;
			else
				return Result;
		}

		/// <summary>
		/// Gets an actor, given a string representation, possibly containing script, of the actor.
		/// </summary>
		/// <param name="Actor">Actor string representation.</param>
		/// <param name="Variables">Current set of variables.</param>
		/// <returns>Actor</returns>
		/// <exception cref="Exception">If actor could not be found.</exception>
		public async Task<IActor> GetActorAsync(StringAttribute Actor, Variables Variables)
		{
			if (await this.GetActorObjectAsync(Actor, Variables) is IActor ActorRef)
				return ActorRef;
			else
				throw new Exception("Expected an actor: " + Actor);
		}

		private string lastActor = null;
		private Expression lastActorExpression = null;
		private readonly object synchObj = new object();

		/// <summary>
		/// Copies the node.
		/// </summary>
		/// <returns>Copy</returns>
		public virtual ISimulationNode Copy()
		{
			ISimulationNode Result = this.Create(this.parent, this.model);
			this.CopyContents(Result);
			return Result;
		}

		/// <summary>
		/// Copies contents of the node to a new node.
		/// </summary>
		/// <param name="To">Node to receive copied contents.</param>
		public abstract void CopyContents(ISimulationNode To);

		/// <summary>
		/// Copies an array of simulation nodes.
		/// </summary>
		/// <typeparam name="T">Type of simulation node.</typeparam>
		/// <param name="Nodes">Nodes</param>
		/// <returns>Copies of the nodes.</returns>
		public static T[] Copy<T>(T[] Nodes)
			where T : ISimulationNode
		{
			int i, c = Nodes.Length;
			T[] Result = new T[c];

			for (i = 0; i < c; i++)
				Result[i] = (T)Nodes[i].Copy();

			return Result;
		}
	}
}
