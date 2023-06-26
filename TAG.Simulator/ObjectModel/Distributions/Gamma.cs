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
	/// Gamma distribution
	/// </summary>
	public class Gamma : Distribution
	{
		private double t0;
		private double α;
		private double β;
		private double θ;
		private double k;
		private double μ;
		private double invGammaAlpha;

		/// <summary>
		/// Gamma distribution
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public Gamma(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(Gamma);

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new Gamma(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.t0 = XML.Attribute(Definition, "t0", 0.0);

			if (Definition.HasAttribute("k") && Definition.HasAttribute("θ"))
			{
				this.k = XML.Attribute(Definition, "k", 0.0);
				this.θ = XML.Attribute(Definition, "θ", 0.0);

				if (Definition.HasAttribute("α") || Definition.HasAttribute("β") || Definition.HasAttribute("μ"))
					throw new Exception("Too many parameters in Gamma distribution definition.");

				this.α = this.k;
				this.β = 1 / this.θ;
				this.μ = this.k * this.θ;
			}
			else if (Definition.HasAttribute("α") && Definition.HasAttribute("β"))
			{
				this.α = XML.Attribute(Definition, "α", 0.0);
				this.β = XML.Attribute(Definition, "β", 0.0);

				if (Definition.HasAttribute("k") || Definition.HasAttribute("θ") || Definition.HasAttribute("μ"))
					throw new Exception("Too many parameters in Gamma distribution definition.");

				this.k = this.α;
				this.θ = 1 / this.β;
				this.μ = this.α / this.β;
			}
			else if (Definition.HasAttribute("k") && Definition.HasAttribute("μ"))
			{
				this.k = XML.Attribute(Definition, "k", 0.0);
				this.μ = XML.Attribute(Definition, "μ", 0.0);

				if (Definition.HasAttribute("α") || Definition.HasAttribute("β") || Definition.HasAttribute("θ"))
					throw new Exception("Too many parameters in Gamma distribution definition.");

				this.α = this.k;
				this.β = this.α / this.μ;
				this.θ = this.μ / this.k;
			}

			this.invGammaAlpha = 1.0 / StatMath.Γ(this.α);

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
			t -= this.t0;
			if (t < 0)
				return 0;
			else
				return StatMath.γ(this.α, this.β * t) * this.invGammaAlpha;
		}

		/// <summary>
		/// Exports the PDF function body.
		/// </summary>
		/// <param name="Output">Export output</param>
		public override void ExportPdfBody(StringBuilder Output)
		{
			Output.Append("t<");
			Output.Append(CommonTypes.Encode(this.t0));
			Output.Append("?0:");
			Output.Append(CommonTypes.Encode(Math.Pow(this.β, this.α) * this.invGammaAlpha));
			Output.Append("*(t-");
			Output.Append(CommonTypes.Encode(this.t0));
			Output.Append(").^");
			Output.Append(CommonTypes.Encode(this.α - 1));
			Output.Append(".*exp(-");
			Output.Append(CommonTypes.Encode(this.β));
			Output.Append("*(t-");
			Output.Append(CommonTypes.Encode(this.t0));
			Output.Append("))");
		}

	}
}
