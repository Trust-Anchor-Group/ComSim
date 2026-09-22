using System.Threading.Tasks;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Actors
{
	/// <summary>
	/// Interface for nodes used to configure an instance of an actor.
	/// </summary>
	public interface IInstanceNode : ISimulationNode
	{
		/// <summary>
		/// Configures an actor instance.
		/// </summary>
		/// <param name="Variables">Collection of instance variables.</param>
		Task ConfigureInstance(Actor ActorInstance, Variables Variables);
	}
}
