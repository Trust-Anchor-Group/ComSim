using TAG.Simulator.Statistics;

namespace TAG.Simulator.ObjectModel.Graphs.Plots
{
	/// <summary>
	/// Plots a line area graph
	/// </summary>
	public class PlotLineArea : PlotLine
	{
		/// <summary>
		/// Plots a line area graph
		/// </summary>
		/// <param name="Model">Underlying simulation model.</param>
		public PlotLineArea(Model Model)
			: base(Model)
		{
		}

		/// <summary>
		/// Plots a line area graph
		/// </summary>
		/// <param name="Model">Underlying simulation model.</param>
		/// <param name="Color">Color of graph.</param>
		public PlotLineArea(Model Model, string Color)
			: base(Model, Color)
		{
		}

		/// <summary>
		/// Adds a statistic to the plot.
		/// </summary>
		/// <param name="Record">Statistic</param>
		public override void Add(Statistic Record)
		{
			if (Record.Mean.HasValue)
			{
				this.AddPoint(this.GetStartTime(Record), Record.Mean.Value);
				this.AddPoint(this.GetStopTime(Record), Record.Mean.Value);
			}
			else if (Record.Count > 0)
			{
				this.AddPoint(this.GetStartTime(Record), Record.Count);
				this.AddPoint(this.GetStopTime(Record), Record.Count);
			}
			else
				this.Break();
		}

		/// <summary>
		/// Gets the plot script
		/// </summary>
		/// <returns>Graph script.</returns>
		public override string GetPlotScript()
		{
			this.Break();

			int i;

			for (i = 1; i < this.Index; i++)
			{
				if (i > 1)
					this.graph.Append('+');

				this.graph.Append("plot2dlinearea(x");
				this.graph.Append(i.ToString());
				this.graph.Append(",y");
				this.graph.Append(i.ToString());
				this.graph.Append(",alpha(\"");
				this.graph.Append(this.Color);
				this.graph.AppendLine("\",16))");
			}

			if (this.Index > 1)
				this.graph.Append('+');

			return base.GetPlotScript();
		}
	}
}
