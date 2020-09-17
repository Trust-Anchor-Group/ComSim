using System;
using System.Collections;
using System.Collections.Generic;
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
				DateTime Now = DateTime.Now;
				while (Now >= this.stop)
					this.NextBucketLocked();

				this.count++;
				this.totCount++;

				return this.start;
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
	}
}
