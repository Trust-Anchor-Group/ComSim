using System;

namespace TAG.Simulator.Events
{
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
