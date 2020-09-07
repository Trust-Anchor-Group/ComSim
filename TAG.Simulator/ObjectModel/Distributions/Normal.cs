using System;
using System.Threading.Tasks;
using System.Xml;
using Waher.Content.Xml;

namespace TAG.Simulator.ObjectModel.Distributions
{
	/// <summary>
	/// Normal distribution
	/// </summary>
	public class Normal : Distribution
	{
		private double μ;
		private double σ;

		/// <summary>
		/// Normal distribution
		/// </summary>
		/// <param name="Parent">Parent node</param>
		public Normal(ISimulationNode Parent)
			: base(Parent)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "Normal";

		/// <summary>
		/// μ
		/// </summary>
		public double Mean => this.μ;

		/// <summary>
		/// σ
		/// </summary>
		public double StdDev => this.σ;

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent)
		{
			return new Normal(Parent);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.μ = XML.Attribute(Definition, "μ", 0.0);
			this.σ = XML.Attribute(Definition, "σ", 0.0);

			return base.FromXml(Definition);
		}
	}
}
