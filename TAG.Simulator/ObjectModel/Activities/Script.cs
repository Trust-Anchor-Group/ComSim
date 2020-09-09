﻿using System;
using System.Threading.Tasks;
using System.Xml;
using Waher.Content;
using Waher.Content.Xml;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Activities
{
	/// <summary>
	/// Executes script in an activity.
	/// </summary>
	public class Script : ActivityNode 
	{
		private string script;
		private Expression expression;

		/// <summary>
		/// Represents a delay in an activity.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		public Script(ISimulationNode Parent)
			: base(Parent)
		{
		}

		/// <summary>
		/// Script string
		/// </summary>
		public string ScriptString => this.script;

		/// <summary>
		/// Parsed expression
		/// </summary>
		public Expression Expression => this.expression;

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "Script";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent)
		{
			return new Script(Parent);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.script = Definition.InnerText;
			this.expression = new Expression(this.script);

			return base.FromXml(Definition);
		}

		/// <summary>
		/// If children are 
		/// </summary>
		public override bool ParseChildren => false;
	}
}