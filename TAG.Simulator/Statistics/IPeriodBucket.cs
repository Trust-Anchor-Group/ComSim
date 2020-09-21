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
	/// Statistical bucket with periods
	/// </summary>
	public interface IPeriodBucket : IBucket, IEnumerable<Statistic>
	{
	}
}
