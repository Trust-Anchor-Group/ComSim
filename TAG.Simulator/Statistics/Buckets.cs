using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SkiaSharp;
using Waher.Content;

namespace TAG.Simulator.Statistics
{
	/// <summary>
	/// A collection of buckets
	/// </summary>
	public class Buckets
	{
		private readonly SortedDictionary<string, Bucket> buckets = new SortedDictionary<string, Bucket>();
		private readonly Duration bucketTime;
		private DateTime start;

		/// <summary>
		/// A collection of buckets
		/// </summary>
		/// <param name="StartTime">Starting time</param>
		/// <param name="BucketTime">Duration of one bucket, where statistics is collected.</param>
		public Buckets(DateTime StartTime, Duration BucketTime)
			: this(StartTime, BucketTime, false, false, null)
		{
		}

		/// <summary>
		/// A collection of buckets
		/// </summary>
		/// <param name="StartTime">Starting time</param>
		/// <param name="BucketTime">Duration of one bucket, where statistics is collected.</param>
		/// <param name="CalcStdDev">If standard deviation is to be calculated.</param>
		/// <param name="PersistentCounters">If counters are persistent across bucket boundaries.</param>
		/// <param name="IDs">Predefined IDs</param>
		public Buckets(DateTime StartTime, Duration BucketTime, bool CalcStdDev, bool PersistentCounters, string[] IDs)
		{
			this.start = StartTime;
			this.bucketTime = BucketTime;

			if (!(IDs is null))
			{
				foreach (string ID in IDs)
					this.buckets[ID] = new Bucket(ID, CalcStdDev, PersistentCounters, StartTime, BucketTime);
			}
		}

		/// <summary>
		/// A collection of buckets
		/// </summary>
		/// <param name="StartTime">Starting time</param>
		/// <param name="BucketTime">Duration of one bucket, where statistics is collected.</param>
		/// <param name="Buckets">Predefined buckets.</param>
		public Buckets(DateTime StartTime, Duration BucketTime, params Bucket[] Buckets)
		{
			this.start = StartTime;
			this.bucketTime = BucketTime;

			if (!(Buckets is null))
			{
				foreach (Bucket Bucket in Buckets)
					this.buckets[Bucket.Id] = Bucket;
			}
		}

		/// <summary>
		/// Counts an event.
		/// </summary>
		/// <param name="Counter">Counter ID</param>
		public void CountEvent(string Counter)
		{
			Bucket Bucket;

			lock (this.buckets)
			{
				if (!this.buckets.TryGetValue(Counter, out Bucket))
				{
					Bucket = new Bucket(Counter, false, false, this.start, this.bucketTime);
					this.buckets[Counter] = Bucket;
				}
			}

			this.start = Bucket.Inc();
		}

		/// <summary>
		/// Increments a counter.
		/// </summary>
		/// <param name="Counter">Counter ID</param>
		public void IncrementCounter(string Counter)
		{
			Bucket Bucket;

			lock (this.buckets)
			{
				if (!this.buckets.TryGetValue(Counter, out Bucket))
				{
					Bucket = new Bucket(Counter, false, true, this.start, this.bucketTime);
					this.buckets[Counter] = Bucket;
				}
			}

			this.start = Bucket.Inc();
		}

		/// <summary>
		/// Decrements a counter.
		/// </summary>
		/// <param name="Counter">Counter ID</param>
		public void DecrementCounter(string Counter)
		{
			Bucket Bucket;

			lock (this.buckets)
			{
				if (!this.buckets.TryGetValue(Counter, out Bucket))
				{
					Bucket = new Bucket(Counter, false, true, this.start, this.bucketTime);
					this.buckets[Counter] = Bucket;
				}
			}

			this.start = Bucket.Dec();
		}

		/// <summary>
		/// Samples a value
		/// </summary>
		/// <param name="Counter">Counter ID</param>
		/// <param name="Value">Value</param>
		public void Sample(string Counter, double Value)
		{
			Bucket Bucket;

			lock (this.buckets)
			{
				if (!this.buckets.TryGetValue(Counter, out Bucket))
				{
					Bucket = new Bucket(Counter, true, false, this.start, this.bucketTime);
					this.buckets[Counter] = Bucket;
				}
			}

			this.start = Bucket.Sample(Value);
		}

		/// <summary>
		/// Number of counters
		/// </summary>
		public int Count
		{
			get
			{
				lock (this.buckets)
				{
					return this.buckets.Count;
				}
			}
		}

		/// <summary>
		/// Gets a count table of registered counters.
		/// </summary>
		/// <param name="Order">Optional sort order of records.</param>
		/// <returns>Count table</returns>
		public CountTable GetTotalCountTable(string[] Order = null)
		{
			CountTable Result = new CountTable();

			lock (this.buckets)
			{
				if (Order is null)
				{
					foreach (Bucket Bucket in this.buckets.Values)
						Result.Add(Bucket.Id, Bucket.TotalCount);
				}
				else
				{
					foreach (string Id in Order)
					{
						if (this.buckets.TryGetValue(Id, out Bucket Bucket))
							Result.Add(Bucket.Id, Bucket.TotalCount);
					}
				}
			}

			return Result;
		}

		/// <summary>
		/// Tries to get a bucket, given its ID.
		/// </summary>
		/// <param name="Id">Bucket ID</param>
		/// <param name="Bucket">Bucket, if found.</param>
		/// <returns>If a bucket was found.</returns>
		public bool TryGetBucket(string Id, out Bucket Bucket)
		{
			lock (this.buckets)
			{
				return this.buckets.TryGetValue(Id, out Bucket);
			}
		}

		/// <summary>
		/// Exports historical counts as a graph.
		/// </summary>
		/// <param name="Title">Title of graph.</param>
		/// <param name="Order">Preferred order, can be null.</param>
		/// <param name="Output">Export destination</param>
		/// <param name="Model">Simulation model</param>
		/// <param name="Palette">Optional predefined palette</param>
		public void ExportCountHistoryGraph(string Title, IEnumerable<string> Order, StreamWriter Output, Model Model, SKColor[] Palette = null)
		{
			lock (this.buckets)
			{
				SortedDictionary<DateTime, long> Counts = new SortedDictionary<DateTime, long>();
				int Count = 0;

				if (Order is null)
					Order = this.buckets.Keys;

				foreach (string ActivityId in Order)
				{
					if (this.TryGetBucket(ActivityId, out Bucket Bucket))
					{
						Bucket.Flush();

						Statistic Last = null;

						foreach (Statistic Rec in Bucket)
						{
							if (!Counts.ContainsKey(Rec.Start))
								Counts[Rec.Start] = 0;

							Last = Rec;
						}

						if (!(Last is null))
						{
							if (!Counts.ContainsKey(Last.Stop))
								Counts[Last.Stop] = 0;
						}

						Count++;
					}
				}

				StringBuilder Script = new StringBuilder();
				StringBuilder PlotScript = null;
				SKColor Color;
				string t;
				int i = 0;
				int j = 0;
				int c = Counts.Count;
				bool First = true;

				if (Palette is null)
					Palette = Model.CreatePalette(Count);

				Script.Append("Time:=[");

				foreach (DateTime TP in Counts.Keys)
				{
					if (First)
						First = false;
					else
						Script.Append(',');

					Script.Append(t = CommonTypes.Encode(Model.GetTimeCoordinates(TP)));

					if (j > 0 && j < c - 1)
					{
						Script.Append(',');
						Script.Append(t);
					}

					j++;
				}

				Script.AppendLine("];");

				foreach (string ActivityId in Order)
				{
					if (this.TryGetBucket(ActivityId, out Bucket Bucket))
					{
						foreach (Statistic Rec in Bucket)
						{
							if (!Counts.TryGetValue(Rec.Stop, out long Value))
								Value = 0;

							Value += Rec.Count;
							Counts[Rec.Stop] = Value;
						}

						First = true;
						Color = Palette[i++];

						Script.Append(ActivityId);
						Script.Append("Count:=[");

						bool Skip = true;

						foreach (long Value in Counts.Values)
						{
							if (Skip)
							{
								Skip = false;
								continue;
							}

							if (First)
								First = false;
							else
								Script.Append(',');

							Script.Append(t = Value.ToString());
							Script.Append(',');
							Script.Append(t);
						}

						Script.AppendLine("];");

						if (PlotScript is null)
							PlotScript = new StringBuilder();
						else
							PlotScript.Append('+');

						PlotScript.Append("plot2dlinearea(Time,");
						PlotScript.Append(ActivityId);
						PlotScript.Append("Count,rgb(");
						PlotScript.Append(Color.Red.ToString());
						PlotScript.Append(',');
						PlotScript.Append(Color.Green.ToString());
						PlotScript.Append(',');
						PlotScript.Append(Color.Blue.ToString());
						PlotScript.Append("))");
					}
				}

				Script.AppendLine("GraphWidth:=1000;");
				Script.AppendLine("GraphHeight:=400;");
				Script.Append("G:=");
				Script.Append(PlotScript.ToString());
				Script.AppendLine(";");
				Script.Append("G.LabelX:=\"Time × ");
				Script.Append(Model.TimeUnitStr);
				Script.AppendLine("\";");
				Script.Append("G.LabelY:=\"Count / ");
				Script.Append(Model.BucketTimeStr);
				Script.AppendLine("\";");
				Script.Append("G.Title:=\"");
				Script.Append(Title);
				Script.AppendLine("\";");
				Script.Append("G");

				Output.WriteLine("{");
				Output.WriteLine(Script.ToString());
				Output.WriteLine("}");
				Output.WriteLine();
			}
		}

		/// <summary>
		/// Sample IDs
		/// </summary>
		public string[] IDs
		{
			get
			{
				lock (this.buckets)
				{
					string[] Result = new string[this.buckets.Count];
					this.buckets.Keys.CopyTo(Result, 0);
					return Result;
				}
			}
		}

	}
}
