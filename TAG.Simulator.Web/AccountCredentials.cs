using System;
using Waher.Persistence.Attributes;

namespace TAG.Simulator.Web
{
	/// <summary>
	/// Web Account credentials
	/// </summary>
	[CollectionName("WebCredentials")]
	[Index("Url", "UserName")]
	public class AccountCredentials
	{
		private string objectId = null;
		private string url = string.Empty;
		private string userName = string.Empty;
		private string password = string.Empty;

		/// <summary>
		/// Web Account credentials
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
		/// URL to login page.
		/// </summary>
		public string Url
		{
			get => this.url;
			set => this.url = value;
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
		/// Password
		/// </summary>
		[DefaultValueStringEmpty]
		public string Password
		{
			get => this.password;
			set => this.password = value;
		}
	}
}
