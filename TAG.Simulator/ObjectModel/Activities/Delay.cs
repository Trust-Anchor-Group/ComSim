using System;
using System.Collections.Generic;
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
		public Delay(ISimulationNode Parent)
			: base(Parent)
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
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent)
		{
			return new Delay(Parent);
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
		/// <param name="Model">Current model</param>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>Next node of execution, if different from the default, otherwise null (for default).</returns>
		public override async Task<LinkedListNode<IActivityNode>> Execute(Model Model, Variables Variables)
		{
			DateTime Now = DateTime.Now;
			DateTime TP = Now + this.duration;
			TimeSpan TS = TP - Now;

			if (TS > TimeSpan.Zero)
				await Task.Delay(TS);

			return null;
		}
	}
}
