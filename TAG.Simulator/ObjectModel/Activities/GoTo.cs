using System;
using System.Threading.Tasks;
using System.Xml;
using Waher.Content;
using Waher.Content.Xml;

namespace TAG.Simulator.ObjectModel.Activities
{
	/// <summary>
	/// Jumps to another node in the activity.
	/// </summary>
	public class GoTo : ActivityNode 
	{
		private Model model;
		private ActivityNode node;
		private string reference;

		/// <summary>
		/// Jumps to another node in the activity.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		public GoTo(ISimulationNode Parent)
			: base(Parent)
		{
		}

		/// <summary>
		/// Referenced node
		/// </summary>
		public ActivityNode Node => this.node;

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
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent)
		{
			return new GoTo(Parent);
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
		/// Initialized the node before simulation.
		/// </summary>
		/// <param name="Model">Model being executed.</param>
		public override Task Initialize(Model Model)
		{
			this.model = Model;

			return base.Initialize(Model);
		}

		/// <summary>
		/// Starts the node.
		/// </summary>
		public override Task Start()
		{
			if (!this.model.TryGetActivityNode(this.reference, out this.node))
				throw new Exception("Activity node not found: " + this.reference);

			return base.Start();
		}
	}
}
