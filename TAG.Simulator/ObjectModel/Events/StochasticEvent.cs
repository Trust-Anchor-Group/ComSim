using System;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel.Distributions;
using Waher.Content.Xml;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Events
{
	/// <summary>
	/// Stochastic Event
	/// </summary>
	public class StochasticEvent : Event, ITimeTriggerEvent
	{
		private string distributionId;
		private Distribution distribution;

		/// <summary>
		/// Stochastic Event
		/// </summary>
		/// <param name="Parent">Parent node</param>
		public StochasticEvent(ISimulationNode Parent)
			: base(Parent)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "StochasticEvent";

		/// <summary>
		/// ID of Distribution
		/// </summary>
		public string DistributionId => this.distributionId;

		/// <summary>
		/// Distribution
		/// </summary>
		public Distribution Distribution => this.distribution;

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent)
		{
			return new StochasticEvent(Parent);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.distributionId = XML.Attribute(Definition, "distribution");

			return base.FromXml(Definition);
		}

		/// <summary>
		/// Initialized the node before simulation.
		/// </summary>
		/// <param name="Model">Model being executed.</param>
		public override Task Initialize(Model Model)
		{
			if (!Model.TryGetDistribution(this.distributionId, out this.distribution))
				throw new Exception("Distribution not found: " + this.distributionId);

			return base.Initialize(Model);
		}

		/// <summary>
		/// Check if event is triggered during a time period.
		/// </summary>
		/// <param name="t1">Starting time of period.</param>
		/// <param name="t2">Ending time of period.</param>
		/// <param name="NrCycles">Number of time cycles completed.</param>
		public void CheckTrigger(double t1, double t2, int NrCycles)
		{
			int n = this.distribution.CheckTrigger(t1, t2, NrCycles);
			while (n > 0)
			{
				this.Trigger(new Variables());
				n--;
			}
		}

	}
}
