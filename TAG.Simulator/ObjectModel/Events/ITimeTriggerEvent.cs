namespace TAG.Simulator.ObjectModel.Events
{
	/// <summary>
	/// Interface for events that can be triggered by elapsed time.
	/// </summary>
	public interface ITimeTriggerEvent : IEvent
	{
		/// <summary>
		/// Check if event is triggered during a time period.
		/// </summary>
		/// <param name="t1">Starting time of period.</param>
		/// <param name="t2">Ending time of period.</param>
		/// <param name="NrCycles">Number of time cycles completed.</param>
		void CheckTrigger(double t1, double t2, int NrCycles);
	}
}
