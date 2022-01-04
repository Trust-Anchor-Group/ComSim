using System;

namespace TAG.Simulator.Statistics
{
	/// <summary>
	/// Interface for sample filters
	/// </summary>
	public interface IFilter
	{
		/// <summary>
		/// Appends a filter to the current filter.
		/// </summary>
		/// <param name="Filter">Filter to append.</param>
		void Append(IFilter Filter);

		/// <summary>
		/// Filters a value
		/// </summary>
		/// <param name="Timestamp">Timestamp of value</param>
		/// <param name="Value">Value</param>
		/// <returns>If value should be discarded</returns>
		bool Filter(ref DateTime Timestamp, ref double Value);
	}
}
