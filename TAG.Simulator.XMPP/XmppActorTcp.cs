using System;
using System.Threading.Tasks;

namespace TAG.Simulator.XMPP
{
	/// <summary>
	/// XMPP Actor connecting to the XMPP network using traditional TCP (c2s).
	/// </summary>
	public class XmppActorTcp : XmppActor
	{
		/// <summary>
		/// XMPP Actor connecting to the XMPP network using traditional TCP (c2s).
		/// </summary>
		/// <param name="Parent">Parent node</param>
		public XmppActorTcp(ISimulationNode Parent)
			: base(Parent)
		{
		}

		/// <summary>
		/// XMPP Actor connecting to the XMPP network using traditional TCP (c2s).
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="InstanceIndex">Instance index.</param>
		/// <param name="InstanceId">ID of instance</param>
		public XmppActorTcp(ISimulationNode Parent, int InstanceIndex, string InstanceId)
			: base(Parent, InstanceIndex, InstanceId)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "XmppActorTcp";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent)
		{
			return new XmppActorTcp(Parent);
		}

		/// <summary>
		/// Creates an instance object of the XMPP actor, and initializes it.
		/// </summary>
		/// <param name="InstanceIndex">Instance index.</param>
		/// <param name="InstanceId">ID of instance</param>
		/// <returns>Actor instance.</returns>
		protected override XmppActor CreateInstanceObject(int InstanceIndex, string InstanceId)
		{
			return new XmppActorTcp(this.Parent, InstanceIndex, InstanceId);
		}
	}
}
