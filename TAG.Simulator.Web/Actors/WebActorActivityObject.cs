namespace TAG.Simulator.Web.Actors
{
	/// <summary>
	/// Object used in simulation activities representing a web actor.
	/// </summary>
	public class WebActorActivityObject
	{
		/// <summary>
		/// Web Client reference
		/// </summary>
		public CookieWebClient Client;

		/// <summary>
		/// User name used for authentication
		/// </summary>
		public string UserName;

		/// <summary>
		/// Instance ID
		/// </summary>
		public string InstanceId;

		/// <summary>
		/// Instance Index
		/// </summary>
		public int InstanceIndex;
	}
}
