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
		/// <param name="Parent">Parent node</param>
		public SimulationNodeChildren(ISimulationNode Parent)
			: base(Parent)
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
					Children.Add(await Factory.Create(E, this));
			}

			this.children = Children.ToArray();
		}

		/// <summary>
		/// Evaluates <paramref name="Method"/> on each node in the subtree defined by the current node.
		/// </summary>
		/// <param name="Method">Method to call.</param>
		/// <param name="DepthFirst">If children are iterated before parents.</param>
		public override async Task ForEach(ForEachCallbackMethod Method, bool DepthFirst)
		{
			if (!DepthFirst)
				await Method(this);

			if (!(this.children is null))
			{
				foreach (ISimulationNode Child in this.children)
					await Child.ForEach(Method, DepthFirst);
			}

			if (DepthFirst)
				await Method(this);
		}

	}
}
