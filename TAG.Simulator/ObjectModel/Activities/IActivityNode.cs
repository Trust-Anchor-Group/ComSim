using System;
using System.Threading.Tasks;
using System.Xml;
using Waher.Content.Xml;

namespace TAG.Simulator.ObjectModel.Activities
{
	/// <summary>
	/// Interface for activity nodes
	/// </summary>
	public interface IActivityNode : ISimulationNode
	{
		/// <summary>
		/// ID of activity node.
		/// </summary>
		string Id
		{
			get;
		}
	}
}
