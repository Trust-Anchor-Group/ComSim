using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Waher.Content;
using Waher.Script.Objects;
using Waher.Script.Units;

namespace TAG.Simulator.Statistics
{
	/// <summary>
	/// Interface for buckets.
	/// </summary>
	public interface IBucket
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
		/// <param name="Value">Sample value reported</param>
		/// <returns>Start time of bucket to which the value was reported.</returns>
		DateTime Sample(PhysicalQuantity Value);

		/// <summary>
		/// Adds a sample
		/// </summary>
		/// <param name="Value">Sample value reported</param>
		/// <returns>Start time of bucket to which the value was reported.</returns>
		DateTime Sample(double Value);

		/// <summary>
		/// Counts one occurrence
		/// </summary>
		/// <returns>Start time of bucket to which the value was reported.</returns>
		DateTime CountOccurrence();

		/// <summary>
		/// Terminates the ongoing collection of data.
		/// </summary>
		void Flush();

		/// <summary>
		/// Exports historical counts as a graph.
		/// </summary>
		/// <param name="Title">Title of graph.</param>
		/// <param name="LabelY">Label for Y-axis.</param>
		/// <param name="Output">Export destination</param>
		/// <param name="Model">Simulation model</param>
		void ExportSampleHistoryGraph(string Title, string LabelY, StreamWriter Output, Model Model);

		/// <summary>
		/// Exports data to XML
		/// </summary>
		/// <param name="Output">XML Output</param>
		/// <param name="RowElement">XML Row element name.</param>
		void ExportXml(XmlWriter Output, string RowElement);
	}
}
