using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;

namespace TAG.Simulator
{
	/// <summary>
	/// Callback method for iteration across the simulation model.
	/// </summary>
	/// <param name="Node">Current node being processed.</param>
	public delegate Task ForEachCallbackMethod(ISimulationNode Node);

	/// <summary>
	/// Basic interface for simulator nodes. Implementing this interface allows classes with default contructors to be used
	/// in simulator models.
	/// </summary>
	public interface ISimulationNode
	{
		/// <summary>
		/// Parent node in the simulation model.
		/// </summary>
		ISimulationNode Parent
		{
			get;
		}

		/// <summary>
		/// XML Namespace where the element is defined.
		/// </summary>
		string Namespace
		{
			get;
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		string LocalName
		{
			get;
		}

		/// <summary>
		/// Points to the embedded XML Schema resource defining the semantics of the XML namespace.
		/// </summary>
		string SchemaResource
		{
			get;
		}

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model owning the node.</param>
		/// <returns>New instance</returns>
		ISimulationNode Create(ISimulationNode Parent, Model Model);

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		Task FromXml(XmlElement Definition);

		/// <summary>
		/// Evaluates <paramref name="Method"/> on each node in the subtree defined by the current node.
		/// </summary>
		/// <param name="Method">Method to call.</param>
		/// <param name="DepthFirst">If children are iterated before parents.</param>
		Task ForEach(ForEachCallbackMethod Method, bool DepthFirst);

		/// <summary>
		/// Initialized the node before simulation.
		/// </summary>
		Task Initialize();

		/// <summary>
		/// Starts the node.
		/// </summary>
		Task Start();

		/// <summary>
		/// Finalizes the node after simulation.
		/// </summary>
		Task Finalize();

		/// <summary>
		/// Exports Markdown
		/// </summary>
		/// <param name="Output">Output node</param>
		Task ExportMarkdown(StreamWriter Output);

		/// <summary>
		/// Exports XML
		/// </summary>
		/// <param name="Output">Output node</param>
		Task ExportXml(XmlWriter Output);
	}
}
