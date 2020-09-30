using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel.Distributions;
using Waher.Content;
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
		public override string LocalName => "StochasticEvent";

		/// <summary>
		/// ID of Distribution
		/// </summary>
		public string DistributionId => this.distributionId;

		/// <summary>
		/// Distribution
		/// </summary>
		public IDistribution Distribution => this.distribution;

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
				this.Trigger(this.Model.GetEventVariables(null), this.guard, this.guardLimit);
				n--;
			}
		}

		/// <summary>
		/// Exports use case diagram data.
		/// </summary>
		/// <param name="Output">Output stream.</param>
		public override void ExportUseCaseData(StreamWriter Output)
		{
			base.ExportUseCaseData(Output);

			Output.WriteLine("note right of UC1");
			Output.Write("Triggered in accordance with distribution ");
			Output.WriteLine(this.distributionId);
			Output.WriteLine("end note");

			if (!(this.guard is null))
			{
				Output.WriteLine("note left of UC1");
				Output.WriteLine("Additional conditions, guarded by script");
				Output.WriteLine("end note");
			}
		}

		/// <summary>
		/// Exports Probability Script graph.
		/// </summary>
		/// <param name="Output">Output</param>
		/// <returns>If a PDF graph was added.</returns>
		public override bool ExportPdfScript(StringBuilder Output)
		{
			if (!(this.distribution is null))
			{
				double t2 = Model.GetTimeCoordinates(this.Model.EndTime);
				double dt = t2 / 1000;
				string s;

				Output.Append("+(");
				this.distribution.ExportPdfOnceOnly(Output);

				Output.Append("t:=0..");
				Output.Append(CommonTypes.Encode(t2));
				Output.Append("|");
				Output.Append(CommonTypes.Encode(dt));
				Output.AppendLine(";");

				if (this.Model.TimeCycleUnits != 1)
				{
					Output.Append("ct:=t/");
					Output.Append(s = CommonTypes.Encode(this.Model.TimeCycleUnits));
					Output.AppendLine(";");
					Output.Append("ct:=(ct-floor(ct))*");
					Output.Append(s);
					Output.AppendLine(";");
				}
				else
					Output.AppendLine("ct:=t-floor(t);");

				Output.Append("y:=");
				Output.Append(this.distributionId);
				Output.Append("PDF(ct)");

				if (this.Model.BucketTimeMs != this.Model.TimeUnitMs)
				{
					Output.Append(".*");
					Output.Append(CommonTypes.Encode(this.Model.BucketTimeMs / this.Model.TimeUnitMs));
				}

				Output.AppendLine(";");
				Output.AppendLine("G:=plot2dline(t,y,\"Blue\",3);");
				Output.Append("G.LabelX:=\"Time × ");
				Output.Append(this.Model.TimeUnitStr);
				Output.AppendLine("\";");
				Output.Append("G.LabelY:=\"Intensity (/ ");
				Output.Append(this.Model.TimeUnitStr);
				Output.AppendLine(")\";");
				Output.Append("G.Title:=\"Probability Density Function of ");
				Output.Append(this.distributionId);
				Output.AppendLine("\";");
				Output.AppendLine("G");
				Output.Append(")");

				return true;
			}
			else
				return false;
		}

	}
}
