using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using TAG.Simulator.ObjectModel.Graphs.Plots;
using Waher.Content;
using Waher.Script.Objects;
using Waher.Script.Units;

namespace TAG.Simulator.Statistics
{
	/// <summary>
	/// Statistical bucket
	/// </summary>
	public class Bucket : IPeriodBucket
	{
		private readonly LinkedList<Statistic> statistics = new LinkedList<Statistic>();
		private readonly LinkedList<double> samples;
		private readonly Model model;
		private readonly string id;
		private readonly string title;
		private readonly string labelY;
		private readonly bool calcStdDev;
		private IFilter filter = null;
		private Duration bucketTime;
		private DateTime start;
		private DateTime stop;
		private Unit unit = null;
		private long count = 0;
		private long totCount = 0;
		private long relativeCounter = 0;
		private double sum = 0;
		private double min = double.MaxValue;
		private double max = double.MinValue;
		private bool hasSamples = false;
		private bool sampleGraph = false;

		/// <summary>
		/// Statistical bucket
		/// </summary>
		/// <param name="Id">ID of bucket.</param>
		/// <param name="Title">Title of bucket</param>
		/// <param name="LabelY">Y-label of bucket.</param>
		/// <param name="Model">Simulation model.</param>
		/// <param name="CalcStdDev">If standard deviation is to be calculated.</param>
		/// <param name="StartTime">Starting time</param>
		/// <param name="BucketTime">Duration of one bucket, where statistics is collected.</param>
		public Bucket(string Id, string Title, string LabelY, Model Model, bool CalcStdDev, DateTime StartTime, Duration BucketTime)
		{
			this.id = Id;
			this.title = Title;
			this.labelY = LabelY;
			this.model = Model;
			this.samples = CalcStdDev ? new LinkedList<double>() : null;
			this.calcStdDev = CalcStdDev;
			this.bucketTime = BucketTime;
			this.start = StartTime;
			this.stop = this.start + BucketTime;
		}

		/// <summary>
		/// Bucket ID
		/// </summary>
		public string Id => this.id;

		/// <summary>
		/// Optional header
		/// </summary>
		public string Header => string.Empty;

		/// <summary>
		/// Time to accumulate values.
		/// </summary>
		public Duration BucketTime
		{
			get => this.bucketTime;
			set => this.bucketTime = value;
		}

		/// <summary>
		/// Counter
		/// </summary>
		public long Count
		{
			get
			{
				lock (this)
				{
					return this.count;
				}
			}
		}

		/// <summary>
		/// Total Counter
		/// </summary>
		public long TotalCount
		{
			get
			{
				lock (this)
				{
					return this.totCount;
				}
			}
		}

		/// <summary>
		/// Sum of samples.
		/// </summary>
		public double Sum
		{
			get
			{
				lock (this)
				{
					return this.sum;
				}
			}
		}

		/// <summary>
		/// Smallest sample
		/// </summary>
		public double Min
		{
			get
			{
				lock (this)
				{
					return this.min;
				}
			}
		}

		/// <summary>
		/// Largest sample
		/// </summary>
		public double Max
		{
			get
			{
				lock (this)
				{
					return this.max;
				}
			}
		}

		/// <summary>
		/// Mean (average) value of samples.
		/// </summary>
		public double Mean
		{
			get
			{
				lock (this)
				{
					return this.sum / this.count;
				}
			}
		}

		/// <summary>
		/// Increments counter.
		/// </summary>
		/// <returns>Start time of bucket that was incremented.</returns>
		public DateTime Inc()
		{
			DateTime Timestamp = DateTime.Now;

			lock (this)
			{
				return this.Sample(Timestamp, ++this.relativeCounter);
			}
		}

		/// <summary>
		/// Decrements counter.
		/// </summary>
		/// <returns>Start time of bucket that was incremented.</returns>
		public DateTime Dec()
		{
			DateTime Timestamp = DateTime.Now;

			lock (this)
			{
				return this.Sample(Timestamp, --this.relativeCounter);
			}
		}

		private void NextBucketLocked()
		{
			if (this.hasSamples)
			{
				double Mean = this.sum / this.count;

				if (this.calcStdDev)
				{
					double Variance = this.CalcVarianceLocked();
					double StdDev = Math.Sqrt(Variance);

					this.statistics.AddLast(new Statistic(this.start, this.stop, this.count, Mean, Variance, StdDev, this.min, this.max));

					this.samples.Clear();
				}
				else
					this.statistics.AddLast(new Statistic(this.start, this.stop, this.count, Mean, this.min, this.max));

				this.sampleGraph = true;
				this.hasSamples = false;
				this.sum = 0;
				this.min = double.MaxValue;
				this.max = double.MinValue;
			}
			else
				this.statistics.AddLast(new Statistic(this.start, this.stop, this.count));

			this.count = 0;
			this.start = this.stop;
			this.stop = this.start + this.bucketTime;
		}

		/// <summary>
		/// Adds a sample
		/// </summary>
		/// <param name="Timestamp">Timestamp of value.</param>
		/// <param name="Value">Sample value reported</param>
		/// <returns>Start time of bucket to which the value was reported.</returns>
		public DateTime Sample(DateTime Timestamp, PhysicalQuantity Value)
		{
			double v;

			if (this.unit is null)
			{
				this.unit = Value.Unit;
				v = Value.Magnitude;
			}
			else if (!Unit.TryConvert(Value.Magnitude, Value.Unit, this.unit, out v))
				throw new Exception("Incompatible units: " + Value.Unit.ToString() + " and " + this.unit.ToString());

			return this.Sample(Timestamp, v);
		}

		/// <summary>
		/// Adds a sample
		/// </summary>
		/// <param name="Timestamp">Timestamp of value.</param>
		/// <param name="Value">Sample value reported</param>
		/// <returns>Start time of bucket to which the value was reported.</returns>
		public DateTime Sample(DateTime Timestamp, double Value)
		{
			if (this.filter?.Filter(ref Timestamp, ref Value) ?? false)
				return this.start;

			lock (this)
			{
				while (Timestamp >= this.stop)
					this.NextBucketLocked();

				this.sum += Value;
				this.count++;
				this.totCount++;

				if (this.hasSamples)
				{
					if (Value < this.min)
						this.min = Value;
					else if (Value > this.max)
						this.max = Value;
				}
				else
				{
					if (Value < this.min)
						this.min = Value;

					if (Value > this.max)
						this.max = Value;

					this.hasSamples = true;
				}

				this.samples?.AddLast(Value);

				return this.start;
			}
		}

		/// <summary>
		/// Counts one occurrence
		/// </summary>
		/// <param name="Timestamp">Timestamp of occurrence.</param>
		/// <returns>Start time of bucket to which the value was reported.</returns>
		public DateTime CountOccurrence(DateTime Timestamp)
		{
			lock (this)
			{
				while (Timestamp >= this.stop)
					this.NextBucketLocked();

				this.count++;
				this.totCount++;

				return this.start;
			}
		}

		/// <summary>
		/// Variance of samples
		/// </summary>
		public double Variance
		{
			get
			{
				if (this.samples is null)
					throw new InvalidOperationException("Bucket not prepared for calculating variances or standard deviations.");

				lock (this)
				{
					return this.CalcVarianceLocked();
				}
			}
		}

		private double CalcVarianceLocked()
		{
			double μ = this.sum / this.count;
			double S = 0;
			double x;

			foreach (double d in this.samples)
			{
				x = (d - μ);
				S += x * x;
			}

			return S / this.count;
		}

		/// <summary>
		/// (Biased) standard deviation of samples
		/// </summary>
		public double StdDev => Math.Sqrt(this.Variance);

		/// <summary>
		/// Terminates the ongoing collection of data.
		/// </summary>
		public void Flush()
		{
			lock (this)
			{
				if (this.count > 0)
					this.NextBucketLocked();
			}
		}

		/// <summary>
		/// Gets an enumerator for recorded statistics.
		/// </summary>
		/// <returns>Enumerator</returns>
		public IEnumerator<Statistic> GetEnumerator()
		{
			return this.statistics.GetEnumerator();
		}

		/// <summary>
		/// Gets an enumerator for recorded statistics.
		/// </summary>
		/// <returns>Enumerator</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.statistics.GetEnumerator();
		}


		/// <summary>
		/// Exports the graph to a markdown output.
		/// </summary>
		/// <param name="Output">Markdown output</param>
		public void ExportGraph(StreamWriter Output)
		{
			Output.WriteLine("{");
			Output.WriteLine("GraphWidth:=1000;");
			Output.WriteLine("GraphHeight:=400;");

			this.ExportGraphScript(Output, null, true);

			Output.WriteLine("}");
			Output.WriteLine();
		}

		/// <summary>
		/// Exports the graph to a markdown output.
		/// </summary>
		/// <param name="Output">Markdown output</param>
		/// <param name="CustomColor">Optional custom color</param>
		/// <param name="Span">If the entire span can be included.</param>
		/// <returns>If script was exported.</returns>
		public bool ExportGraphScript(StreamWriter Output, string CustomColor, bool Span)
		{
			this.Flush();

			string LabelY = this.labelY;

			if (!(this.unit is null))
				LabelY += " (" + this.unit.ToString() + ")";

			lock (this)
			{
				Plot Graph;

				if (Span)
				{
					if (this.sampleGraph)
						Graph = new PlotLineSpan(this.model, string.IsNullOrEmpty(CustomColor) ? "Red" : CustomColor, string.IsNullOrEmpty(CustomColor) ? "Blue" : CustomColor);
					else
						Graph = new PlotLineArea(this.model, string.IsNullOrEmpty(CustomColor) ? "Red" : CustomColor);
				}
				else
					Graph = new PlotLine(this.model, string.IsNullOrEmpty(CustomColor) ? "Red" : CustomColor);

				foreach (Statistic Rec in this.statistics)
					Graph.Add(Rec);

				if (!Graph.HasGraph)
					return false;

				Output.Write("G:=(");
				Output.Write(Graph.GetPlotScript());
				Output.WriteLine(");");

				Output.Write("G.LabelX:=\"Time × ");
				Output.Write(this.model.TimeUnitStr);
				Output.WriteLine("\";");
				Output.Write("G.LabelY:=\"");
				Output.Write(LabelY.Replace("%BT%", Model.DurationToString(this.bucketTime)));
				Output.WriteLine("\";");
				Output.Write("G.Title:=\"");
				Output.Write(this.title.Replace("\"", "\\\""));
				Output.WriteLine("\";");
				Output.WriteLine("G");

				return true;
			}
		}

		/// <summary>
		/// Exports data to XML
		/// </summary>
		/// <param name="Output">XML Output</param>
		/// <param name="RowElement">XML Row element name.</param>
		public void ExportXml(XmlWriter Output, string RowElement)
		{
			this.Flush();

			lock (this)
			{
				Output.WriteStartElement(RowElement);
				Output.WriteAttributeString("type", this.id);
				Output.WriteAttributeString("count", this.totCount.ToString());

				if (!(this.unit is null))
					Output.WriteAttributeString("unit", this.unit.ToString());

				foreach (Statistic Rec in this.statistics)
					Rec.ExportXml(Output);

				Output.WriteEndElement();
			}
		}

		/// <summary>
		/// Adds a filter to the bucket.
		/// </summary>
		/// <param name="Filter">Filter</param>
		public void Add(IFilter Filter)
		{
			if (this.filter is null)
				this.filter = Filter;
			else
				this.filter.Append(Filter);
		}

	}
}
