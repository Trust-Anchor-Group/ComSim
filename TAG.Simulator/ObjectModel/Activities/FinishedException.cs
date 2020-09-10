using System;
using System.Collections.Generic;
using System.Text;

namespace TAG.Simulator.ObjectModel.Activities
{
	/// <summary>
	/// Exceptions thrown when the execution of an activity is completed.
	/// </summary>
	public class FinishedException : Exception
	{
		/// <summary>
		/// Exceptions thrown when the execution of an activity is completed.
		/// </summary>
		public FinishedException()
			: base()
		{
		}
	}
}
