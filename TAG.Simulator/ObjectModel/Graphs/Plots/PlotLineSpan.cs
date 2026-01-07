using TAG.Simulator.Statistics;

namespace TAG.Simulator.ObjectModel.Graphs.Plots
{
	/// <summary>
	/// Plots a line graph with min-max span
	/// </summary>
	public class PlotLineSpan : PlotLine
	{
		private readonly string spanColor;
		private Series min = null;
		private Series max = null;

		/// <summary>
		/// Plots a line graph with min-max span
		/// </summary>
		/// <param name="Model">Underlying simulation model.</param>
		public PlotLineSpan(Model Model)
			: base(Model)
		{
			this.spanColor = "Blue";
		}

		/// <summary>
		/// Plots a line graph with min-max span
		/// </summary>
		/// <param name="Model">Underlying simulation model.</param>
		/// <param name="Color">Color of graph.</param>
		public PlotLineSpan(Model Model, string Color)
			: this(Model, Color, Color)
		{
		}

		/// <summary>
		/// Plots a line graph with min-max span
		/// </summary>
		/// <param name="Model">Underlying simulation model.</param>
		/// <param name="Color">Color of graph.</param>
		/// <param name="SpanColor">Color of spanned area.</param>
		public PlotLineSpan(Model Model, string Color, string SpanColor)
			: base(Model, Color)
		{
			this.spanColor = SpanColor;
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

				if (Record.Min.HasValue && Record.Max.HasValue)
				{
					if (this.min is null)
					{
						this.min = new Series("Min" + this.Index.ToString());
						this.max = new Series("Max" + this.Index.ToString());
					}

					this.min.Add(Record.Min.Value);
					this.min.Add(Record.Min.Value);
					
					this.max.Add(Record.Max.Value);
					this.max.Add(Record.Max.Value);
				}
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
		/// Breaks the graph.
		/// </summary>
		public override void Break()
		{
			if (!(this.min is null))
			{
				this.graph.AppendLine(this.min.EndSeries());
				this.graph.AppendLine(this.max.EndSeries());

				this.min = null;
				this.max = null;
			}

			base.Break();
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

				this.graph.Append("polygon2d(join(x");
				this.graph.Append(i.ToString());
				this.graph.Append(",Reverse(x");
				this.graph.Append(i.ToString());
				this.graph.Append(")),join(Min");
				this.graph.Append(i.ToString());
				this.graph.Append(",Reverse(Max");
				this.graph.Append(i.ToString());
				this.graph.Append(")),alpha(\"");
				this.graph.Append(this.spanColor);
				this.graph.Append("\",16))");
			}

			for (i = 1; i < this.Index; i++)
			{
				this.graph.Append("+plot2dline(x");
				this.graph.Append(i.ToString());
				this.graph.Append(",Min");
				this.graph.Append(i.ToString());
				this.graph.Append(",alpha(\"");
				this.graph.Append(this.spanColor);
				this.graph.Append("\",32))");
				this.graph.Append("+plot2dline(x");
				this.graph.Append(i.ToString());
				this.graph.Append(",Max");
				this.graph.Append(i.ToString());
				this.graph.Append(",alpha(\"");
				this.graph.Append(this.spanColor);
				this.graph.Append("\",32))");
			}

			if (this.Index > 1)
				this.graph.Append('+');

			return base.GetPlotScript();
		}

	}
}
