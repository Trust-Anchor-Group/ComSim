using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using TAG.Simulator.ObjectModel.Graphs;
using Waher.Content;
using Waher.Script.Objects;
using Waher.Script.Units;

namespace TAG.Simulator.Statistics
{
	/// <summary>
	/// Interface for buckets.
	/// </summary>
	public interface IBucket : IGraph
	{
		/// <summary>
		/// Bucket ID
		/// </summary>
		string Id
		{
			get;
		}

		/// <summary>
		/// Total Counter
		/// </summary>
		long TotalCount
		{
			get;
		}

		/// <summary>
		/// Time to accumulate values.
		/// </summary>
		Duration BucketTime
		{
			get;
			set;
		}

		/// <summary>
		/// Increments counter.
		/// </summary>
		/// <returns>Start time of bucket that was incremented.</returns>
		DateTime Inc();

		/// <summary>
		/// Decrements counter.
		/// </summary>
		/// <returns>Start time of bucket that was incremented.</returns>
		DateTime Dec();

		/// <summary>
		/// Adds a sample
		/// </summary>
		/// <param name="Timestamp">Timestamp of value.</param>
		/// <param name="Value">Sample value reported</param>
		/// <returns>Start time of bucket to which the value was reported.</returns>
		DateTime Sample(DateTime Timestamp, PhysicalQuantity Value);

		/// <summary>
		/// Adds a sample
		/// </summary>
		/// <param name="Timestamp">Timestamp of value.</param>
		/// <param name="Value">Sample value reported</param>
		/// <returns>Start time of bucket to which the value was reported.</returns>
		DateTime Sample(DateTime Timestamp, double Value);

		/// <summary>
		/// Counts one occurrence
		/// </summary>
		/// <param name="Timestamp">Timestamp of occurrence.</param>
		/// <returns>Start time of bucket to which the value was reported.</returns>
		DateTime CountOccurrence(DateTime Timestamp);

		/// <summary>
		/// Terminates the ongoing collection of data.
		/// </summary>
		void Flush();

		/// <summary>
		/// Exports data to XML
		/// </summary>
		/// <param name="Output">XML Output</param>
		/// <param name="RowElement">XML Row element name.</param>
		void ExportXml(XmlWriter Output, string RowElement);

		/// <summary>
		/// Adds a filter to the bucket.
		/// </summary>
		/// <param name="Filter">Filter</param>
		void Add(IFilter Filter);
	}
}
