using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel.Actors;
using Waher.Content;
using Waher.Content.Xml;
using Waher.Script;
using Waher.Things.ControlParameters;

namespace TAG.Simulator.XMPP.IoT.Extensions.ControlParameters
{
	/// <summary>
	/// Duration sensor data control parameter node.
	/// </summary>
	public class DurationControlParameter : ControlParameterNode
	{
		private Duration min;
		private Duration max;

		/// <summary>
		/// Duration sensor data control parameter node.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public DurationControlParameter(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "DurationControlParameter";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new DurationControlParameter(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			if (Definition.HasAttribute("min"))
				this.min = XML.Attribute(Definition, "min", Duration.Zero);
			else
				this.min = null;

			if (Definition.HasAttribute("max"))
				this.max = XML.Attribute(Definition, "max", Duration.Zero);
			else
				this.max = null;

			return base.FromXml(Definition);
		}

		/// <summary>
		/// Adds control parameters.
		/// </summary>
		/// <param name="Parameters">Generated list of control parameters.</param>
		/// <param name="Actor">Actor instance</param>
		public override void AddParameters(List<ControlParameter> Parameters, IActor Actor)
		{
			Parameters.Add(new Waher.Things.ControlParameters.DurationControlParameter(this.Name, this.Page, this.Label, this.Description, this.min, this.max,
				(Node) =>
				{
					Variables Variables = this.Model.GetEventVariables(Actor);

					if (!string.IsNullOrEmpty(this.Actor))
						Variables[this.Actor] = Actor;

					object Value = this.Value.Evaluate(Variables);

					if (!(Value is Duration TypedValue))
						TypedValue = Duration.Parse(Value.ToString());

					return Task.FromResult<Duration>(TypedValue);
				},
				async (Node, Value) =>
				{
					DateTime Start = DateTime.Now;
					Variables Variables = this.Model.GetEventVariables(Actor);
					Variables[this.Variable] = Value;

					if (!string.IsNullOrEmpty(this.Actor))
						Variables[this.Actor] = Actor;

					await this.SetEvent.Trigger(Variables);
				}));
		}

	}
}
