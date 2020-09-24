using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;

namespace TAG.Simulator
{
	/// <summary>
	/// Basic interface for simulator nodes with child nodes.
	/// </summary>
	public interface ISimulationNodeChildren : ISimulationNode
	{
		/// <summary>
		/// Child nodes.
		/// </summary>
		ISimulationNode[] Children
		{
			get;
		}
	}
}
