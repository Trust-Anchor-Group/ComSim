using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TAG.Simulator.Extensions;
using TAG.Simulator.ObjectModel.Activities;
using TAG.Simulator.ObjectModel.Values;
using Waher.Script;

namespace TAG.Simulator.Web.Activities
{
	/// <summary>
	/// Defines the payload of a web method call.
	/// </summary>
	/// <param name="Parent">Parent node</param>
	/// <param name="Model">Model in which the node is defined.</param>
	public class Payload(ISimulationNode Parent, Model Model)
		: WebNode(Parent, Model), IValueRecipient
	{
		private IValue value;

		/// <summary>
		/// Payload value.
		/// </summary>
		public IValue Value => this.value;

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(Payload);

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new Payload(Parent, Model);
		}

		/// <summary>
		/// Registers a value for the argument.
		/// </summary>
		/// <param name="Value">Value node</param>
		public void Register(IValue Value)
		{
			if (this.value is null)
				this.value = Value;
			else
				throw new Exception("Value already registered.");
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
	}
}
