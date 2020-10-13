using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TAG.Simulator.ObjectModel.Actors;
using Waher.Networking.XMPP.Sensor;

namespace TAG.Simulator.XMPP.IoT.Extensions
{
	/// <summary>
	/// Sensor Client XMPP extension
	/// </summary>
	public class SensorClientExtension : IoTXmppExtension
	{
		/// <summary>
		/// Sensor Client XMPP extension
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public SensorClientExtension(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "SensorClientExtension";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new SensorClientExtension(Parent, Model);
		}

		/// <summary>
		/// Adds the extension to the client.
		/// </summary>
		/// <param name="Instance">Actor instance.</param>
		/// <param name="Client">XMPP Client</param>
		public override Task Add(IActor Instance, Waher.Networking.XMPP.XmppClient Client)
		{
			SensorClient Extension = new SensorClient(Client);
			Client.SetTag("SensorClient", Extension);

			return Task.CompletedTask;
		}

	}
}
