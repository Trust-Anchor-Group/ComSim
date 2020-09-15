using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Waher.Content;
using Waher.Content.Xml;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Activities
{
	/// <summary>
	/// Increments a counter.
	/// </summary>
	public class Inc : ActivityNode
	{
		private string counter;

		/// <summary>
		/// Increments a counter.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		public Inc(ISimulationNode Parent)
			: base(Parent)
		{
		}

		/// <summary>
		/// Counter name
		/// </summary>
		public string Counter => this.counter;

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "Inc";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent)
		{
			return new Inc(Parent);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.counter = XML.Attribute(Definition, "counter");

			return Task.CompletedTask;
		}

		/// <summary>
		/// Executes a node.
		/// </summary>
		/// <param name="Model">Current model</param>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>Next node of execution, if different from the default, otherwise null (for default).</returns>
		public override Task<LinkedListNode<IActivityNode>> Execute(Model Model, Variables Variables)
		{
			Model.IncrementCounter(this.counter);
			return Task.FromResult<LinkedListNode<IActivityNode>>(null);
		}

		/// <summary>
		/// Exports PlantUML
		/// </summary>
		/// <param name="Output">Output node</param>
		/// <param name="Indentation">Number of tabs to indent.</param>
		public override void ExportPlantUml(StreamWriter Output, int Indentation)
		{
			Indent(Output, Indentation);
			Output.Write(":Inc(");
			Output.Write(this.counter);
			Output.WriteLine(");");
		}
	}
}
