using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel.Values;
using Waher.Content.Xml;
using Waher.Script;
using Waher.Script.Objects;

namespace TAG.Simulator.ObjectModel.Activities
{
	/// <summary>
	/// Sets a variable value when an event is triggered.
	/// </summary>
	public class Sample : ActivityNode, IValueRecipient
	{
		private IValue value = null;
		private string name;

		/// <summary>
		/// Sets a variable value when an event is triggered.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public Sample(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "Sample";

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
			return new Sample(Parent, Model);
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
				throw new Exception("Sample node already has a value defined.");
		}

		/// <summary>
		/// Executes a node.
		/// </summary>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>Next node of execution, if different from the default, otherwise null (for default).</returns>
		public override Task<LinkedListNode<IActivityNode>> Execute(Variables Variables)
		{
			object Value = this.value.Evaluate(Variables);

			if (Value is double d)
				this.Model.Sample(this.name, d);
			else if (Value is PhysicalQuantity Q)
				this.Model.Sample(this.name, Q);
			else
			{
				d = Convert.ToDouble(Value);
				this.Model.Sample(this.name, d);
			}

			return Task.FromResult<LinkedListNode<IActivityNode>>(null);
		}

		/// <summary>
		/// Exports PlantUML
		/// </summary>
		/// <param name="Output">Output node</param>
		/// <param name="Indentation">Number of tabs to indent.</param>
		/// <param name="QuoteChar">Quote character.</param>
		public override void ExportPlantUml(StreamWriter Output, int Indentation, char QuoteChar)
		{
			Indent(Output, Indentation);
			Output.Write(":Sample(");
			Output.Write(this.name);
			Output.Write(',');
			this.value?.ExportPlantUml(Output, Indentation, QuoteChar);
			Output.WriteLine(");");
		}

	}
}
