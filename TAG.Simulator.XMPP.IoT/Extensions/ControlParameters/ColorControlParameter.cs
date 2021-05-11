using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using SkiaSharp;
using TAG.Simulator.ObjectModel.Actors;
using Waher.Content;
using Waher.Script;
using Waher.Things.ControlParameters;

namespace TAG.Simulator.XMPP.IoT.Extensions.ControlParameters
{
	/// <summary>
	/// Color sensor data control parameter node.
	/// </summary>
	public class ColorControlParameter : ControlParameterNode
	{
		/// <summary>
		/// Color sensor data control parameter node.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public ColorControlParameter(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "ColorControlParameter";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new ColorControlParameter(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			return base.FromXml(Definition);
		}

		/// <summary>
		/// Adds control parameters.
		/// </summary>
		/// <param name="Parameters">Generated list of control parameters.</param>
		/// <param name="Actor">Actor instance</param>
		public override void AddParameters(List<ControlParameter> Parameters, IActor Actor)
		{
			Parameters.Add(new Waher.Things.ControlParameters.ColorControlParameter(this.Name, this.Page, this.Label, this.Description,
				(Node) =>
				{
					Variables Variables = this.Model.GetEventVariables(Actor);

					if (!string.IsNullOrEmpty(this.Actor))
						Variables[this.Actor] = Actor;

					object Value = this.Value.Evaluate(Variables);

					if (Value is ColorReference Ref)
						return Task.FromResult<ColorReference>(Ref);
					else if (Value is SKColor Color)
						return Task.FromResult<ColorReference>(new ColorReference(Color.Red, Color.Green, Color.Blue, Color.Alpha));
					else
						throw new Exception("Value is not a color.");
				},
				async (Node, Value) =>
				{
					Variables Variables = this.Model.GetEventVariables(Actor);
					Variables[this.Variable] = Value;

					if (!string.IsNullOrEmpty(this.Actor))
						Variables[this.Actor] = Actor;

					await this.SetActivity.ExecuteTask(Variables);
				}));
		}

	}
}
