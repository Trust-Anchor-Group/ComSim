﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel.Actors;
using Waher.Content.Xml;
using Waher.Script;
using Waher.Things.ControlParameters;

namespace TAG.Simulator.XMPP.IoT.Extensions.ControlParameters
{
	/// <summary>
	/// Int32 sensor data control parameter node.
	/// </summary>
	public class Int32ControlParameter : ControlParameterNode
	{
		private int? min;
		private int? max;

		/// <summary>
		/// Int32 sensor data control parameter node.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public Int32ControlParameter(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(Int32ControlParameter);

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new Int32ControlParameter(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			if (Definition.HasAttribute("min"))
				this.min = XML.Attribute(Definition, "min", int.MinValue);
			else
				this.min = null;

			if (Definition.HasAttribute("max"))
				this.max = XML.Attribute(Definition, "max", int.MaxValue);
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
			Parameters.Add(new Waher.Things.ControlParameters.Int32ControlParameter(this.Name, this.Page, this.Label, this.Description, this.min, this.max,
				async (Node) =>
				{
					Variables Variables = this.Model.GetEventVariables(Actor);

					if (!string.IsNullOrEmpty(this.Actor))
						Variables[this.Actor] = Actor;

					object Value = await this.Value.EvaluateAsync(Variables);

					if (!(Value is int TypedValue))
						TypedValue = Convert.ToInt32(Value);

					return TypedValue;
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
