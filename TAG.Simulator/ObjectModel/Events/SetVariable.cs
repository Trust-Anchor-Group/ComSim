﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel.Values;
using Waher.Content.Xml;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Events
{
	/// <summary>
	/// Sets a variable value when an event is triggered.
	/// </summary>
	public class SetVariable : EventPreparation, IValueRecipient
	{
		private IValue value = null;
		private string name;

		/// <summary>
		/// Sets a variable value when an event is triggered.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public SetVariable(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(SetVariable);

		/// <summary>
		/// Name of variable within the scope of the event.
		/// </summary>
		public string Name => this.name;

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new SetVariable(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.name = XML.Attribute(Definition, "name");

			return base.FromXml(Definition);
		}

		/// <summary>
		/// Registers a value for the argument.
		/// </summary>
		/// <param name="Value">Value node</param>
		public void Register(IValue Value)
		{
			if (this.value is null)
				this.value = Value;
			else
				throw new Exception("SetVariable node already has a value defined.");
		}

		/// <summary>
		/// Prepares <paramref name="Variables"/> for the execution of an event.
		/// </summary>
		/// <param name="Variables">Event variables</param>
		/// <param name="Tags">Extensible list of meta-data tags related to the event.</param>
		public override async Task Prepare(Variables Variables, List<KeyValuePair<string, object>> Tags)
		{
			Variables[this.name] = this.value is null ? null : await this.value.EvaluateAsync(Variables);
		}

		/// <summary>
		/// Releases resources at the end of an event.
		/// </summary>
		/// <param name="Variables">Event variables</param>
		public override void Release(Variables Variables)
		{
			Variables.Remove(this.name);
		}

		/// <summary>
		/// Exports the node to PlantUML script in a markdown document.
		/// </summary>
		/// <param name="Output">Output stream.</param>
		/// <param name="Name">Optional name for the association.</param>
		/// <param name="Index">Chart Index</param>
		public override void ExportPlantUml(StreamWriter Output, string Name, int Index)
		{
			string s = Index.ToString();

			Output.Write("note \"");
			Output.Write(this.name);
			Output.Write('=');
			this.value?.ExportPlantUml(Output, 0, '\'');
			Output.Write("\" as N");
			Output.Write(this.name);
			Output.WriteLine(s);

			Output.Write("UC");
			Output.Write(s);
			Output.Write(" .. N");
			Output.Write(this.name);
			Output.WriteLine(s);
		}
	}
}
