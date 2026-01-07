using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Events
{
	/// <summary>
	/// Interface for event preparation nodes
	/// </summary>
	public interface IEventPreparation : ISimulationNode
	{
		/// <summary>
		/// Prepares <paramref name="Variables"/> for the execution of an event.
		/// </summary>
		/// <param name="Variables">Event variables</param>
		/// <param name="Tags">Extensible list of meta-data tags related to the event.</param>
		Task Prepare(Variables Variables, List<KeyValuePair<string, object>> Tags);

		/// <summary>
		/// Releases resources at the end of an event.
		/// </summary>
		/// <param name="Variables">Event variables</param>
		void Release(Variables Variables);

		/// <summary>
		/// Exports the node to PlantUML script in a markdown document.
		/// </summary>
		/// <param name="Output">Output stream.</param>
		/// <param name="Name">Optional name for the association.</param>
		/// <param name="Index">Chart Index</param>
		void ExportPlantUml(StreamWriter Output, string Name, int Index);
	}
}
