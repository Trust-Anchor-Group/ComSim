using System;

namespace TAG.Simulator.Events
{
	/// <summary>
	/// Delegate for thread count event handlers.
	/// </summary>
	/// <param name="Sender">Sender of event.</param>
	/// <param name="e">Event arguments.</param>
	public delegate void ThreadCountEventHandler(object Sender, ThreadCountEventArgs e);

	/// <summary>
	/// Event arguments for thread count events.
	/// </summary>
	public class ThreadCountEventArgs : EventArgs
	{
		private int? count;

		/// <summary>
		/// Event arguments for thread count events.
		/// </summary>
		public ThreadCountEventArgs()
		{
		}

		/// <summary>
		/// Number of threads of the current process.
		/// </summary>
		public int? Count
		{
			get => this.count;
			set => this.count = value;
		}
	}
}
