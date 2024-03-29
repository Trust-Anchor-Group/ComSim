﻿using System;
using System.IO;
using System.Threading.Tasks;

namespace TAG.Simulator.ObjectModel.Events
{
	/// <summary>
	/// Container for events.
	/// </summary>
	public class Events : SimulationNodeChildren
	{
		/// <summary>
		/// Container for events.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public Events(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(Events);

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new Events(Parent, Model);
		}

		/// <summary>
		/// Exports Markdown
		/// </summary>
		/// <param name="Output">Output</param>
		public override Task ExportMarkdown(StreamWriter Output)
		{
			return Task.CompletedTask;
		}
	}
}
