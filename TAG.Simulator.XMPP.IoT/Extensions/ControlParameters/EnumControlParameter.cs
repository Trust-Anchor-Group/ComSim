using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel.Actors;
using Waher.Content.Xml;
using Waher.Runtime.Inventory;
using Waher.Script;
using Waher.Things.ControlParameters;

namespace TAG.Simulator.XMPP.IoT.Extensions.ControlParameters
{
	/// <summary>
	/// Enum sensor data control parameter node.
	/// </summary>
	public class EnumControlParameter : ControlParameterNode
	{
		private string enumTypeName;
		private Type enumType;
		private string[] labels;

		/// <summary>
		/// Enum sensor data control parameter node.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public EnumControlParameter(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "EnumControlParameter";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new EnumControlParameter(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override async Task FromXml(XmlElement Definition)
		{
			this.enumTypeName = XML.Attribute(Definition, "enumType");
			this.enumType = Types.GetType(this.enumTypeName);

			if (this.enumType is null)
				throw new Exception("Type not recognized: " + this.enumTypeName);

			TypeInfo TI = this.enumType.GetTypeInfo();
			if (!TI.IsEnum)
				throw new Exception("Type not an enumeration type: " + this.enumTypeName);

			await base.FromXml(Definition);

			List<string> Labels = new List<string>();

			foreach (ISimulationNode Node in this.Children)
			{
				if (Node is Label Label)
					Labels.Add(Label.Value);
			}

			this.labels = Labels.ToArray();
		}

		/// <summary>
		/// Adds control parameters.
		/// </summary>
		/// <param name="Parameters">Generated list of control parameters.</param>
		/// <param name="Actor">Actor instance</param>
		public override void AddParameters(List<ControlParameter> Parameters, IActor Actor)
		{
			async Task<Enum> Get(Waher.Things.IThingReference Node)
			{
				Variables Variables = this.Model.GetEventVariables(Actor);

				if (!string.IsNullOrEmpty(this.Actor))
					Variables[this.Actor] = Actor;

				object Value = await this.Value.EvaluateAsync(Variables);

				if (!(Value is Enum TypedValue))
					TypedValue = (Enum)Enum.Parse(this.enumType, Value.ToString());

				return TypedValue;
			}

			async Task Set(Waher.Things.IThingReference Node, Enum Value)
			{
				DateTime Start = DateTime.Now;
				Variables Variables = this.Model.GetEventVariables(Actor);
				Variables[this.Variable] = Value;

				if (!string.IsNullOrEmpty(this.Actor))
					Variables[this.Actor] = Actor;

				await this.SetEvent.Trigger(Variables);
			}

			if (this.labels.Length == 0)
				Parameters.Add(new Waher.Things.ControlParameters.EnumControlParameter(this.Name, this.Page, this.Label, this.Description, this.enumType, Get, Set));
			else
				Parameters.Add(new Waher.Things.ControlParameters.EnumControlParameter(this.Name, this.Page, this.Label, this.Description, this.enumType, Get, Set, this.labels));
		}

	}
}
