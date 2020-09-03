using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;

namespace TAG.Simulator.ObjectModel
{
	/// <summary>
	/// Abstract base class for simulation nodes with children
	/// </summary>
	public abstract class SimulationNodeChildren : SimulationNode
	{
		private ISimulationNode[] children;

		/// <summary>
		/// Abstract base class for simulation nodes
		/// </summary>
		public SimulationNodeChildren()
			: base()
		{
		}

		/// <summary>
		/// Child nodes.
		/// </summary>
		public ISimulationNode[] Children => this.children;

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override async Task FromXml(XmlElement Definition)
		{
			List<ISimulationNode> Children = new List<ISimulationNode>();

			foreach (XmlNode N in Definition.ChildNodes)
			{
				if (N is XmlElement E)
					Children.Add(await Factory.Create(E));
			}

			this.children = Children.ToArray();
		}
	}
}
