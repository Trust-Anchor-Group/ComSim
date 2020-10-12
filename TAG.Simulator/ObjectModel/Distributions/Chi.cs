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
		private double t0;
		private double k;
		private double kHalf;
		private double c;

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

	}
}
