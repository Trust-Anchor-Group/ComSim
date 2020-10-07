using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using SkiaSharp;
using TAG.Simulator.ObjectModel.Distributions;
using TAG.Simulator.ObjectModel.Events;
using TAG.Simulator.ObjectModel.Graphs;
using Waher.Content;
using Waher.Script.Objects;

namespace TAG.Simulator.Statistics
{
	/// <summary>
	/// A collection of buckets
	/// </summary>
	public class Buckets
	{
		private readonly SortedDictionary<string, IBucket> buckets = new SortedDictionary<string, IBucket>();
		private readonly Duration bucketTime;
		private readonly string title;
		private readonly string labelY;
		private readonly Model model;
		private DateTime start;

		/// <summary>
		/// A collection of buckets
		/// </summary>
		/// <param name="StartTime">Starting time</param>
		/// <param name="BucketTime">Duration of one bucket, where statistics is collected.</param>
		/// <param name="Title">Title of buckets. %ID% will be replaced by the ID of each bucket.</param>
		/// <param name="LabelY">Y-label of buckets. %ID% will be replaced by the ID of each bucket.</param>
		/// <param name="Model">Simulation model.</param>
		public Buckets(DateTime StartTime, Duration BucketTime, string Title, string LabelY, Model Model)
			: this(StartTime, BucketTime, Title, LabelY, Model, false, null)
		{
		}

		/// <summary>
		/// A collection of buckets
		/// </summary>
		/// <param name="StartTime">Starting time</param>
		/// <param name="BucketTime">Duration of one bucket, where statistics is collected.</param>
		/// <param name="Title">Title of buckets. %ID% will be replaced by the ID of each bucket.</param>
		/// <param name="LabelY">Y-label of buckets. %ID% will be replaced by the ID of each bucket.</param>
		/// <param name="Model">Simulation model.</param>
		/// <param name="CalcStdDev">If standard deviation is to be calculated.</param>
		/// <param name="IDs">Predefined IDs</param>
		public Buckets(DateTime StartTime, Duration BucketTime, string Title, string LabelY, Model Model, bool CalcStdDev, string[] IDs)
		{
			this.start = StartTime;
			this.bucketTime = BucketTime;
			this.title = Title;
			this.labelY = LabelY;
			this.model = Model;

			if (!(IDs is null))
			{
				foreach (string ID in IDs)
					this.buckets[ID] = new Bucket(ID, this.GetTitle(ID), this.labelY, this.model, CalcStdDev, StartTime, BucketTime);
			}
		}

		private string GetTitle(string ID)
		{
			return this.title.Replace("%ID%", ID);
		}

		/// <summary>
		/// A collection of buckets
		/// </summary>
		/// <param name="StartTime">Starting time</param>
		/// <param name="BucketTime">Duration of one bucket, where statistics is collected.</param>
		/// <param name="Title">Title of buckets. %ID% will be replaced by the ID of each bucket.</param>
		/// <param name="LabelY">Y-label of buckets. %ID% will be replaced by the ID of each bucket.</param>
		/// <param name="Model">Simulation model.</param>
		/// <param name="Buckets">Predefined buckets.</param>
		public Buckets(DateTime StartTime, Duration BucketTime, string Title, string LabelY, Model Model, params IBucket[] Buckets)
		{
			this.start = StartTime;
			this.bucketTime = BucketTime;
			this.title = Title;
			this.labelY = LabelY;
			this.model = Model;

			if (!(Buckets is null))
			{
				foreach (IBucket Bucket in Buckets)
					this.buckets[Bucket.Id] = Bucket;
			}
		}

		/// <summary>
		/// Counts an event.
		/// </summary>
		/// <param name="Counter">Counter ID</param>
		public void CountEvent(string Counter)
		{
			IBucket Bucket;

			lock (this.buckets)
			{
				if (!this.buckets.TryGetValue(Counter, out Bucket))
				{
					Bucket = new Bucket(Counter, this.GetTitle(Counter), this.labelY, this.model, false, this.start, this.bucketTime);
					this.buckets[Counter] = Bucket;
				}
			}

			this.start = Bucket.CountOccurrence(DateTime.Now);
		}

		/// <summary>
		/// Increments a counter.
		/// </summary>
		/// <param name="Counter">Counter ID</param>
		public void IncrementCounter(string Counter)
		{
			IBucket Bucket;

			lock (this.buckets)
			{
				if (!this.buckets.TryGetValue(Counter, out Bucket))
				{
					Bucket = new Bucket(Counter, this.GetTitle(Counter), this.labelY, this.model, false, this.start, this.bucketTime);
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
			IBucket Bucket;

			lock (this.buckets)
			{
				if (!this.buckets.TryGetValue(Counter, out Bucket))
				{
					Bucket = new Bucket(Counter, this.GetTitle(Counter), this.labelY, this.model, false, this.start, this.bucketTime);
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
			IBucket Bucket;

			lock (this.buckets)
			{
				if (!this.buckets.TryGetValue(Counter, out Bucket))
				{
					Bucket = new Bucket(Counter, this.GetTitle(Counter), this.labelY, this.model, true, this.start, this.bucketTime);
					this.buckets[Counter] = Bucket;
				}
			}

			this.start = Bucket.Sample(DateTime.Now, Value);
		}

		/// <summary>
		/// Samples a value
		/// </summary>
		/// <param name="Counter">Counter ID</param>
		/// <param name="Value">Value</param>
		public void Sample(string Counter, PhysicalQuantity Value)
		{
			IBucket Bucket;

			lock (this.buckets)
			{
				if (!this.buckets.TryGetValue(Counter, out Bucket))
				{
					Bucket = new Bucket(Counter, this.GetTitle(Counter), this.labelY, this.model, true, this.start, this.bucketTime);
					this.buckets[Counter] = Bucket;
				}
			}

			this.start = Bucket.Sample(DateTime.Now, Value);
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
					foreach (IBucket Bucket in this.buckets.Values)
						Result.Add(Bucket.Id, Bucket.TotalCount);
				}
				else
				{
					foreach (string Id in Order)
					{
						if (this.buckets.TryGetValue(Id, out IBucket Bucket))
							Result.Add(Bucket.Id, Bucket.TotalCount);
					}
				}
			}

			return Result;
		}

		/// <summary>
		/// Exports data to XML
		/// </summary>
		/// <param name="Output">XML Output</param>
		/// <param name="TableElement">XML Table element name.</param>
		/// <param name="RowElement">XML Row element name.</param>
		public void ExportXml(XmlWriter Output, string TableElement, string RowElement)
		{
			Output.WriteStartElement(TableElement);

			IBucket[] Buckets;

			lock (this.buckets)
			{
				Buckets = new IBucket[this.buckets.Count];
				this.buckets.Values.CopyTo(Buckets, 0);
			}

			foreach (IBucket Bucket in Buckets)
				Bucket.ExportXml(Output, RowElement);

			Output.WriteEndElement();
		}

		/// <summary>
		/// Tries to get a bucket, given its ID.
		/// </summary>
		/// <param name="Id">Bucket ID</param>
		/// <param name="Bucket">Bucket, if found.</param>
		/// <returns>If a bucket was found.</returns>
		public bool TryGetBucket(string Id, out IBucket Bucket)
		{
			lock (this.buckets)
			{
				return this.buckets.TryGetValue(Id, out Bucket);
			}
		}

		/// <summary>
		/// Gets a sample bucket.
		/// </summary>
		/// <param name="Id">Bucket ID</param>
		/// <returns>Bucket.</returns>
		public IBucket GetSampleBucket(string Id)
		{
			IBucket Bucket;

			lock (this.buckets)
			{
				if (!this.buckets.TryGetValue(Id, out Bucket))
				{
					Bucket = new Bucket(Id, this.GetTitle(Id), this.labelY, this.model, true, this.start, this.bucketTime);
					this.buckets[Id] = Bucket;
				}
			}

			return Bucket;
		}

		/// <summary>
		/// Gets a count bucket.
		/// </summary>
		/// <param name="Id">Bucket ID</param>
		/// <returns>Bucket.</returns>
		public IBucket GetCountBucket(string Id)
		{
			IBucket Bucket;

			lock (this.buckets)
			{
				if (!this.buckets.TryGetValue(Id, out Bucket))
				{
					Bucket = new Bucket(Id, this.GetTitle(Id), this.labelY, this.model, false, this.start, this.bucketTime);
					this.buckets[Id] = Bucket;
				}
			}

			return Bucket;
		}

		/// <summary>
		/// Registers a custom bucket.
		/// </summary>
		/// <param name="Bucket">Bucket</param>
		public void Register(IBucket Bucket)
		{
			lock (this.buckets)
			{
				if (this.buckets.ContainsKey(Bucket.Id))
					throw new Exception("A bucket with ID " + Bucket.Id + " already registered.");

				this.buckets[Bucket.Id] = Bucket;
			}
		}

		/// <summary>
		/// Exports historical counts as a graph.
		/// </summary>
		/// <param name="Title">Title of graph.</param>
		/// <param name="Order">Preferred order, can be null.</param>
		/// <param name="Output">Export destination</param>
		/// <param name="Model">Simulation model</param>
		/// <param name="Events">Associated event objects.</param>
		/// <param name="Palette">Optional predefined palette</param>
		public void ExportCountHistoryGraph(string Title, IEnumerable<string> Order, StreamWriter Output, Model Model,
			IEnumerable<IEvent> Events, SKColor[] Palette = null)
		{
			lock (this.buckets)
			{
				SortedDictionary<DateTime, long> Counts = new SortedDictionary<DateTime, long>();
				int Count = 0;

				if (Order is null)
					Order = this.buckets.Keys;

				foreach (string ActivityId in Order)
				{
					if (this.TryGetBucket(ActivityId, out IBucket Bucket))
					{
						if (Bucket is IPeriodBucket PeriodBucket)
						{
							PeriodBucket.Flush();

							Statistic Last = null;

							foreach (Statistic Rec in PeriodBucket)
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
				}

				List<string> Labels = new List<string>();
				List<SKColor> LabelColors = new List<SKColor>();
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

				StringBuilder PlotScript2 = null;
				int Index = 0;
				string Alias;

				foreach (string ActivityId in Order)
				{
					Index++;
					Alias = "C" + Index.ToString();

					if (this.TryGetBucket(ActivityId, out IBucket Bucket))
					{
						if (Bucket is IPeriodBucket PeriodBucket)
						{
							foreach (Statistic Rec in PeriodBucket)
							{
								if (!Counts.TryGetValue(Rec.Stop, out long Value))
									Value = 0;

								Value += Rec.Count;
								Counts[Rec.Stop] = Value;
							}

							First = true;
							Color = Palette[i++];

							Script.Append(Alias);
							Script.Append(":=[");

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
							{
								PlotScript = new StringBuilder();
								PlotScript2 = new StringBuilder();
							}
							else
							{
								PlotScript.Append('+');
								PlotScript2.Append('+');
							}

							PlotScript.Append("plot2dlinearea(Time,");
							PlotScript.Append(Alias);
							PlotScript.Append(",rgba(");
							PlotScript.Append(Color.Red.ToString());
							PlotScript.Append(',');
							PlotScript.Append(Color.Green.ToString());
							PlotScript.Append(',');
							PlotScript.Append(Color.Blue.ToString());
							PlotScript.Append(",64))");

							PlotScript2.Append("plot2dline(Time,");
							PlotScript2.Append(Alias);
							PlotScript2.Append(",rgb(");
							PlotScript2.Append(Color.Red.ToString());
							PlotScript2.Append(',');
							PlotScript2.Append(Color.Green.ToString());
							PlotScript2.Append(',');
							PlotScript2.Append(Color.Blue.ToString());
							PlotScript2.Append("),3)");

							Labels.Add(ActivityId);
							LabelColors.Add(Color);
						}
					}
				}

				if (!(PlotScript2 is null))
				{
					PlotScript.Append('+');
					PlotScript.Append(PlotScript2.ToString());
				}

				bool PdfShown = false;

				if (!(Events is null))
				{
					LinkedList<IDistribution> Distributions = null;

					foreach (IEvent Event in Events)
					{
						IDistribution Distribution = Event.Distribution;
						if (Distribution is null)
							continue;

						if (Distributions is null)
							Distributions = new LinkedList<IDistribution>();

						Distributions.AddLast(Distribution);
						Distribution.ExportPdfOnceOnly(Script);
					}

					if (!(Distributions is null))
					{
						double t2 = Model.GetTimeCoordinates(Model.EndTime);
						double dt = t2 / 1000;
						bool First2 = true;
						string s;

						Script.Append("t:=0..");
						Script.Append(CommonTypes.Encode(t2));
						Script.Append("|");
						Script.Append(CommonTypes.Encode(dt));
						Script.AppendLine(";");

						if (Model.TimeCycleUnits != 1)
						{
							Script.Append("ct:=t/");
							Script.Append(s = CommonTypes.Encode(Model.TimeCycleUnits));
							Script.AppendLine(";");
							Script.Append("ct:=(ct-floor(ct))*");
							Script.Append(s);
							Script.AppendLine(";");
						}
						else
							Script.AppendLine("ct:=t-floor(t);");

						PlotScript.Append("+plot2dline(t,(");

						foreach (IDistribution Distribution in Distributions)
						{
							if (First2)
								First2 = false;
							else
								PlotScript.Append('+');

							PlotScript.Append(Distribution.Id);
							PlotScript.Append("PDF(ct)");
						}

						PlotScript.Append(')');

						if (Model.BucketTimeMs != Model.TimeUnitMs)
						{
							PlotScript.Append(".*");
							PlotScript.Append(CommonTypes.Encode(Model.BucketTimeMs / Model.TimeUnitMs));
						}

						PlotScript.AppendLine(",\"Blue\",3)");
						PdfShown = true;
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

				if (PdfShown)
				{
					Labels.Add("Expected intensity");
					LabelColors.Add(SKColors.Blue);
					CombinedGraph.ExportLegend(Output, Labels.ToArray(), LabelColors.ToArray());
				}
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
