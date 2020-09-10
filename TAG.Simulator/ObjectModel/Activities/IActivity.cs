using System;
using System.Threading.Tasks;
using System.Xml;
using Waher.Content.Xml;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Activities
{
	/// <summary>
	/// Interface for activities
	/// </summary>
	public interface IActivity : ISimulationNode
	{
		/// <summary>
		/// ID of activity.
		/// </summary>
		string Id
		{
			get;
		}

		/// <summary>
		/// Registers a child activity node.
		/// </summary>
		/// <param name="Model">Model being executed.</param>
		/// <param name="Node">Activity node.</param>
		void Register(Model Model, IActivityNode Node);

		/// <summary>
		/// Executes the activity.
		/// </summary>
		/// <param name="Model">Current model</param>
		/// <param name="Variables">Set of variables for the activity.</param>
		Task ExecuteTask(Model Model, Variables Variables);
	}
}
