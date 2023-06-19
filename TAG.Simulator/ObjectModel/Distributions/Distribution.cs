using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Waher.Content;
using Waher.Content.Xml;

namespace TAG.Simulator.ObjectModel.Distributions
{
	/// <summary>
	/// Abstract base class for distributions
	/// </summary>
	public abstract class Distribution : SimulationNode, IDistribution
	{
		private string id;
		private double n;
		private double timeCycleUnits;

		/// <summary>
		/// Abstract base class for distributions
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public Distribution(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// ID of distribution.
		/// </summary>
		public string Id => this.id;

		/// <summary>
		/// Intensity/Frequency/Factor
		/// </summary>
		public double N => this.n;

		/// <summary>
		/// Time cycle, in number of <see cref="Model.TimeUnit"/>.
		/// </summary>
		public double TimeCycleUnits => this.timeCycleUnits;

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.id = XML.Attribute(Definition, "id");
			this.n = XML.Attribute(Definition, "N", 0.0);

			return Task.CompletedTask;
		}

		/// <summary>
		/// Initialized the node before simulation.
		/// </summary>
		public override Task Initialize()
		{
			this.Model.Register(this);
			this.timeCycleUnits = this.Model.TimeCycleUnits;

			return base.Initialize();
		}

		/// <summary>
		/// Check if distribution has a sample within the time period.
		/// </summary>
		/// <param name="t1">Starting time of period.</param>
		/// <param name="t2">Ending time of period.</param>
		/// <param name="NrCycles">Number of time cycles completed.</param>
		/// <returns>How many times samples were found in time period.</returns>
		public virtual int CheckTrigger(double t1, double t2, int NrCycles)
		{
			double p;

			if (t1 < t2)
				p = this.GetCumulativeProbability(t2, NrCycles) - this.GetCumulativeProbability(t1, NrCycles);
			else
			{
				p = this.GetCumulativeProbability(this.Model.TimeCycleUnits, NrCycles - 1) - this.GetCumulativeProbability(t1, NrCycles - 1);
				p += this.GetCumulativeProbability(t2, NrCycles) - this.GetCumulativeProbability(0, NrCycles);
			}

			if (p <= 0)
				return 0;

			p *= this.N;

			int Nr = (int)p;
			if (Nr > 0)
				p -= Nr;

			if (p > 0 && this.Model.GetRandomDouble() <= p)
				Nr++;

			return Nr;
		}

		/// <summary>
		/// The Cumulative Distribution Function (CDF) of the distribution, excluding intensity (<see cref="N"/>).
		/// </summary>
		/// <param name="t">Time</param>
		/// <param name="NrCycles">Number of time cycles completed.</param>
		/// <returns>CDU(t)</returns>
		protected abstract double GetCumulativeProbability(double t, int NrCycles);

		/// <summary>
		/// Exports the PDF function, if not already exported.
		/// </summary>
		/// <param name="Output">Export output</param>
		public void ExportPdfOnceOnly(StringBuilder Output)
		{
			if (!this.exported)
			{
				this.exported = true;
				this.ExportPdf(Output);
			}
		}

		private bool exported;

		/// <summary>
		/// Exports the PDF function.
		/// </summary>
		/// <param name="Output">Export output</param>
		public virtual void ExportPdf(StringBuilder Output)
		{
			Output.Append(this.id);
			Output.Append("PDF(t):=");
			if (this.n != 1)
			{
				Output.Append(CommonTypes.Encode(this.n));
				Output.Append('*');
			}
			Output.Append('(');
			this.ExportPdfBody(Output);
			Output.AppendLine(");");
		}

		/// <summary>
		/// Exports the PDF function body.
		/// </summary>
		/// <param name="Output">Export output</param>
		public abstract void ExportPdfBody(StringBuilder Output);

		/// <summary>
		/// Copies contents of the node to a new node.
		/// </summary>
		/// <param name="To">Node to receive copied contents.</param>
		public override void CopyContents(ISimulationNode To)
		{
			Distribution TypedTo = (Distribution)To;

			TypedTo.id = this.id;
			TypedTo.n = this.n;
			TypedTo.timeCycleUnits = this.timeCycleUnits;
		}

	}
}
