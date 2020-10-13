using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TAG.Simulator.ObjectModel.Actors;
using Waher.Networking.XMPP.Control;

namespace TAG.Simulator.XMPP.IoT.Extensions
{
	/// <summary>
	/// Control Client XMPP extension
	/// </summary>
	public class ControlClientExtension : IoTXmppExtension
	{
		/// <summary>
		/// Control Client XMPP extension
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public ControlClientExtension(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "ControlClientExtension";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new ControlClientExtension(Parent, Model);
		}

		/// <summary>
		/// Adds the extension to the client.
		/// </summary>
		/// <param name="Instance">Actor instance.</param>
		/// <param name="Client">XMPP Client</param>
		public override Task Add(IActor Instance, Waher.Networking.XMPP.XmppClient Client)
		{
			ControlClient Extension = new ControlClient(Client);
			Client.SetTag("ControlClient", Extension);

			return Task.CompletedTask;
		}

	}
}
