using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TAG.Simulator.ObjectModel.Actors;
using Waher.Networking.XMPP;

namespace TAG.Simulator.XMPP.Extensions
{
	/// <summary>
	/// Interface for XMPP extensions
	/// </summary>
	public interface IXmppExtension : ISimulationNode
	{
		/// <summary>
		/// Adds the extension to the client.
		/// </summary>
		/// <param name="Instance">Actor instance.</param>
		/// <param name="Client">XMPP Client</param>
		Task Add(IActor Instance, XmppClient Client);
	}
}
