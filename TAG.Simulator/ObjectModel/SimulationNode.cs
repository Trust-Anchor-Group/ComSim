using System;
using System.Threading.Tasks;
using System.Xml;

namespace TAG.Simulator.ObjectModel
{
	/// <summary>
	/// Abstract base class for simulation nodes
	/// </summary>
	public abstract class SimulationNode : ISimulationNode
	{
		private readonly ISimulationNode parent;

		/// <summary>
		/// Abstract base class for simulation nodes
		/// </summary>
		/// <param name="Parent">Parent node</param>
		public SimulationNode(ISimulationNode Parent)
		{
			this.parent = Parent;
		}

		/// <summary>
		/// Parent node
		/// </summary>
		public ISimulationNode Parent => this.parent;

		/// <summary>
		/// XML Namespace where the element is defined.
		/// </summary>
		public virtual string Namespace => Model.ComSimNamespace;

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
		/// <returns>New instance</returns>
		public abstract ISimulationNode Create(ISimulationNode Parent);

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
		/// <param name="Model">Model being executed.</param>
		public virtual Task Initialize(Model Model)
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

	}
}
