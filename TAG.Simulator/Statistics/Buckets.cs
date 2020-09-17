using System;
using System.Collections.Generic;
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
		{
			this.start = StartTime;
			this.bucketTime = BucketTime;
		}

		/// <summary>
		/// Increments a counter.
		/// </summary>
		/// <param name="Counter">Counter ID</param>
		public void Inc(string Counter)
		{
			Bucket Bucket;

			lock (this.buckets)
			{
				if (!this.buckets.TryGetValue(Counter, out Bucket))
				{
					Bucket = new Bucket(Counter, false, this.start, this.bucketTime);
					this.buckets[Counter] = Bucket;
				}
			}

			this.start = Bucket.Inc();
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
					Bucket = new Bucket(Counter, false, this.start, this.bucketTime);
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
		public CountTable GetTable(string[] Order = null)
		{
			CountTable Result = new CountTable();

			lock (this.buckets)
			{
				if (Order is null)
				{
					foreach (Bucket Bucket in this.buckets.Values)
						Result.Add(Bucket.Id, Bucket.Count);
				}
				else
				{
					foreach (string Id in Order)
					{
						if (this.buckets.TryGetValue(Id, out Bucket Bucket))
							Result.Add(Bucket.Id, Bucket.Count);
					}
				}
			}

			return Result;
		}
	}
}
