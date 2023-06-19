using System;
using System.Threading.Tasks;
using System.Xml;
using Waher.Content;
using Waher.Content.Xml;

namespace TAG.Simulator.ObjectModel.Measurements
{
	/// <summary>
	/// Defines the bucket time of a series.
	/// </summary>
	public class BucketTime : SeriesReference
	{
		private Duration duration;

		/// <summary>
		/// Defines the bucket time of a series.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public BucketTime(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// ID of series
		/// </summary>
		public Duration Duration => this.duration;

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "BucketTime";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new BucketTime(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.duration = XML.Attribute(Definition, "duration", Duration.Zero);

			if (this.duration <= Duration.Zero)
				throw new Exception("Bucket durations must be positive.");

			return base.FromXml(Definition);
		}

		/// <summary>
		/// Starts the node.
		/// </summary>
		public override async Task Start()
		{
			await base.Start();

			this.Bucket.BucketTime = this.duration;
		}

		/// <summary>
		/// Copies contents of the node to a new node.
		/// </summary>
		/// <param name="To">Node to receive copied contents.</param>
		public override void CopyContents(ISimulationNode To)
		{
			BucketTime TypedTo = (BucketTime)To;

			TypedTo.duration = this.duration;

			base.CopyContents(To);
		}

	}
}
