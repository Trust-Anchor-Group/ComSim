using System;
using System.Threading.Tasks;
using System.Xml;
using Waher.Content.Xml;

namespace TAG.Simulator.ObjectModel.Distributions
{
	/// <summary>
	/// Uniform distribution
	/// </summary>
	public class Uniform : Distribution
	{
		private double from;
		private double to;

		/// <summary>
		/// Uniform distribution
		/// </summary>
		/// <param name="Parent">Parent node</param>
		public Uniform(ISimulationNode Parent)
			: base(Parent)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "Uniform";

		/// <summary>
		/// From
		/// </summary>
		public double From => this.from;

		/// <summary>
		/// To
		/// </summary>
		public double To => this.to;

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent)
		{
			return new Uniform(Parent);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.from = XML.Attribute(Definition, "from", 0.0);
			this.to = XML.Attribute(Definition, "to", 0.0);

			return base.FromXml(Definition);
		}
	}
}
