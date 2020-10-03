using System;
using System.Threading.Tasks;

namespace TAG.Simulator.MQ.Tasks
{
	/// <summary>
	/// Task of connecting to an MQ Broker.
	/// </summary>
	internal class ConnectionTask : MqTask
	{
		private readonly TaskCompletionSource<bool> result;
		private readonly string userName;
		private readonly string password;

		/// <summary>
		/// Task of connecting to an MQ Broker.
		/// </summary>
		public ConnectionTask(string UserName, string Password)
			: base()
		{
			this.userName = UserName;
			this.password = Password;
			this.result = new TaskCompletionSource<bool>();
		}

		/// <summary>
		/// Completion task.
		/// </summary>
		public Task Completed => this.result.Task;

		/// <summary>
		/// Performs work defined by the task.
		/// </summary>
		/// <param name="Client">MQ Client</param>
		public override void DoWork(MqClient Client)
		{
			try
			{
				Client.Connect(this.userName, this.password);
				this.result.TrySetResult(true);
			}
			catch (Exception ex)
			{
				this.result.TrySetException(ex);
			}
		}
	}
}
