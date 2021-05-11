using System;
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
	/// String sensor data control parameter node.
	/// </summary>
	public class StringControlParameter : ControlParameterNode
	{
		private string regularExpression;
		private string[] options;
		private string[] labels;

		/// <summary>
		/// String sensor data control parameter node.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public StringControlParameter(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "StringControlParameter";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new StringControlParameter(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override async Task FromXml(XmlElement Definition)
		{
			this.regularExpression = XML.Attribute(Definition, "regularExpression");

			await base.FromXml(Definition);

			List<string> Options = new List<string>();
			List<string> Labels = new List<string>();

			foreach (ISimulationNode Node in this.Children)
			{
				if (Node is Option Option)
				{
					Options.Add(Option.Value);
					Labels.Add(Option.Label);
				}
			}

			this.options = Options.ToArray();
			this.labels = Labels.ToArray();
		}

		/// <summary>
		/// Adds control parameters.
		/// </summary>
		/// <param name="Parameters">Generated list of control parameters.</param>
		/// <param name="Actor">Actor instance</param>
		public override void AddParameters(List<ControlParameter> Parameters, IActor Actor)
		{
			Task<string> Get(Waher.Things.IThingReference Node)
			{
				Variables Variables = this.Model.GetEventVariables(Actor);

				if (!string.IsNullOrEmpty(this.Actor))
					Variables[this.Actor] = Actor;

				object Value = this.Value.Evaluate(Variables);

				if (!(Value is string TypedValue))
					TypedValue = Value.ToString();

				return Task.FromResult<string>(TypedValue);
			}

			async Task Set(Waher.Things.IThingReference Node, string Value)
			{
				DateTime Start = DateTime.Now;
				Variables Variables = this.Model.GetEventVariables(Actor);
				Variables[this.Variable] = Value;

				if (!string.IsNullOrEmpty(this.Actor))
					Variables[this.Actor] = Actor;

				await this.SetEvent.Trigger(Variables);
			}

			if (!string.IsNullOrEmpty(this.regularExpression))
				Parameters.Add(new Waher.Things.ControlParameters.StringControlParameter(this.Name, this.Page, this.Label, this.Description, this.regularExpression, Get, Set));
			else if (this.options.Length > 0)
				Parameters.Add(new Waher.Things.ControlParameters.StringControlParameter(this.Name, this.Page, this.Label, this.Description, this.options, this.labels, Get, Set));
			else
				Parameters.Add(new Waher.Things.ControlParameters.StringControlParameter(this.Name, this.Page, this.Label, this.Description, Get, Set));
		}

	}
}
