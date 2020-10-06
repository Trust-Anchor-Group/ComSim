using System;
using System.IO;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Activities
{
	/// <summary>
	/// Interface for condition nodes
	/// </summary>
	public interface IConditionNode : IActivityNode
	{
		/// <summary>
		/// If the node condition is true.
		/// </summary>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>If embedded nodes are to be executed.</returns>
		bool IsTrue(Variables Variables);

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
