using System;
using System.Threading.Tasks;

namespace TAG.Simulator.MQ.Tasks
{
	/// <summary>
	/// Abstract base class for MQ tasks.
	/// </summary>
	internal abstract class MqTask : IDisposable
	{
		/// <summary>
		/// Abstract base class for MQ tasks.
		/// </summary>
		public MqTask()
		{ 
		}

		/// <summary>
		/// <see cref="IDisposable.Dispose"/>
		/// </summary>
		public virtual void Dispose()
		{
		}

		/// <summary>
		/// Performs work defined by the task.
		/// </summary>
		/// <param name="Client">MQ Client</param>
		public abstract void DoWork(MqClient Client);
	}
}
