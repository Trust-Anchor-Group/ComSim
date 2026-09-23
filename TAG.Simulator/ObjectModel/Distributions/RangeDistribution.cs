using System.Threading.Tasks;
using System.Xml;
using Waher.Content.Xml;

namespace TAG.Simulator.ObjectModel.Distributions
{
	/// <summary>
	/// Abstract base class for distributions that work on a given range.
	/// </summary>
	public abstract class RangeDistribution : Distribution
	{
		private double from;
		private double to;
		private bool inverted;

		/// <summary>
		/// Abstract base class for distributions that work on a given range.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public RangeDistribution(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// From
		/// </summary>
		public double From => this.from;

		/// <summary>
		/// To
		/// </summary>
		public double To => this.to;

		/// <summary>
		/// If the interval is inverted (with respect to the model time cycle).
		/// </summary>
		public bool Inverted => this.inverted;

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.from = XML.Attribute(Definition, "from", 0.0);
			this.to = XML.Attribute(Definition, "to", 0.0);
			this.inverted = this.from >= this.to;

			return base.FromXml(Definition);
		}
	}
}
