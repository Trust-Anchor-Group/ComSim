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
		/// <param name="Client">MQ Client</param>
		/// <param name="UserName">User Name</param>
		/// <param name="Password">Password</param>
		public ConnectionTask(MqClient Client, string UserName, string Password)
			: base(Client)
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
		/// <returns>If work should be continued (true), or if it is completed (false).</returns>
		public override Task<bool> DoWork()
		{
			try
			{
				this.Client.Connect(this.userName, this.password);
				this.result.TrySetResult(true);
			}
			catch (Exception ex)
			{
				this.result.TrySetException(ex);
			}

			return Task.FromResult(false);
		}
	}
}
