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
		private bool inverted;

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
		/// If the interval is inverted (with respect to the model time cycle).
		/// </summary>
		public bool Inverted => this.inverted;

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
			this.inverted = this.from >= this.to;

			return base.FromXml(Definition);
		}

		/// <summary>
		/// The Cumulative Distribution Function (CDF) of the distribution, excluding intensity (<see cref="Distribution.N"/>).
		/// </summary>
		/// <param name="t">Time</param>
		/// <param name="NrCycles">Number of time cycles completed.</param>
		/// <returns>CDU(t)</returns>
		protected override double GetCumulativeProbability(double t, int NrCycles)
		{
			if (this.inverted)
			{
				if (t >= this.from)
					return (t - this.from) / (this.TimeCycleUnits + this.to - this.from) + NrCycles;
				else if (t <= this.to)
					return (this.TimeCycleUnits + t - this.from) / (this.TimeCycleUnits + this.to - this.from) + NrCycles;
				else
					return NrCycles;
			}
			else
			{
				if (t <= this.from)
					return NrCycles;
				else if (t >= this.to)
					return NrCycles + 1;
				else
					return (t - this.from) / (this.to - this.from) + NrCycles;
			}
		}

	}
}
