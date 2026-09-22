using System;
using TAG.Simulator.ObjectModel.Actors;
using Waher.Networking.XMPP;

namespace TAG.Simulator.XMPP.Actors
{
	/// <summary>
	/// Object used in simulation activities.
	/// </summary>
	public class XmppActivityObject : ActivityObject
	{
		/// <summary>
		/// Object used in simulation activities.
		/// </summary>
		public XmppActivityObject(Actor ActorInstance)
			: base(ActorInstance)
		{
		}

		/// <summary>
		/// XMPP Client reference
		/// </summary>
		public XmppClient Client;

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

		/// <summary>
		/// Access to extension objects.
		/// </summary>
		/// <param name="Index">Extension name</param>
		/// <returns>Extension object, if found.</returns>
		/// <exception cref="Exception">If no extension with the given name was found.</exception>
		public override object this[string Index]
		{
			get
			{
				if (this.Client.TryGetTag(Index, out object Obj))
					return Obj;
				else
					return base[Index];
			}
		}
	}
}
