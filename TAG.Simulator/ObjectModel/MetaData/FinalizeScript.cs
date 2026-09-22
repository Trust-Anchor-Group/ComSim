using System.Threading.Tasks;
using System.Xml;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.MetaData
{
	/// <summary>
	/// Executes script at the end of the simulation, allowing for persistance and
	/// cleaning of states.
	/// </summary>
	public class FinalizeScript : ScriptNode
	{
		/// <summary>
		/// Executes model script, allowing for definitions that will be available across events.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public FinalizeScript(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(FinalizeScript);

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new FinalizeScript(Parent, Model);
		}
		/// <summary>
		/// Initialized the node before simulation.
		/// </summary>
		public override async Task Finalize()
		{
			await this.Expression.EvaluateAsync(this.Model.Variables);
			await base.Initialize();
		}
	}
}
