using System;
using Waher.Persistence.Attributes;

namespace TAG.Simulator.MQTT
{
	/// <summary>
	/// MQTT Account credentials
	/// </summary>
	[CollectionName("MqttCredentials")]
	[Index("Domain", "UserName")]
	public class AccountCredentials
	{
		private string objectId = null;
		private string domain = string.Empty;
		private string userName = string.Empty;
		private string password = string.Empty;

		/// <summary>
		/// MQTT Account credentials
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
		/// Account user name
		/// </summary>
		public string Domain
		{
			get => this.domain;
			set => this.domain = value;
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
