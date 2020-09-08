using System;
using Waher.Persistence.Attributes;

namespace TAG.Simulator.XMPP
{
	/// <summary>
	/// XMPP Account credentials
	/// </summary>
	[CollectionName("XmppCredentials")]
	[Index("Domain", "UserName")]
	public class AccountCredentials
	{
		private string objectId = null;
		private string domain = string.Empty;
		private string userName = string.Empty;
		private string passwordHash = string.Empty;
		private string passwordHashMethod = string.Empty;

		/// <summary>
		/// XMPP Account credentials
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
		public string PasswordHash
		{
			get => this.passwordHash;
			set => this.passwordHash = value;
		}

		/// <summary>
		/// Password hash method
		/// </summary>
		public string PasswordHashMethod
		{
			get => this.passwordHashMethod;
			set => this.passwordHashMethod = value;
		}
	}
}
