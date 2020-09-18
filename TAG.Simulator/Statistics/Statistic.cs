using System;
using System.Xml;
using Waher.Content;
using Waher.Content.Xml;

namespace TAG.Simulator.Statistics
{
	/// <summary>
	/// Represents collected statistical information from a small portion of time.
	/// </summary>
	public class Statistic
	{
		private readonly DateTime start;
		private readonly DateTime stop;
		private readonly long count;
		private readonly double? mean;
		private readonly double? variance;
		private readonly double? stdDev;
		private readonly double? min;
		private readonly double? max;

		/// <summary>
		/// Represents collected statistical information from a small portion of time.
		/// </summary>
		/// <param name="Start">Start of period.</param>
		/// <param name="Stop">End of period.</param>
		/// <param name="Count">Number of events</param>
		/// <param name="Mean">Mean value</param>
		/// <param name="Variance">Variance of values</param>
		/// <param name="StdDev">Standard deviation of values</param>
		/// <param name="Min">Smallest value</param>
		/// <param name="Max">Largest value</param>
		public Statistic(DateTime Start, DateTime Stop, long Count, double Mean, double Variance, double StdDev, double Min, double Max)
		{
			this.start = Start;
			this.stop = Stop;
			this.count = Count;
			this.mean = Mean;
			this.variance = Variance;
			this.stdDev = StdDev;
			this.min = Min;
			this.max = Max;
		}

		/// <summary>
		/// Represents collected statistical information from a small portion of time.
		/// </summary>
		/// <param name="Start">Start of period.</param>
		/// <param name="Stop">End of period.</param>
		/// <param name="Count">Number of events</param>
		/// <param name="Mean">Mean value</param>
		/// <param name="Min">Smallest value</param>
		/// <param name="Max">Largest value</param>
		public Statistic(DateTime Start, DateTime Stop, long Count, double Mean, double Min, double Max)
		{
			this.start = Start;
			this.stop = Stop;
			this.count = Count;
			this.mean = Mean;
			this.variance = null;
			this.stdDev = null;
			this.min = Min;
			this.max = Max;
		}

		/// <summary>
		/// Represents collected statistical information from a small portion of time.
		/// </summary>
		/// <param name="Start">Start of period.</param>
		/// <param name="Stop">End of period.</param>
		/// <param name="Count">Number of events</param>
		public Statistic(DateTime Start, DateTime Stop, long Count)
		{
			this.start = Start;
			this.stop = Stop;
			this.count = Count;
			this.mean = null;
			this.variance = null;
			this.stdDev = null;
			this.min = null;
			this.max = null;
		}

		/// <summary>
		/// Start of period.
		/// </summary>
		public DateTime Start => this.start;

		/// <summary>
		/// End of period.
		/// </summary>
		public DateTime Stop => this.stop;

		/// <summary>
		/// Number of events
		/// </summary>
		public long Count => this.count;

		/// <summary>
		/// Mean value
		/// </summary>
		public double? Mean => this.mean;

		/// <summary>
		/// Variance of values
		/// </summary>
		public double? Variance => this.variance;

		/// <summary>
		/// Standard deviation of values
		/// </summary>
		public double? StdDev => this.stdDev;

		/// <summary>
		/// Smallest value
		/// </summary>
		public double? Min => this.min;

		/// <summary>
		/// Largest value
		/// </summary>
		public double? Max => this.max;

		/// <summary>
		/// Exports data to XML
		/// </summary>
		/// <param name="Output">XML Output</param>
		public void ExportXml(XmlWriter Output)
		{
			Output.WriteStartElement("Stat");
			Output.WriteAttributeString("start", XML.Encode(this.start));
			Output.WriteAttributeString("stop", XML.Encode(this.stop));
			Output.WriteAttributeString("count", this.count.ToString());

			if (this.mean.HasValue)
				Output.WriteAttributeString("mean", CommonTypes.Encode(this.mean.Value));

			if (this.min.HasValue)
				Output.WriteAttributeString("min", CommonTypes.Encode(this.min.Value));

			if (this.max.HasValue)
				Output.WriteAttributeString("max", CommonTypes.Encode(this.max.Value));

			if (this.variance.HasValue)
				Output.WriteAttributeString("variance", CommonTypes.Encode(this.variance.Value));

			if (this.stdDev.HasValue)
				Output.WriteAttributeString("stdDev", CommonTypes.Encode(this.stdDev.Value));

			Output.WriteEndElement();
		}
	}
}
