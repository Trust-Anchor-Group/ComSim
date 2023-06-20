using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.Statistics;
using Waher.Content.Xml;

namespace TAG.Simulator.ObjectModel.Measurements
{
	/// <summary>
	/// Abstract base class for series references nodes.
	/// </summary>
	public abstract class SeriesReference : SimulationNode
	{
		private string @for;
		private IBucket bucket;

		/// <summary>
		/// Abstract base class for series references nodes.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public SeriesReference(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// ID of series
		/// </summary>
		public string For => this.@for;

		/// <summary>
		/// Referenced bucket.
		/// </summary>
		public IBucket Bucket => this.bucket;

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.@for = XML.Attribute(Definition, "for");

			return Task.CompletedTask;
		}

		/// <summary>
		/// Starts the node.
		/// </summary>
		public override Task Start()
		{
			this.bucket = this.Model.GetSampleBucket(this.@for);
			return base.Start();
		}
	}
}
