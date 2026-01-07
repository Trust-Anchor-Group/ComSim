using System.Collections.Generic;

namespace TAG.Simulator.Statistics
{
	/// <summary>
	/// Statistical bucket with periods
	/// </summary>
	public interface IPeriodBucket : IBucket, IEnumerable<Statistic>
	{
	}
}
