using System.Threading.Tasks;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Actors
{
	/// <summary>
	/// Executes script when an instance is created, allowing for definitions that will be 
	/// available across events for that particular instance.
	/// </summary>
	public class InstanceScript : ScriptNode, IInstanceNode
	{
		/// <summary>
		/// Executes script when an instance is created, allowing for definitions that will be 
		/// available across events for that particular instance.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public InstanceScript(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(InstanceScript);

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new InstanceScript(Parent, Model);
		}

		/// <summary>
		/// Configures an actor instance.
		/// </summary>
		/// <param name="Variables">Collection of instance variables.</param>
		public async Task ConfigureInstance(Actor ActorInstance, Variables Variables)
		{
			await this.Expression.EvaluateAsync(Variables);
		}
	}
}
