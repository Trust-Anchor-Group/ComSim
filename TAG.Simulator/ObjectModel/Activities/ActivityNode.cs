using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Waher.Content.Xml;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Activities
{
	/// <summary>
	/// Abstract base class for activity nodes
	/// </summary>
	public abstract class ActivityNode : SimulationNodeChildren, IActivityNode
	{
		private LinkedList<IActivityNode> activityNodes = null;
		private int count;
		private string id;

		/// <summary>
		/// Abstract base class for activity nodes
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public ActivityNode(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// ID of activity node.
		/// </summary>
		public string Id => this.id;

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.id = XML.Attribute(Definition, "id");

			return base.FromXml(Definition);
		}

		/// <summary>
		/// Initialized the node before simulation.
		/// </summary>
		public override Task Initialize()
		{
			if (this.Parent is IActivity Activity)
				Activity.Register(this);
			else if (this.Parent is IActivityNode ActivityNode)
				ActivityNode.Register(this);

			return base.Initialize();
		}

		/// <summary>
		/// Registers a child activity node.
		/// </summary>
		/// <param name="Node">Activity node.</param>
		public void Register(IActivityNode Node)
		{
			if (this.activityNodes is null)
				this.activityNodes = new LinkedList<IActivityNode>();

			this.Model.Register(this.activityNodes.AddLast(Node));
			this.count++;
		}

		/// <summary>
		/// First child activity node.
		/// </summary>
		protected LinkedListNode<IActivityNode> FirstNode => this.activityNodes?.First;

		/// <summary>
		/// Number of registered activity nodes.
		/// </summary>
		protected int Count => this.count;

		/// <summary>
		/// Executes a node.
		/// </summary>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>Next node of execution, if different from the default, otherwise null (for default).</returns>
		public abstract Task<LinkedListNode<IActivityNode>> Execute(Variables Variables);

		/// <summary>
		/// Exports PlantUML
		/// </summary>
		/// <param name="Output">Output</param>
		/// <param name="Indentation">Number of tabs to indent.</param>
		/// <param name="QuoteChar">Quote character.</param>
		public virtual void ExportPlantUml(StreamWriter Output, int Indentation, char QuoteChar)
		{
			if (!(this.activityNodes is null))
			{
				foreach (IActivityNode Node in this.activityNodes)
					Node.ExportPlantUml(Output, Indentation, QuoteChar);
			}
		}
	}
}
