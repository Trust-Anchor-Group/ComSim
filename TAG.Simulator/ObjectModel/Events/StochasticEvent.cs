﻿using System;
using System.IO;
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
		private IDistribution distribution;
		private Expression guard;
		private int guardLimit;

		/// <summary>
		/// Stochastic Event
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public StochasticEvent(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(StochasticEvent);

		/// <summary>
		/// ID of Distribution
		/// </summary>
		public string DistributionId => this.distributionId;

		/// <summary>
		/// Distribution
		/// </summary>
		public override IDistribution Distribution => this.distribution;

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new StochasticEvent(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.distributionId = XML.Attribute(Definition, "distribution");
			this.guardLimit = XML.Attribute(Definition, "guardLimit", 1000);

			if (Definition.HasAttribute("guard"))
				this.guard = new Expression(Definition.GetAttribute("guard"));
			else
				this.guard = null;

			return base.FromXml(Definition);
		}

		/// <summary>
		/// Initialized the node before simulation.
		/// </summary>
		public override Task Initialize()
		{
			if (!this.Model.TryGetDistribution(this.distributionId, out this.distribution))
				throw new Exception("Distribution not found: " + this.distributionId);

			return base.Initialize();
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
				Task.Run(() => this.Trigger(this.Model.GetEventVariables(null), this.guard, this.guardLimit));
				n--;
			}
		}

		/// <summary>
		/// Name of use case association.
		/// </summary>
		public override string UseCaseLinkName => this.distributionId;

		/// <summary>
		/// Exports use case diagram data.
		/// </summary>
		/// <param name="Output">Output stream.</param>
		/// <param name="Index">Chart Index</param>
		public override void ExportUseCaseData(StreamWriter Output, int Index)
		{
			base.ExportUseCaseData(Output, Index);

			if (!(this.guard is null))
			{
				Output.Write("note left of UC");
				Output.WriteLine(Index.ToString());
				Output.Write(this.distributionId);
				Output.WriteLine(" is subject to conditions defined by script");
				Output.WriteLine("end note");
			}
		}

	}
}
