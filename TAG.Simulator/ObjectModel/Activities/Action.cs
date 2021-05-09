using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using Waher.Content;
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
		private StringAttribute actor;
		private StringAttribute action;

		/// <summary>
		/// Represents an action on an actor.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public Action(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "Action";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new Action(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			foreach (XmlAttribute Attribute in Definition.Attributes)
			{
				switch (Attribute.Name)
				{
					case "actor":
						this.actor = new StringAttribute(Attribute.Value);
						break;

					case "action":
						this.action = new StringAttribute(Attribute.Value);
						break;

					case "id":
					case "xmlns":
						break;

					default:
						if (Attribute.Prefix == "xmlns")
							break;

						string s = Attribute.Value;

						if (s.StartsWith("{") && s.EndsWith("}"))
						{
							this.AddChild(new Argument(this, this.Model, Attribute.Name,
								new Expression(s.Substring(1, s.Length - 2).Trim())));
						}
						else if (CommonTypes.TryParse(s, out double d))
							this.AddChild(new Argument(this, this.Model, Attribute.Name, d));
						else if (XML.TryParse(s, out System.DateTime TP))
							this.AddChild(new Argument(this, this.Model, Attribute.Name, TP));
						else if (Waher.Content.Duration.TryParse(s, out Waher.Content.Duration Duration))
							this.AddChild(new Argument(this, this.Model, Attribute.Name, Duration));
						else if (TimeSpan.TryParse(s, out TimeSpan TS))
							this.AddChild(new Argument(this, this.Model, Attribute.Name, TS));
						else
							this.AddChild(new Argument(this, this.Model, Attribute.Name, s));
						break;
				}
			}

			return base.FromXml(Definition);
		}

		/// <summary>
		/// Initialized the node before simulation.
		/// </summary>
		public override Task Initialize()
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

			return base.Initialize();
		}

		/// <summary>
		/// Executes a node.
		/// </summary>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>Next node of execution, if different from the default, otherwise null (for default).</returns>
		public override async Task<LinkedListNode<IActivityNode>> Execute(Variables Variables)
		{
			string ActorValue = this.actor.GetValue(Variables);
			string ActionValue = this.action.GetValue(Variables);
			object Actor = this.GetActorObject(ActorValue, Variables);
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

						if (MI.Name != ActionValue)
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

		/// <summary>
		/// Exports PlantUML
		/// </summary>
		/// <param name="Output">Output</param>
		/// <param name="Indentation">Number of tabs to indent.</param>
		/// <param name="QuoteChar">Quote character.</param>
		public override void ExportPlantUml(StreamWriter Output, int Indentation, char QuoteChar)
		{
			Indent(Output, Indentation);
			Output.Write(':');
			Output.Write(this.actor.Value);
			Output.Write('.');
			Output.Write(this.action.Value);
			Output.Write("(");

			Indentation++;

			foreach (Argument Arg in this.arguments)
			{
				Output.WriteLine();
				Indent(Output, Indentation);

				Output.Write(Arg.Name);
				Output.Write(": ");

				Arg.Value.ExportPlantUml(Output, Indentation, QuoteChar);
			}

			Output.WriteLine(");");
		}

	}
}
