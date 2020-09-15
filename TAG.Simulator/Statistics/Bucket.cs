using System;
using System.Collections.Generic;
using System.IO;

namespace TAG.Simulator.Statistics
{
	/// <summary>
	/// Statistical bucket
	/// </summary>
	public class Bucket
	{
		private readonly LinkedList<double> samples;
		private readonly string id;
		private long count = 0;
		private double sum = 0;
		private double min = double.MaxValue;
		private double max = double.MinValue;

		/// <summary>
		/// Statistical bucket
		/// </summary>
		/// <param name="Id">ID of bucket.</param>
		/// <param name="CalcStdDev">If standard deviation is to be calculated.</param>
		public Bucket(string Id, bool CalcStdDev)
		{
			this.id = Id;
			this.samples = CalcStdDev ? new LinkedList<double>() : null;
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
		public void Inc()
		{
			lock (this)
			{
				this.count++;
			}
		}

		/// <summary>
		/// Adds a sample
		/// </summary>
		/// <param name="Value"></param>
		public void Sample(double Value)
		{
			lock (this)
			{
				this.sum += Value;
				this.count++;

				if (Value < this.min)
					this.min = Value;

				if (Value > this.max)
					this.max = Value;

				this.samples?.AddLast(Value);
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

				double μ = this.Mean;
				double S = 0;
				double x;

				lock (this)
				{

					foreach (double d in this.samples)
					{
						x = (d - μ);
						S += x * x;
					}
			
					return S / this.count;
				}
			}
		}

		/// <summary>
		/// (Biased) standard deviation of samples
		/// </summary>
		public double StdDev => Math.Sqrt(this.Variance);
	}
}
