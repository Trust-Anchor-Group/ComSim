using System;

namespace TAG.Simulator.MQ.Actors
{
	/// <summary>
	/// Object used in simulation activities.
	/// </summary>
	public class MqActivityObject
	{
		/// <summary>
		/// MQ Client reference
		/// </summary>
		public MqClient Client;

		/// <summary>
		/// User name used in connection
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
