using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using Waher.Content.Xml;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Activities
{
	/// <summary>
	/// Represents an action on an actor.
	/// </summary>
	public class Action : ActivityNode
	{
		private Argument[] arguments;
		private string[] argumentNames;
		private string actor;
		private string action;

		/// <summary>
		/// Represents an action on an actor.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		public Action(ISimulationNode Parent)
			: base(Parent)
		{
		}

		/// <summary>
		/// Actor ID
		/// </summary>
		public string ActorId => this.actor;

		/// <summary>
		/// Action
		/// </summary>
		public string ActionName => this.action;

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "Action";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent)
		{
			return new Action(Parent);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.actor = XML.Attribute(Definition, "actor");
			this.action = XML.Attribute(Definition, "action");

			return base.FromXml(Definition);
		}

		/// <summary>
		/// Initialized the node before simulation.
		/// </summary>
		/// <param name="Model">Model being executed.</param>
		public override Task Initialize(Model Model)
		{
			List<Argument> Arguments = new List<Argument>();
			List<string> Names = new List<string>();

			foreach (ISimulationNode Node in this.Children)
			{
				if (Node is Argument Argument)
				{
					Arguments.Add(Argument);
					Names.Add(Argument.Name);
				}
			}

			this.arguments = Arguments.ToArray();
			this.argumentNames = Names.ToArray();

			return base.Initialize(Model);
		}

		/// <summary>
		/// Executes a node.
		/// </summary>
		/// <param name="Model">Current model</param>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>Next node of execution, if different from the default, otherwise null (for default).</returns>
		public override async Task<LinkedListNode<IActivityNode>> Execute(Model Model, Variables Variables)
		{
			if (!Variables.TryGetVariable(this.actor, out Variable v))
				throw new Exception("Actor not found: " + this.actor);

			object Actor = v.ValueObject;
			if (Actor is null)
				throw new Exception("Actor is null.");

			Type T = Actor.GetType();
			MethodInfo Method;
			int[] Positions;
			int i, j, c;

			lock (this.synchObj)
			{
				if (T == this.lastType)
				{
					Method = this.lastMethod;
					Positions = this.argumentPositions;
				}
				else
				{
					c = this.argumentNames.Length;
					Positions = new int[c];
					Method = null;

					IEnumerable<MethodInfo> Methods = T.GetRuntimeMethods();

					foreach (MethodInfo MI in Methods)
					{
						if (MI.IsAbstract || !MI.IsPublic)
							continue;

						if (MI.Name != this.action)
							continue;

						ParameterInfo[] Parameters = MI.GetParameters();
						if (Parameters.Length != c)
							continue;

						for (i = 0; i < c; i++)
						{
							ParameterInfo Parameter = Parameters[i];
							j = Array.IndexOf<string>(this.argumentNames, Parameter.Name);
							if (j < 0)
							{
								Parameters = null;
								break;
							}

							Positions[i] = j;
						}

						if (Parameters is null)
							continue;

						Method = MI;
						break;
					}

					if (Method is null)
						throw new Exception("No method named " + this.action + " found on actor " + this.actor + " (of type " + T.FullName + "), with the givet set of arguments.");

					this.lastMethod = Method;
					this.argumentPositions = Positions;
					this.lastType = T;
				}
			}

			c = Positions.Length;

			object[] Arguments = new object[c];

			for (i = 0; i < c; i++)
				Arguments[i] = this.arguments[Positions[i]].Value.Evaluate(Variables);

			object Result = Method.Invoke(Actor, Arguments);

			if (Result is Task Task)
				await Task;

			return null;
		}

		private Type lastType = null;
		private MethodInfo lastMethod = null;
		private int[] argumentPositions = null;
		private readonly object synchObj = new object();

	}
}
