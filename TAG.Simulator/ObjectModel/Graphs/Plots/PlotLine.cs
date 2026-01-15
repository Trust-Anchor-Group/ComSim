using System.Text;
using TAG.Simulator.Statistics;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Graphs.Plots
{
	/// <summary>
	/// Plots a line graph
	/// </summary>
	public class PlotLine : Plot
	{
		private readonly string color;
		private Series x = null;
		private Series y = null;
		private int index = 1;
		private bool first = true;

		/// <summary>
		/// Graph being built.
		/// </summary>
		protected readonly StringBuilder graph = new StringBuilder();

		/// <summary>
		/// Plots a line graph
		/// </summary>
		/// <param name="Model">Underlying simulation model.</param>
		public PlotLine(Model Model)
			: this(Model, "Red")
		{
		}

		/// <summary>
		/// Plots a line graph
		/// </summary>
		/// <param name="Model">Underlying simulation model.</param>
		/// <param name="Color">Color of graph.</param>
		public PlotLine(Model Model, string Color)
			: base(Model)
		{
			this.color = Color;
		}

		/// <summary>
		/// Current section index.
		/// </summary>
		public int Index => this.index;

		/// <summary>
		/// Graph color.
		/// </summary>
		public string Color => this.color;

		/// <summary>
		/// If there is a plot to display.
		/// </summary>
		public override bool HasGraph
		{
			get
			{
				this.Break();
				return this.index > 1;
			}
		}

		/// <summary>
		/// Adds a statistic to the plot.
		/// </summary>
		/// <param name="Record">Statistic</param>
		public override void Add(Statistic Record)
		{
			if (Record.Mean.HasValue)
				this.AddPoint(this.GetMeanTime(Record), Record.Mean.Value);
			else if (Record.Count > 0)
				this.AddPoint(this.GetMeanTime(Record), Record.Count);
			else
				this.Break();
		}

		/// <summary>
		/// Adds a point to the graph.
		/// </summary>
		/// <param name="X">X-coordinate</param>
		/// <param name="Y">Y-coordinate</param>
		public void AddPoint(double X, double Y)
		{
			if (this.first)
			{
				this.first = false;

				this.x = new Series("x" + this.index.ToString());
				this.y = new Series("y" + this.index.ToString());
			}

			this.x.Add(X);
			this.y.Add(Y);
		}

		/// <summary>
		/// Breaks the graph.
		/// </summary>
		public override void Break()
		{
			if (!this.first)
			{
				this.graph.AppendLine(this.x.EndSeries());
				this.graph.AppendLine(this.y.EndSeries());

				this.index++;
				this.first = true;
				this.x = null;
				this.y = null;
			}
		}

		/// <summary>
		/// Gets the plot script
		/// </summary>
		/// <param name="Model">Underlying simulation model.</param>
		/// <param name="ShowXAxis">If X-axis should be displayed.</param>
		/// <returns>Graph script.</returns>
		public override string GetPlotScript(Model Model, bool ShowXAxis)
		{
			int i;

			this.Break();

			if (ShowXAxis)
			{
				this.graph.Append("plot2dline([");
				this.graph.Append(Expression.ToString(Model.GetTimeCoordinates(Model.StartTime)));
				this.graph.Append(',');
				this.graph.Append(Expression.ToString(Model.GetTimeCoordinates(Model.EndTime)));
				this.graph.Append("],[0,0],\"Transparent\",1)");
			}

			for (i = 1; i < this.index; i++)
			{
				if (i > 1 || ShowXAxis)
					this.graph.AppendLine("+");

				this.graph.Append("plot2dline(x");
				this.graph.Append(i.ToString());
				this.graph.Append(",y");
				this.graph.Append(i.ToString());
				this.graph.Append(",\"");
				this.graph.Append(this.color);
				this.graph.Append("\",5)");
			}

			return this.graph.ToString();
		}

	}
}
