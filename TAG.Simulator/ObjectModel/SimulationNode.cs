using System;
using System.IO;
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
		/// <param name="Output">Output node</param>
		public virtual Task ExportMarkdown(StreamWriter Output)
		{
			return Task.CompletedTask;
		}

		/// <summary>
		/// Exports XML
		/// </summary>
		/// <param name="Output">Output node</param>
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

	}
}
