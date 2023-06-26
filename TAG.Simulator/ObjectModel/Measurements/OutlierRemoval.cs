using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.Statistics;
using Waher.Content.Xml;
using Waher.Events;

namespace TAG.Simulator.ObjectModel.Measurements
{
	/// <summary>
	/// Removes outliers by comparing incoming samples with the average of the last samples.
	/// </summary>
	public class OutlierRemoval : SeriesReference, IFilter
	{
		private readonly object synchObj = new object();
		private IFilter filter;
		private DateTime[] timespans;
		private double?[] values;
		private double? min;
		private double? max;
		private double sum;
		private int count;
		private int pos;
		private int avgPos;
		private int windowSize;
		private int threshold;
		private bool smooth;
		private bool logNotice;

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
		public override string LocalName => nameof(OutlierRemoval);

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
			this.logNotice = XML.Attribute(Definition, "logNotice", true);
			this.max = Definition.HasAttribute("max") ? XML.Attribute(Definition, "max", 0.0) : (double?)null;
			this.min = Definition.HasAttribute("min") ? XML.Attribute(Definition, "min", 0.0) : (double?)null;

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
			if (this.min.HasValue && Value < this.min.Value)
			{
				if (this.logNotice)
				{
					Log.Notice("Outlier removed. Smaller than minimum value.", this.For, string.Empty, "Outlier",
						new KeyValuePair<string, object>("Value", Value));
				}

				this.Model.CountEvent(this.For + " Outlier");

				return true;
			}

			if (this.max.HasValue && Value > this.max.Value)
			{
				if (this.logNotice)
				{
					Log.Notice("Outlier removed. Larger than maximum value.", this.For, string.Empty, "Outlier",
						new KeyValuePair<string, object>("Value", Value));
				}

				this.Model.CountEvent(this.For + " Outlier");

				return true;
			}

			double? Old;
			double Average;
			int NrAbove = 0;
			int NrBelow = 0;
			int i;

			lock (this.synchObj)
			{
				Old = this.values[this.pos];

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

				Average = this.sum / this.count;

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
			}

			if (!Old.HasValue)
			{
				if (this.logNotice)
				{
					Log.Notice("Outlier removed. Threshold reached.", this.For, string.Empty, "Outlier",
						new KeyValuePair<string, object>("Value", Value));
				}

				this.Model.CountEvent(this.For + " Outlier");

				return true;
			}

			if (this.smooth)
				Value = Average;
			else
				Value = Old.Value;

			return false;
		}
	}
}
