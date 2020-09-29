using System;
using Waher.Networking.MQTT;

namespace TAG.Simulator.MQTT.Actors
{
	/// <summary>
	/// Object used in simulation activities.
	/// </summary>
	public class MqttActivityObject
	{
		/// <summary>
		/// MQTT Client reference
		/// </summary>
		public MqttClient Client;

		/// <summary>
		/// User name used in connection
		/// </summary>
		public string UserName;

		/// <summary>
		/// Instance ID
		/// </summary>
		public string InstanceId;
	}
}
