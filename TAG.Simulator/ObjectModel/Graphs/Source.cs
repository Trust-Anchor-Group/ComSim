using System.Threading.Tasks;
using System.Xml;
using Waher.Content.Xml;

namespace TAG.Simulator.ObjectModel.Graphs
{
	/// <summary>
	/// Graph source reference.
	/// </summary>
	public class Source : SimulationNode, ISource
	{
		private string @ref;

		/// <summary>
		/// Graph source reference.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public Source(ISimulationNode Parent, Model Model) 
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "Source";

		/// <summary>
		/// Reference
		/// </summary>
		public string Reference => this.@ref;

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new Source(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.@ref = XML.Attribute(Definition, "ref");

			return Task.CompletedTask;
		}

		/// <summary>
		/// Initialized the node before simulation.
		/// </summary>
		public override Task Initialize()
		{
			if (this.Parent is ISourceRecipient SourceRecipient)
				SourceRecipient.Register(this);

			return base.Initialize();
		}

		/// <summary>
		/// Copies contents of the node to a new node.
		/// </summary>
		/// <param name="To">Node to receive copied contents.</param>
		public override void CopyContents(ISimulationNode To)
		{
			Source TypedTo = (Source)To;

			TypedTo.@ref = this.@ref;
		}
	}
}
