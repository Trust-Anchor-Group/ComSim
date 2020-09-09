﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Values
{
	/// <summary>
	/// XML value.
	/// </summary>
	public class Xml : Value
	{
		private string value;

		/// <summary>
		/// String value.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		public Xml(ISimulationNode Parent)
			: base(Parent)
		{
		}

		/// <summary>
		/// Value
		/// </summary>
		public string Value => this.value;

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "Xml";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent)
		{
			return new Xml(Parent);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.value = Definition.InnerXml;

			return Task.CompletedTask;
		}
	}
}