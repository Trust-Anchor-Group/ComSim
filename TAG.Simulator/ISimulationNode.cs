using System;
using System.Threading.Tasks;
using System.Xml;

namespace TAG.Simulator
{
	/// <summary>
	/// Basic interface for simulator nodes. Implementing this interface allows classes with default contructors to be used
	/// in simulator models.
	/// </summary>
	public interface ISimulationNode
	{
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
		/// Creates a new instance of the node.
		/// </summary>
		/// <returns>New instance</returns>
		ISimulationNode Create();

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		Task FromXml(XmlElement Definition);
	}
}
