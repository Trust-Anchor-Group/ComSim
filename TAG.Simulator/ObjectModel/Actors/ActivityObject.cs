using System;
using System.Reflection;

namespace TAG.Simulator.ObjectModel.Actors
{
	/// <summary>
	/// Base class for activity objects.
	/// </summary>
	public class ActivityObject
	{
		private readonly Actor actorInstance;

		/// <summary>
		/// Base class for activity objects.
		/// </summary>
		/// <param name="ActorInstance">Actor instance being referenced by the object.</param>
		public ActivityObject(Actor ActorInstance)
		{
			this.actorInstance = ActorInstance;
		}

		/// <summary>
		/// Access to extension objects.
		/// </summary>
		/// <param name="Index">Extension name</param>
		/// <returns>Extension object, if found.</returns>
		/// <exception cref="Exception">If no extension with the given name was found.</exception>
		public virtual object this[string Index]
		{
			get
			{
				if (this.actorInstance.Variables.TryGetVariable(Index, out Waher.Script.Variable v))
					return v.ValueObject;

				Type T = this.actorInstance.GetType();
				PropertyInfo PI = T.GetRuntimeProperty(Index);
				if (!(PI is null))
					return PI.GetValue(this.actorInstance);

				FieldInfo FI = T.GetRuntimeField(Index);
				if (!(FI is null))
					return FI.GetValue(this.actorInstance);

				throw new Exception("No instance variable or property found with name " + Index);
			}
		}
	}
}
