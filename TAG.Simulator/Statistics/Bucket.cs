using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Waher.Content;

namespace TAG.Simulator.Statistics
{
	/// <summary>
	/// Statistical bucket
	/// </summary>
	public class Bucket : IEnumerable<Statistic>
	{
		private readonly LinkedList<Statistic> statistics = new LinkedList<Statistic>();
		private readonly LinkedList<double> samples;
		private readonly string id;
		private readonly Duration bucketTime;
		private readonly bool calcStdDev;
		private DateTime start;
		private DateTime stop;
		private long count = 0;
		private long totCount = 0;
		private double sum = 0;
		private double min = double.MaxValue;
		private double max = double.MinValue;
		private bool hasSamples = false;

		/// <summary>
		/// Statistical bucket
		/// </summary>
		/// <param name="Id">ID of bucket.</param>
		/// <param name="CalcStdDev">If standard deviation is to be calculated.</param>
		/// <param name="StartTime">Starting time</param>
		/// <param name="BucketTime">Duration of one bucket, where statistics is collected.</param>
		public Bucket(string Id, bool CalcStdDev, DateTime StartTime, Duration BucketTime)
		{
			this.id = Id;
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
			lock (this)
			{
				return this.Sample(++this.totCount);
			}
		}

		/// <summary>
		/// Decrements counter.
		/// </summary>
		/// <returns>Start time of bucket that was incremented.</returns>
		public DateTime Dec()
		{
			lock (this)
			{
				return this.Sample(--this.totCount);
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
		/// <param name="Value">Sample value reported</param>
		/// <returns>Start time of bucket to which the value was reported.</returns>
		public DateTime Sample(double Value)
		{
			lock (this)
			{
				DateTime Now = DateTime.Now;
				while (Now >= this.stop)
					this.NextBucketLocked();

				this.sum += Value;
				this.count++;

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
		/// <returns>Start time of bucket to which the value was reported.</returns>
		public DateTime CountOccurrence()
		{
			lock (this)
			{
				DateTime Now = DateTime.Now;
				while (Now >= this.stop)
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
		/// Exports historical counts as a graph.
		/// </summary>
		/// <param name="Title">Title of graph.</param>
		/// <param name="LabelY">Label for Y-axis.</param>
		/// <param name="Output">Export destination</param>
		/// <param name="Model">Simulation model</param>
		public void ExportSampleHistoryGraph(string Title, string LabelY, StreamWriter Output, Model Model)
		{
			this.Flush();

			lock (this)
			{
				StringBuilder TimeScript = new StringBuilder("Time:=[");
				StringBuilder MinScript = new StringBuilder("Min:=[");
				StringBuilder MeanScript = new StringBuilder("Mean:=[");
				StringBuilder MaxScript = new StringBuilder("Max:=[");
				bool First = true;

				foreach (Statistic Rec in this.statistics)
				{
					if (First)
						First = false;
					else
					{
						TimeScript.Append(',');
						MinScript.Append(',');
						MeanScript.Append(',');
						MaxScript.Append(',');
					}

					TimeScript.Append(CommonTypes.Encode(Model.GetTimeCoordinates(Rec.Start)));
					TimeScript.Append(',');
					TimeScript.Append(CommonTypes.Encode(Model.GetTimeCoordinates(Rec.Stop)));

					if (Rec.Min.HasValue)
					{
						MinScript.Append(CommonTypes.Encode(Rec.Min.Value));
						MinScript.Append(',');
						MinScript.Append(CommonTypes.Encode(Rec.Min.Value));
					}
					else
						MinScript.Append("null,null");

					if (Rec.Mean.HasValue)
					{
						MeanScript.Append(CommonTypes.Encode(Rec.Mean.Value));
						MeanScript.Append(',');
						MeanScript.Append(CommonTypes.Encode(Rec.Mean.Value));
					}
					else
						MeanScript.Append("null,null");

					if (Rec.Max.HasValue)
					{
						MaxScript.Append(CommonTypes.Encode(Rec.Max.Value));
						MaxScript.Append(',');
						MaxScript.Append(CommonTypes.Encode(Rec.Max.Value));
					}
					else
						MaxScript.Append("null,null");
				}

				TimeScript.Append("];");
				MinScript.Append("];");
				MeanScript.Append("];");
				MaxScript.Append("];");

				Output.WriteLine("{");
				Output.WriteLine(TimeScript.ToString());
				Output.WriteLine(MinScript.ToString());
				Output.WriteLine(MeanScript.ToString());
				Output.WriteLine(MaxScript.ToString());

				Output.WriteLine("GraphWidth:=1000;");
				Output.WriteLine("GraphHeight:=400;");
				Output.WriteLine("G:=polygon2d(join(Time,Reverse(Time)),join(Min,Reverse(Max)),alpha(\"Blue\",32))+plot2dline(Time,Min,alpha(\"Blue\",128))+plot2dline(Time,Max,alpha(\"Blue\",128))+plot2dline(Time,Mean,\"Red\",5);");
				Output.Write("G.LabelX:=\"Time × ");
				Output.Write(Model.TimeUnitStr);
				Output.WriteLine("\";");
				Output.Write("G.LabelY:=\"");
				Output.Write(LabelY);
				Output.WriteLine("\";");
				Output.Write("G.Title:=\"");
				Output.Write(Title);
				Output.WriteLine("\";");
				Output.WriteLine("G");
				Output.WriteLine("}");
				Output.WriteLine();
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

				foreach (Statistic Rec in this.statistics)
					Rec.ExportXml(Output);

				Output.WriteEndElement();
			}
		}

	}
}
