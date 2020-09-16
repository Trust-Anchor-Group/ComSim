using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Waher.Content.Xml;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Activities
{
	/// <summary>
	/// Jumps to another node in the activity.
	/// </summary>
	public class GoTo : ActivityNode 
	{
		private LinkedListNode<IActivityNode> node;
		private string reference;

		/// <summary>
		/// Jumps to another node in the activity.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public GoTo(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Referenced node
		/// </summary>
		public LinkedListNode<IActivityNode> Node => this.node;

		/// <summary>
		/// Reference
		/// </summary>
		public string Reference => this.reference;

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "GoTo";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new GoTo(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.reference = XML.Attribute(Definition, "ref");

			return Task.CompletedTask;
		}

		/// <summary>
		/// Starts the node.
		/// </summary>
		public override Task Start()
		{
			if (!this.Model.TryGetActivityNode(this.reference, out this.node))
				throw new Exception("Activity node not found: " + this.reference);

			return base.Start();
		}

		/// <summary>
		/// Executes a node.
		/// </summary>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>Next node of execution, if different from the default, otherwise null (for default).</returns>
		public override Task<LinkedListNode<IActivityNode>> Execute(Variables Variables)
		{
			return Task.FromResult<LinkedListNode<IActivityNode>>(this.node);
		}

		/// <summary>
		/// Exports PlantUML
		/// </summary>
		/// <param name="Output">Output node</param>
		/// <param name="Indentation">Number of tabs to indent.</param>
		public override void ExportPlantUml(StreamWriter Output, int Indentation)
		{
			Indent(Output, Indentation);
			Output.Write('(');
			Output.Write(this.reference);
			Output.WriteLine(')');

			Indent(Output, Indentation);
			Output.WriteLine("detach");
		}
	}
}
