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
		/// Executes the activity.
		/// </summary>
		/// <param name="Variables">Set of variables for the activity.</param>
		Task ExecuteTask(Variables Variables);

		/// <summary>
		/// Exports Markdown
		/// </summary>
		/// <param name="Output">Output node</param>
		/// <param name="Title">It title should be exported.</param>
		/// <param name="Event">Connected event object.</param>
		Task ExportMarkdown(StreamWriter Output, bool Title, IEvent Event);
	}
}
