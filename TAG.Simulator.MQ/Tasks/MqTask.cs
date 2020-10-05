using System;
using System.Threading.Tasks;

namespace TAG.Simulator.MQ.Tasks
{
	/// <summary>
	/// Abstract base class for MQ tasks.
	/// </summary>
	internal abstract class MqTask : IDisposable
	{
		private readonly MqClient client;

		/// <summary>
		/// Abstract base class for MQ tasks.
		/// </summary>
		public MqTask(MqClient Client)
		{
			this.client = Client;
		}

		/// <summary>
		/// MQ Client
		/// </summary>
		public MqClient Client => this.client;

		/// <summary>
		/// <see cref="IDisposable.Dispose"/>
		/// </summary>
		public virtual void Dispose()
		{
		}

		/// <summary>
		/// Performs work defined by the task.
		/// </summary>
		/// <returns>If work should be continued (true), or if it is completed (false).</returns>
		public abstract bool DoWork();
	}
}
