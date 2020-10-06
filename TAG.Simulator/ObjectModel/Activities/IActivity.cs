using System;
using System.IO;
using System.Threading.Tasks;
using TAG.Simulator.ObjectModel.Events;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Activities
{
	/// <summary>
	/// Interface for activities
	/// </summary>
	public interface IActivity : ISimulationNodeChildren
	{
		/// <summary>
		/// ID of activity.
		/// </summary>
		string Id
		{
			get;
		}

		/// <summary>
		/// If event should be logged at each start of the activity.
		/// </summary>
		bool LogStart
		{
			get;
		}

		/// <summary>
		/// If event should be logged at the end of each activity.
		/// </summary>
		bool LogEnd
		{
			get;
		}

		/// <summary>
		/// Execution count
		/// </summary>
		int ExecutionCount
		{
			get;
		}

		/// <summary>
		/// Registers a child activity node.
		/// </summary>
		/// <param name="Node">Activity node.</param>
		void Register(IActivityNode Node);

		/// <summary>
		/// Registers an event that calls the activity.
		/// </summary>
		/// <param name="Event">Event.</param>
		void Register(IEvent Event);

		/// <summary>
		/// Executes the activity.
		/// </summary>
		/// <param name="Variables">Set of variables for the activity.</param>
		Task ExecuteTask(Variables Variables);
	}
}
