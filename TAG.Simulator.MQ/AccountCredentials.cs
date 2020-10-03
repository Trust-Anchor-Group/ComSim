using System;
using Waher.Persistence.Attributes;

namespace TAG.Simulator.MQ
{
	/// <summary>
	/// MQ Account credentials
	/// </summary>
	[CollectionName("MqttCredentials")]
	[Index("Host", "UserName")]
	public class AccountCredentials
	{
		private string objectId = null;
		private string host = string.Empty;
		private string userName = string.Empty;
		private string password = string.Empty;

		/// <summary>
		/// MQ Account credentials
		/// </summary>
		public AccountCredentials()
		{
		}

		/// <summary>
		/// Object ID
		/// </summary>
		[ObjectId]
		public string ObjectId
		{
			get => this.objectId;
			set => this.objectId = value;
		}

		/// <summary>
		/// Broker host name
		/// </summary>
		public string Host
		{
			get => this.host;
			set => this.host = value;
		}

		/// <summary>
		/// Account user name
		/// </summary>
		public string UserName
		{
			get => this.userName;
			set => this.userName = value;
		}

		/// <summary>
		/// Password hash
		/// </summary>
		[DefaultValueStringEmpty]
		public string Password
		{
			get => this.password;
			set => this.password = value;
		}
	}
}
