using System;
using System.IO;
using System.Threading.Tasks;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Values.BinaryElements
{
	/// <summary>
	/// Interface for binary elements
	/// </summary>
	public interface IBinaryElement : ISimulationNode
	{
		/// <summary>
		/// Appends the binary element to the output stream.
		/// </summary>
		/// <param name="Output">Output stream.</param>
		/// <param name="Variables">Set of variables for the activity.</param>
		Task Append(MemoryStream Output, Variables Variables);
	}
}
