using System.Text;
using Waher.Content;

namespace TAG.Simulator.ObjectModel.Distributions
{
	/// <summary>
	/// Linearly decreasing distribution
	/// </summary>
	public class LinearDecrease : RangeDistribution
	{
		/// <summary>
		/// Linearly decreasing distribution
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public LinearDecrease(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(LinearDecrease);

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new LinearDecrease(Parent, Model);
		}

		/// <summary>
		/// The Cumulative Distribution Function (CDF) of the distribution, excluding intensity (<see cref="Distribution.N"/>).
		/// </summary>
		/// <param name="t">Time</param>
		/// <param name="NrCycles">Number of time cycles completed.</param>
		/// <returns>CDU(t)</returns>
		public override double GetCumulativeProbability(double t, int NrCycles)
		{
			double t0 = this.From;
			double t1 = this.To;
			double Δ = t1 - t0;
			double r;

			if (this.Inverted)  // t1 < t0
			{
				Δ += this.TimeCycleUnits;

				if (t <= t1)
				{
					t = t1 - t;
					return (t1 * t1 - t * t) / (Δ * Δ) + NrCycles;
				}
				else if (t < t0)
					return t1 * t1 / (Δ * Δ) + NrCycles;
				else
				{
					t = t1 + this.TimeCycleUnits - t;

					return 1.0 + (t1 * t1 - t * t) / (Δ * Δ) + NrCycles;
				}
			}
			else
			{
				if (t <= t0)
					return NrCycles;
				else if (t >= t1)
					return NrCycles + 1;
				else
				{
					t = t1 - t;
					return 1.0 - t * t / (Δ * Δ) + NrCycles;
				}
			}
		}

		/// <summary>
		/// Exports the PDF function body.
		/// </summary>
		/// <param name="Output">Export output</param>
		public override void ExportPdfBody(StringBuilder Output)
		{
			if (this.Inverted)
			{
				Output.Append("t<=");
				Output.Append(CommonTypes.Encode(this.To));
				Output.Append(" ? 2*(");
				Output.Append(CommonTypes.Encode(this.To));
				Output.Append("-t)/");
				Output.Append(CommonTypes.Encode(this.TimeCycleUnits - (this.From - this.To)));
				Output.Append("^2 : t>=");
				Output.Append(CommonTypes.Encode(this.From));
				Output.Append(" ? 2*(");
				Output.Append(CommonTypes.Encode(this.TimeCycleUnits + this.To));
				Output.Append("-t)/");
				Output.Append(CommonTypes.Encode(this.TimeCycleUnits - (this.From - this.To)));
				Output.Append("^2 : 0");
			}
			else
			{
				Output.Append("t>=");
				Output.Append(CommonTypes.Encode(this.From));
				Output.Append(" and t<=");
				Output.Append(CommonTypes.Encode(this.To));
				Output.Append(" ? 2*(");
				Output.Append(CommonTypes.Encode(this.To));
				Output.Append("-t)/");
				Output.Append(CommonTypes.Encode(this.To - this.From));
				Output.Append("^2 : 0");
			}
		}
	}
}
