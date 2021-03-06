﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Activities
{
	/// <summary>
	/// Interface for activity nodes
	/// </summary>
	public interface IActivityNode : ISimulationNode
	{
		/// <summary>
		/// ID of activity node.
		/// </summary>
		string Id
		{
			get;
		}

		/// <summary>
		/// Registers a child activity node.
		/// </summary>
		/// <param name="Node">Activity node.</param>
		void Register(IActivityNode Node);

		/// <summary>
		/// Executes a node.
		/// </summary>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>Next node of execution, if different from the default, otherwise null (for default).</returns>
		Task<LinkedListNode<IActivityNode>> Execute(Variables Variables);

		/// <summary>
		/// Exports PlantUML
		/// </summary>
		/// <param name="Output">Output</param>
		/// <param name="Indentation">Number of tabs to indent.</param>
		/// <param name="QuoteChar">Quote character.</param>
		void ExportPlantUml(StreamWriter Output, int Indentation, char QuoteChar);
	}
}
