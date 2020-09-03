using System;
using System.Threading.Tasks;
using System.Xml;
using Waher.Content.Xml;

namespace TAG.Simulator.ObjectModel.Structure
{
	/// <summary>
	/// Assembly reference.
	/// </summary>
	public class Assembly : SimulationNodeChildren
	{
		private string fileName;

		/// <summary>
		/// Assembly reference.
		/// </summary>
		public Assembly()
			: base()
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "Assembly";

		/// <summary>
		/// Filename of assembly.
		/// </summary>
		public string FileName => this.fileName;

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <returns>New instance</returns>
		public override ISimulationNode Create()
		{
			return new Assembly();
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.fileName = XML.Attribute(Definition, "fileName");

			return base.FromXml(Definition);
		}
	}
}
