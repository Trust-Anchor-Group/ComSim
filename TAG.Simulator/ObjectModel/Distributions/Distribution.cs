using System;
using System.Threading.Tasks;
using System.Xml;
using Waher.Content.Xml;

namespace TAG.Simulator.ObjectModel.Distributions
{
	/// <summary>
	/// Abstract base class for distributions
	/// </summary>
	public abstract class Distribution : SimulationNode
	{
		private Model model;
		private string id;
		private double n;
		private double timeCycleUnits;

		/// <summary>
		/// Abstract base class for distributions
		/// </summary>
		/// <param name="Parent">Parent node</param>
		public Distribution(ISimulationNode Parent)
			: base(Parent)
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
		/// Model hosting the distribution
		/// </summary>
		public Model Model => this.model;

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
		/// <param name="Model">Model being executed.</param>
		public override Task Initialize(Model Model)
		{
			this.model = Model;
			this.model.Register(this);
			this.timeCycleUnits = Model.TimeCycleUnits;

			return base.Initialize(Model);
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
				p = this.GetCumulativeProbability(this.model.TimeCycleUnits, NrCycles - 1) - this.GetCumulativeProbability(t1, NrCycles - 1);
				p += this.GetCumulativeProbability(t2, NrCycles) - this.GetCumulativeProbability(0, NrCycles);
			}

			if (p <= 0)
				return 0;

			p *= this.N;

			int Nr = (int)p;
			if (Nr > 0)
				p -= Nr;

			if (p > 0 && this.model.GetRandomDouble() <= p)
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

	}
}
