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
	/// Represents a delay in an activity.
	/// </summary>
	public class Delay : ActivityNode
	{
		private Duration duration;

		/// <summary>
		/// Represents a delay in an activity.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public Delay(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Duration
		/// </summary>
		public Duration Duration => this.duration;

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "Delay";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new Delay(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.duration = XML.Attribute(Definition, "duration", Duration.Zero);

			return Task.CompletedTask;
		}

		/// <summary>
		/// Executes a node.
		/// </summary>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>Next node of execution, if different from the default, otherwise null (for default).</returns>
		public override async Task<LinkedListNode<IActivityNode>> Execute(Variables Variables)
		{
			DateTime Now = DateTime.Now;
			DateTime TP = Now + this.duration;
			TimeSpan TS = TP - Now;

			if (TS > TimeSpan.Zero)
				await Task.Delay(TS);

			return null;
		}

		/// <summary>
		/// Exports PlantUML
		/// </summary>
		/// <param name="Output">Output node</param>
		/// <param name="Indentation">Number of tabs to indent.</param>
		public override void ExportPlantUml(StreamWriter Output, int Indentation)
		{
			Indent(Output, Indentation);
			Output.Write(":Delay(");

			Values.Duration.ExportText(this.duration, Output);

			Output.WriteLine(");");
		}
	}
}
