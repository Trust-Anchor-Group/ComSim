using TAG.Simulator.ObjectModel.Events;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Actors
{
	/// <summary>
	/// Basic interface for simulator nodes. Implementing this interface allows classes with default contructors to be used
	/// in simulator models.
	/// </summary>
	public interface IActor : ISimulationNodeChildren, IExternalEventsNode
	{
		/// <summary>
		/// ID of actor instance.
		/// </summary>
		string InstanceId
		{
			get;
		}

		/// <summary>
		/// Collection of actor-variables.
		/// </summary>
		Variables Variables
		{
			get;
		}

		/// <summary>
		/// Number of individuals in population that are free.
		/// </summary>
		int FreeCount
		{
			get;
		}

		/// <summary>
		/// Gets a free individual instance from the population.
		/// </summary>
		/// <param name="Index">Zero-based index of individual to get.</param>
		/// <param name="Exclusive">If individual is for exclusive use (i.e. will not be free once gotten, until returned).</param>
		/// <returns>Individual instance returned.</returns>
		IActor GetFreeIndividual(int Index, bool Exclusive);

		/// <summary>
		/// Returns an individual to the population, once free again.
		/// </summary>
		/// <param name="Individual">Individual to return.</param>
		void ReturnIndividual(IActor Individual);

		/// <summary>
		/// Returns the object that will be used by the actor for actions during an activity.
		/// </summary>
		object ActivityObject
		{
			get;
		}

		/// <summary>
		/// References to created instances.
		/// </summary>
		IActor[] Instances
		{
			get;
		}

	}
}
