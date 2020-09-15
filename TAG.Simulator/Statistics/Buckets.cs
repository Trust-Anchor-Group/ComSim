using System;
using System.Collections.Generic;
using System.Text;

namespace TAG.Simulator.Statistics
{
	/// <summary>
	/// A collection of buckets
	/// </summary>
	public class Buckets
	{
		private readonly SortedDictionary<string, Bucket> buckets = new SortedDictionary<string, Bucket>();

		/// <summary>
		/// A collection of buckets
		/// </summary>
		public Buckets()
		{
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
					Bucket = new Bucket(Counter, false);
					this.buckets[Counter] = Bucket;
				}
			}

			Bucket.Inc();
		}

		/// <summary>
		/// Number of counters
		/// </summary>
		public int Count
		{
			get
			{
				lock(this.buckets)
				{
					return this.buckets.Count;
				}
			}
		}

		/// <summary>
		/// Gets a count table of registered counters.
		/// </summary>
		/// <returns>Count table</returns>
		public CountTable GetTable()
		{
			CountTable Result = new CountTable();

			lock (this.buckets)
			{
				foreach (Bucket Bucket in this.buckets.Values)
					Result.Add(Bucket.Id, Bucket.Count);
			}

			return Result;
		}
	}
}
