using TAG.Simulator.Statistics;

namespace TAG.Simulator.ObjectModel.Graphs.Plots
{
	/// <summary>
	/// Abstract base class for plots
	/// </summary>
	public abstract class Plot
	{
		private readonly Model model;

		/// <summary>
		/// Abstract base class for plots
		/// </summary>
		/// <param name="Model">Underlying simulation model.</param>
		public Plot(Model Model)
		{
			this.model = Model;
		}

		/// <summary>
		/// Gets the start time of a statistic.
		/// </summary>
		/// <param name="Record">Statistic.</param>
		/// <returns>Start time</returns>
		public double GetStartTime(Statistic Record)
		{
			return this.model.GetTimeCoordinates(Record.Start);
		}

		/// <summary>
		/// Gets the stop time of a statistic.
		/// </summary>
		/// <param name="Record">Statistic.</param>
		/// <returns>Stop time</returns>
		public double GetStopTime(Statistic Record)
		{
			return this.model.GetTimeCoordinates(Record.Stop);
		}

		/// <summary>
		/// Gets the mean time of a statistic.
		/// </summary>
		/// <param name="Record">Statistic.</param>
		/// <returns>Mean time</returns>
		public double GetMeanTime(Statistic Record)
		{
			return (this.model.GetTimeCoordinates(Record.Start) + this.model.GetTimeCoordinates(Record.Stop)) * 0.5;
		}

		/// <summary>
		/// If there is a plot to display.
		/// </summary>
		public abstract bool HasGraph
		{ 
			get; 
		}

		/// <summary>
		/// Adds a statistic to the plot.
		/// </summary>
		/// <param name="Record">Statistic</param>
		public abstract void Add(Statistic Record);

		/// <summary>
		/// Breaks the graph.
		/// </summary>
		public abstract void Break();

		/// <summary>
		/// Gets the plot script
		/// </summary>
		/// <param name="Model">Underlying simulation model.</param>
		/// <param name="ShowXAxis">If X-axis should be displayed.</param>
		/// <returns>Graph script.</returns>
		public abstract string GetPlotScript(Model Model, bool ShowXAxis);

	}
}
