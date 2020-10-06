using System;
using System.IO;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Values
{
	/// <summary>
	/// Interface for values
	/// </summary>
	public interface IValue : ISimulationNode
	{
		/// <summary>
		/// Evaluates the value.
		/// </summary>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>Evaluated value.</returns>
		object Evaluate(Variables Variables);

		/// <summary>
		/// Exports PlantUML
		/// </summary>
		/// <param name="Output">Output</param>
		/// <param name="Indentation">Number of tabs to indent.</param>
		/// <param name="QuoteChar">Quote character.</param>
		void ExportPlantUml(StreamWriter Output, int Indentation, char QuoteChar);
	}
}
