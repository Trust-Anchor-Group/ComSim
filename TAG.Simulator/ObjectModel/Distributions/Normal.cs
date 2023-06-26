using System;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Waher.Content;
using Waher.Content.Xml;
using Waher.Script.Statistics;

namespace TAG.Simulator.ObjectModel.Distributions
{
	/// <summary>
	/// Normal distribution
	/// </summary>
	public class Normal : Distribution
	{
		private double μ;
		private double σ;
		private double σSqrt2;

		/// <summary>
		/// Normal distribution
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public Normal(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(Normal);

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
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new Normal(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.μ = XML.Attribute(Definition, "μ", 0.0);
			this.σ = XML.Attribute(Definition, "σ", 0.0);
			this.σSqrt2 = this.σ * Math.Sqrt(2);

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
			if (Math.Abs(t - this.μ) > Math.Abs(this.TimeCycleUnits + t - this.μ))
			{
				t += this.TimeCycleUnits;
				NrCycles--;
			}

			double x = (t - this.μ) / this.σSqrt2;
			double erf = x < -5 ? -1 : x > 5 ? 1 : StatMath.Erf(x);

			return ((1 + erf) / 2) + NrCycles;
		}

		/// <summary>
		/// Exports the PDF function body.
		/// </summary>
		/// <param name="Output">Export output</param>
		public override void ExportPdfBody(StringBuilder Output)
		{
			string s;

			Output.Append("exp(-(((t-");
			Output.Append(CommonTypes.Encode(this.μ));
			Output.Append(")/");
			Output.Append(s = CommonTypes.Encode(this.σ));
			Output.Append(").^2)/2)/(");
			Output.Append(s);
			Output.Append("*sqrt(2*pi))");
		}

	}
}
