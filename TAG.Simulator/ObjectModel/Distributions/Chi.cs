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
	/// Chi distribution
	/// </summary>
	public class Chi : Distribution
	{
		/// <summary>
		/// Time of start of distribution
		/// </summary>
		protected double t0;

		/// <summary>
		/// k parameter
		/// </summary>
		protected double k;

		/// <summary>
		/// k/2
		/// </summary>
		protected double kHalf;

		/// <summary>
		/// 1/gamma(k/2)
		/// </summary>
		protected double c;

		/// <summary>
		/// Chi distribution
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public Chi(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "Chi";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new Chi(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.t0 = XML.Attribute(Definition, "t0", 0.0);
			this.k = XML.Attribute(Definition, "k", 0.0);

			this.kHalf = this.k / 2;
			this.c = 1.0 / StatMath.Γ(this.kHalf);

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
				return StatMath.γ(this.kHalf, t * t / 2) * this.c;
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
			Output.Append(CommonTypes.Encode(this.c * Math.Pow(2, 1 - this.kHalf)));
			Output.Append("*(t-");
			Output.Append(CommonTypes.Encode(this.t0));
			Output.Append(").^");
			Output.Append(CommonTypes.Encode(this.k - 1));
			Output.Append(".*exp(-(t-");
			Output.Append(CommonTypes.Encode(this.t0));
			Output.Append(").^2/2)");
		}

		/// <summary>
		/// Copies contents of the node to a new node.
		/// </summary>
		/// <param name="To">Node to receive copied contents.</param>
		public override void CopyContents(ISimulationNode To)
		{
			Chi TypedTo = (Chi)To;

			TypedTo.t0 = this.t0;
			TypedTo.k = this.k;
			TypedTo.kHalf = this.kHalf;
			TypedTo.c = this.c;

			base.CopyContents(To);
		}

	}
}
