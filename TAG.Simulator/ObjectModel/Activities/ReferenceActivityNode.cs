using System;
using System.Threading.Tasks;
using System.Xml;
using Waher.Content.Xml;

namespace TAG.Simulator.ObjectModel.Activities
{
	/// <summary>
	/// Base class for activity nodes with a reference.
	/// </summary>
	public abstract class ReferenceActivityNode : ActivityNode 
	{
		private string reference;

		/// <summary>
		/// Base class for activity nodes with a reference.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public ReferenceActivityNode(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Reference
		/// </summary>
		public string Reference => this.reference;

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.reference = XML.Attribute(Definition, "ref");

			return Task.CompletedTask;
		}
	}
}
