using System;
using System.IO;
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
				this.Trigger(this.Model.GetEventVariables(), this.guard, this.guardLimit);
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
		/// Exports PlantUML
		/// </summary>
		/// <param name="Output">Output node</param>
		public override async Task ExportMarkdown(StreamWriter Output)
		{
			await base.ExportMarkdown(Output);

			if (!(this.distribution is null))
			{
				double t2 = Model.GetTimeCoordinates(this.Model.EndTime);
				double dt = t2 / 1000;
				string s;

				Output.WriteLine("{");
				Output.WriteLine("GraphWidth:=1000;");
				Output.WriteLine("GraphHeight:=400;");

				this.distribution.ExportPdfOnceOnly(Output);

				Output.Write("t:=0..");
				Output.Write(CommonTypes.Encode(t2));
				Output.Write("|");
				Output.Write(CommonTypes.Encode(dt));
				Output.WriteLine(";");

				if (this.Model.TimeCycleUnits != 1)
				{
					Output.Write("ct:=t/");
					Output.Write(s = CommonTypes.Encode(this.Model.TimeCycleUnits));
					Output.WriteLine(";");
					Output.Write("ct:=(ct-floor(ct))*");
					Output.Write(s);
					Output.WriteLine(";");
				}
				else
					Output.WriteLine("ct:=t-floor(t);");

				Output.Write("y:=");
				Output.Write(this.distributionId);
				Output.WriteLine("PDF(ct);");
				Output.WriteLine("G:=plot2dlinearea(t,y,Alpha(\"Blue\",64))+plot2dline(t,y,\"Blue\");");
				Output.Write("G.LabelX:=\"Time × ");
				Output.Write(this.Model.TimeUnitStr);
				Output.WriteLine("\";");
				Output.Write("G.LabelY:=\"Intensity (/ ");
				Output.Write(this.Model.TimeUnitStr);
				Output.WriteLine(")\";");
				Output.Write("G.Title:=\"Probability Density Function of ");
				Output.Write(this.distributionId);
				Output.WriteLine("\";");
				Output.WriteLine("G");
				Output.WriteLine("}");
				Output.WriteLine();
			}
		}

	}
}
