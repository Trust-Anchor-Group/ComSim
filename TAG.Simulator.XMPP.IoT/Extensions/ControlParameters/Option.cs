using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel;
using Waher.Content.Xml;

namespace TAG.Simulator.XMPP.IoT.Extensions.ControlParameters
{
	/// <summary>
	/// String option, with label.
	/// </summary>
	public class Option : SimulationNode
	{
		private string value;
		private string label;

		/// <summary>
		/// String option, with label.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public Option(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Option value.
		/// </summary>
		public string Value => this.value;

		/// <summary>
		/// Associated label
		/// </summary>
		public string Label => this.label;

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "Option";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new Option(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.value = Definition.InnerText;
			this.label = XML.Attribute(Definition, "label");

			return Task.CompletedTask;
		}

		/// <summary>
		/// Copies contents of the node to a new node.
		/// </summary>
		/// <param name="To">Node to receive copied contents.</param>
		public override void CopyContents(ISimulationNode To)
		{
			Option TypedTo = (Option)To;

			TypedTo.value = this.value;
			TypedTo.label = this.label;
		}

	}
}
