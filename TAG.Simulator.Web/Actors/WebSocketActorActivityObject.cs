namespace TAG.Simulator.Web.Actors
{
	/// <summary>
	/// Object used in simulation activities representing a web-socket actor.
	/// </summary>
	public class WebSocketActorActivityObject
	{
		/// <summary>
		/// WebSocket Actor reference
		/// </summary>
		public WebSocketActor Client;

		/// <summary>
		/// Protocol
		/// </summary>
		public string Protocol;

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
