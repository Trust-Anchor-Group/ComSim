﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TAG.Simulator.ModBus.Activities;
using TAG.Simulator.ObjectModel.Activities;
using Waher.Script;

namespace TAG.Simulator.ModBus.Registers.Activities
{
	/// <summary>
	/// Reads a discrete input value.
	/// </summary>
	public class ReadModBusDiscreteInput : ModBusReadOperation
	{
		/// <summary>
		/// Reads a discrete input value.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public ReadModBusDiscreteInput(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(ReadModBusDiscreteInput);

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new ReadModBusDiscreteInput(Parent, Model);
		}

		/// <summary>
		/// Executes a node.
		/// </summary>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>Next node of execution, if different from the default, otherwise null (for default).</returns>
		public override Task<LinkedListNode<IActivityNode>> Execute(Variables Variables)
		{
			throw new NotImplementedException();
		}
	}
}
