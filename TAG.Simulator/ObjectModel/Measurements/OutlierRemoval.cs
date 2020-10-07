using System;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.Statistics;
using Waher.Content.Xml;

namespace TAG.Simulator.ObjectModel.Measurements
{
	/// <summary>
	/// Removes outliers by comparing incoming samples with the average of the last samples.
	/// </summary>
	public class OutlierRemoval : SeriesReference, IFilter
	{
		private object synchObj = new object();
		private IFilter filter;
		private DateTime[] timespans;
		private double?[] values;
		private double sum;
		private int count;
		private int pos;
		private int avgPos;
		private int windowSize;
		private int threshold;
		private bool smooth;

		/// <summary>
		/// Removes outliers by comparing incoming samples with the average of the last samples.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public OutlierRemoval(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Number of samples to include in the average calculation.
		/// </summary>
		public int WindowSize => this.windowSize;

		/// <summary>
		/// Threshold for outlier detection.
		/// </summary>
		public int Threshold => this.threshold;

		/// <summary>
		/// If the average value should be used to also smooth the output.
		/// </summary>
		public bool Smooth => this.smooth;

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "OutlierRemoval";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new OutlierRemoval(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.windowSize = XML.Attribute(Definition, "windowSize", 0);
			this.threshold = XML.Attribute(Definition, "threshold", 0);
			this.smooth = XML.Attribute(Definition, "smooth", false);

			if (this.windowSize < 1)
				throw new Exception("Windows size must be positive.");

			if (this.threshold < 1)
				throw new Exception("Threshold must be positive.");

			this.timespans = new DateTime[this.windowSize];
			this.values = new double?[this.windowSize];
			this.sum = 0;
			this.count = 0;
			this.pos = 0;
			this.avgPos = this.count / 2;

			return base.FromXml(Definition);
		}

		/// <summary>
		/// Starts the node.
		/// </summary>
		public override async Task Start()
		{
			await base.Start();
			this.Bucket.Add(this);
		}

		/// <summary>
		/// Appends a filter to the current filter.
		/// </summary>
		/// <param name="Filter">Filter to append.</param>
		public void Append(IFilter Filter)
		{
			if (this.filter is null)
				this.filter = Filter;
			else
				this.filter.Append(Filter);
		}

		/// <summary>
		/// Filters a value
		/// </summary>
		/// <param name="Timestamp">Timestamp of value</param>
		/// <param name="Value">Value</param>
		/// <returns>If value should be discarded</returns>
		public bool Filter(ref DateTime Timestamp, ref double Value)
		{
			lock (this.synchObj)
			{
				double? Old = this.values[this.pos];

				this.timespans[this.pos] = Timestamp;
				this.values[this.pos] = Value;

				if (++this.pos == this.windowSize)
					this.pos = 0;

				if (Old.HasValue)
				{
					this.sum -= Old.Value;
					this.count--;
				}

				this.sum += Value;
				this.count++;

				double Average = this.sum / this.count;
				int NrAbove = 0;
				int NrBelow = 0;
				int i;

				for (i = 0; i < this.windowSize; i++)
				{
					Old = this.values[i];
					if (Old.HasValue)
					{
						if (Old.Value > Average)
							NrAbove++;
						else if (Old.Value < Average)
							NrBelow++;
					}
				}

				Timestamp = this.timespans[this.avgPos];
				Old = this.values[this.avgPos];

				if (NrAbove <= this.threshold && NrBelow > this.threshold)
				{
					if (Old.HasValue && Old.Value > Average)
					{
						this.sum -= Old.Value;
						this.count--;
						this.values[this.avgPos] = Old = null;
					}
				}
				else if (NrBelow <= this.threshold && NrAbove > this.threshold)
				{
					if (Old.HasValue && Old.Value < Average)
					{
						this.sum -= Old.Value;
						this.count--;
						this.values[this.avgPos] = Old = null;
					}
				}

				if (++this.avgPos == this.windowSize)
					this.avgPos = 0;

				if (!Old.HasValue)
					return true;

				if (this.smooth)
					Value = Average;
				else
					Value = Old.Value;

				return false;
			}
		}
	}
}
