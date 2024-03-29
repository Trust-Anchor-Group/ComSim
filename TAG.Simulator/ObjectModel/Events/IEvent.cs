﻿using System;
using System.IO;
using System.Threading.Tasks;
using TAG.Simulator.ObjectModel.Distributions;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Events
{
	/// <summary>
	/// Interface for events
	/// </summary>
	public interface IEvent : ISimulationNode
	{
		/// <summary>
		/// ID of event.
		/// </summary>
		string Id
		{
			get;
		}

		/// <summary>
		/// Associated distribution, null if none.
		/// </summary>
		IDistribution Distribution
		{
			get;
		}

		/// <summary>
		/// Event description
		/// </summary>
		string Description
		{
			get;
		}

		/// <summary>
		/// Triggers the event.
		/// </summary>
		/// <param name="Variables">Event variables</param>
		/// <param name="Guard">Optional guard expression.</param>
		/// <param name="GuardLimit">Maximum number of times to apply guard expression in search of suitable candidates.</param>
		Task Trigger(Variables Variables, Expression Guard = null, int GuardLimit = int.MaxValue);

		/// <summary>
		/// Registers an event preparation node.
		/// </summary>
		/// <param name="PreparationNode">Preparation node.</param>
		void Register(IEventPreparation PreparationNode);

		/// <summary>
		/// Registers an external event.
		/// </summary>
		/// <param name="ExternalEvent">External event.</param>
		void Register(IExternalEvent ExternalEvent);

		/// <summary>
		/// Exports use case information
		/// </summary>
		/// <param name="Output">Output</param>
		/// <param name="Index">Chart Index</param>
		void ExportUseCaseData(StreamWriter Output, int Index);

		/// <summary>
		/// Gets a <see cref="Task"/> object, that will be completed when the event is triggered.
		/// </summary>
		/// <returns>Trigger task object.</returns>
		Task GetTrigger();
	}
}
