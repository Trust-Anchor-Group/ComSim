using System;
using System.Threading.Tasks;
using System.Xml;
using Waher.Content.Xml;

namespace TAG.Simulator.ObjectModel.Distributions
{
	/// <summary>
	/// Abstract base class for distributions
	/// </summary>
	public abstract class Distribution : SimulationNode
	{
		private string id;
		private double n;

		/// <summary>
		/// Abstract base class for distributions
		/// </summary>
		/// <param name="Parent">Parent node</param>
		public Distribution(ISimulationNode Parent)
			: base(Parent)
		{
		}

		/// <summary>
		/// ID of distribution.
		/// </summary>
		public string Id => this.id;

		/// <summary>
		/// Intensity/Frequency/Factor
		/// </summary>
		public double N => this.n;

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.id = XML.Attribute(Definition, "id");
			this.n = XML.Attribute(Definition, "N", 0.0);

			return Task.CompletedTask;
		}

		/// <summary>
		/// Initialized the node before simulation.
		/// </summary>
		/// <param name="Model">Model being executed.</param>
		public override Task Initialize(Model Model)
		{
			Model.Register(this);
			return base.Initialize(Model);
		}
	}
}
