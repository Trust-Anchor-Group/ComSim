using System;
using System.Threading.Tasks;
using System.Xml;
using Waher.Content;
using Waher.Content.Xml;

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
	}
}
