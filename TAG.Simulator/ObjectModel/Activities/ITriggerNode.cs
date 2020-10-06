using System;
using System.IO;
using System.Threading.Tasks;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Activities
{
	/// <summary>
	/// Interface for trigger nodes
	/// </summary>
	public interface ITriggerNode : IActivityNode
	{
		/// <summary>
		/// Gets a task object.
		/// </summary>
		/// <returns>Task object signalling when trigger is activated.</returns>
		Task GetTask();

		/// <summary>
		/// Exports PlantUML
		/// </summary>
		/// <param name="Output">Output</param>
		/// <param name="Indentation">Number of tabs to indent.</param>
		/// <param name="First">If the condition is the first condition.</param>
		/// <param name="QuoteChar">Quote character.</param>
		void ExportPlantUml(StreamWriter Output, int Indentation, bool First, char QuoteChar);
	}
}
