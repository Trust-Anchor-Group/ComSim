using System;
using System.Collections.Generic;
using System.Text;
using Waher.Content;

namespace TAG.Simulator.ObjectModel.Graphs.Plots
{
	/// <summary>
	/// Represents a data series.
	/// </summary>
	public class Series
	{
		private readonly StringBuilder values = new StringBuilder();
		private bool first = true;

		/// <summary>
		/// Represents a data series.
		/// </summary>
		/// <param name="Name">Name of series</param>
		public Series(string Name)
		{
			this.values.Append(Name);
			this.values.Append(":=[");
		}

		/// <summary>
		/// Adds a value to the series.
		/// </summary>
		/// <param name="Value">Value</param>
		public void Add(double Value)
		{
			if (this.first)
				this.first = false;
			else
				this.values.Append(',');

			this.values.Append(CommonTypes.Encode(Value));
		}

		/// <summary>
		/// Ends the series.
		/// </summary>
		/// <returns>Script representation of series.</returns>
		public string EndSeries()
		{
			this.values.Append("];");
			return this.values.ToString();
		}
	}
}
