using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using Waher.Content.Xml;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Activities.EventLog
{
	/// <summary>
	/// Represents an identity property.
	/// </summary>
	public class Tag : ActivityNode
	{
		private StringAttribute key;
		private ObjectAttribute value;

		/// <summary>
		/// Represents an identity property.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public Tag(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(Tag);

		/// <summary>
		/// Key definition
		/// </summary>
		public string Key => this.key?.Value;

		/// <summary>
		/// Value definition
		/// </summary>
		public string Value => this.value?.ValueString;

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new Tag(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.key = new StringAttribute(XML.Attribute(Definition, "key"));
			this.value = new ObjectAttribute(XML.Attribute(Definition, "value"));

			return base.FromXml(Definition);
		}

		/// <summary>
		/// Initialized the node before simulation.
		/// </summary>
		public override Task Initialize()
		{
			if (this.Parent is LogActivityNode LogActivityNode)
				LogActivityNode.Register(this);

			return base.Initialize();
		}

		/// <summary>
		/// Executes a node.
		/// </summary>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>Next node of execution, if different from the default, otherwise null (for default).</returns>
		public override Task<LinkedListNode<IActivityNode>> Execute(Variables Variables)
		{
			return Task.FromResult<LinkedListNode<IActivityNode>>(null);
		}

		/// <summary>
		/// Evaluates the property name and value.
		/// </summary>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>Evaluated property.</returns>
		public async Task<KeyValuePair<string, object>> Evaluate(Variables Variables)
		{
			string Key = await this.key.GetValueAsync(Variables);
			object Value = await this.value.GetValueAsync(Variables);

			return new KeyValuePair<string, object>(Key, Value);
		}
	}
}
