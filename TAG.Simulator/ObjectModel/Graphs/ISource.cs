using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Waher.Content.Xml;

namespace TAG.Simulator.ObjectModel.Graphs
{
	/// <summary>
	/// Interface for sources
	/// </summary>
	public interface ISource : ISimulationNode
	{
		/// <summary>
		/// Reference
		/// </summary>
		string Reference
		{
			get;
		}
	}
}
