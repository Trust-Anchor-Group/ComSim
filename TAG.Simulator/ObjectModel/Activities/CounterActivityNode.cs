using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Waher.Content;
using Waher.Content.Xml;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Activities
{
	/// <summary>
	/// Abstract base class for counter activity nodes.
	/// </summary>
	public abstract class CounterActivityNode : ActivityNode
	{
		private string counter;

		/// <summary>
		/// Abstract base class for counter activity nodes.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public CounterActivityNode(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Counter name
		/// </summary>
		public string Counter => this.counter;

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.counter = XML.Attribute(Definition, "counter");

			return Task.CompletedTask;
		}

	}
}
