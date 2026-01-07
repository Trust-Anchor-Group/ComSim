using TAG.Simulator.ObjectModel.Actors;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Events
{
	/// <summary>
	/// Event variables
	/// </summary>
	public class EventVariables : Variables
	{
		private readonly Variables modelVariables;

		/// <summary>
		/// Event variables
		/// </summary>
		/// <param name="ModelVariables">Model variables.</param>
		/// <param name="Actor">Optional Actor</param>
		public EventVariables(Variables ModelVariables, IActor Actor)
			: base()
		{
			this.modelVariables = ModelVariables;

			if (!(Actor is null))
				this.Add("Actor", Actor.Variables);
		}

		/// <summary>
		/// Model variables
		/// </summary>
		public Variables ModelVariables => this.modelVariables;

		/// <summary>
		/// If the collection contains a variable with a given name.
		/// </summary>
		/// <param name="Name">Variable name.</param>
		/// <returns>If a variable with that name exists.</returns>
		public override bool ContainsVariable(string Name)
		{
			return base.ContainsVariable(Name) || this.modelVariables.ContainsVariable(Name);
		}

		/// <summary>
		/// Tries to get a variable object, given its name.
		/// </summary>
		/// <param name="Name">Variable name.</param>
		/// <param name="Variable">Variable, if found, or null otherwise.</param>
		/// <returns>If a variable with the corresponding name was found.</returns>
		public override bool TryGetVariable(string Name, out Variable Variable)
		{
			if (base.TryGetVariable(Name, out Variable))
				return true;

			if (this.modelVariables.TryGetVariable(Name, out Variable))
				return true;

			return false;
		}

		/// <summary>
		/// Removes a varaiable from the collection.
		/// </summary>
		/// <param name="VariableName">Name of variable.</param>
		/// <returns>If the variable was found and removed.</returns>
		public override bool Remove(string VariableName)
		{
			if (base.Remove(VariableName))
				return true;

			if (this.modelVariables.Remove(VariableName))
				return true;

			return false;
		}
	}
}
