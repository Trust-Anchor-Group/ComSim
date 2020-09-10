using System;
using System.Threading.Tasks;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Values
{
	/// <summary>
	/// Abstract base class for values
	/// </summary>
	public abstract class Value : SimulationNode, IValue
	{
		/// <summary>
		/// Abstract base class for values
		/// </summary>
		/// <param name="Parent">Parent node</param>
		public Value(ISimulationNode Parent)
			: base(Parent)
		{
		}

		/// <summary>
		/// Initialized the node before simulation.
		/// </summary>
		/// <param name="Model">Model being executed.</param>
		public override Task Initialize(Model Model)
		{
			return base.Initialize(Model);
		}

		/// <summary>
		/// Evaluates the value.
		/// </summary>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>Evaluated value.</returns>
		public abstract object Evaluate(Variables Variables);

	}
}
