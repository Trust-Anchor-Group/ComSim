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
		/// <summary>
		/// Abstract base class for simulation nodes
		/// </summary>
		public SimulationNode()
		{
		}

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
		/// <returns>New instance</returns>
		public abstract ISimulationNode Create();

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public abstract Task FromXml(XmlElement Definition);
	}
}
