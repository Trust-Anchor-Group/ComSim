using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.Extensions;
using TAG.Simulator.ObjectModel;
using TAG.Simulator.ObjectModel.Activities;
using Waher.Content.Xml;
using Waher.Script;

namespace TAG.Simulator.Web.Activities
{
	/// <summary>
	/// Represents an identity property.
	/// </summary>
	/// <param name="Parent">Parent node</param>
	/// <param name="Model">Model in which the node is defined.</param>
	public class Header(ISimulationNode Parent, Model Model)
		: WebNode(Parent, Model)
	{
		private StringAttribute name;
		private StringAttribute value;

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(Header);

		/// <summary>
		/// Name definition of the header.
		/// </summary>
		public string Name => this.name?.Value;

		/// <summary>
		/// Value definition of the header.
		/// </summary>
		public string Value => this.value?.Value;

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new Header(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.name = new StringAttribute(XML.Attribute(Definition, "name"));
			this.value = new StringAttribute(XML.Attribute(Definition, "value"));

			return base.FromXml(Definition);
		}

		/// <summary>
		/// Initialized the node before simulation.
		/// </summary>
		public override Task Initialize()
		{
			if (this.Parent is WebCall WebCall)
				WebCall.Register(this);

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
		public async Task<KeyValuePair<string, string>> Evaluate(Variables Variables)
		{
			string Name = await this.name.GetValueAsync(Variables);
			string Value = await this.value.GetValueAsync(Variables);

			return new KeyValuePair<string, string>(Name, Value);
		}
	}
}
