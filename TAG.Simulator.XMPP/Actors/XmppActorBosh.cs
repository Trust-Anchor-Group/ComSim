﻿using System;

namespace TAG.Simulator.XMPP.Actors
{
	/// <summary>
	/// XMPP Actor connecting to the XMPP network using BOSH (HTTP).
	/// </summary>
	public class XmppActorBosh : XmppActorEndpoint
	{
		/// <summary>
		/// XMPP Actor connecting to the XMPP network using BOSH (HTTP).
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public XmppActorBosh(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// XMPP Actor connecting to the XMPP network using BOSH (HTTP).
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <param name="InstanceIndex">Instance index.</param>
		/// <param name="InstanceId">ID of instance</param>
		public XmppActorBosh(ISimulationNode Parent, Model Model, int InstanceIndex, string InstanceId)
			: base(Parent, Model, InstanceIndex, InstanceId)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(XmppActorBosh);

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new XmppActorBosh(Parent, Model);
		}

		/// <summary>
		/// Creates an instance object of the XMPP actor, and initializes it.
		/// 
		/// Note: Parent of newly created actor should point to this node (the creator of the instance object).
		/// </summary>
		/// <param name="InstanceIndex">Instance index.</param>
		/// <param name="InstanceId">ID of instance</param>
		/// <returns>Actor instance.</returns>
		protected override XmppActor CreateInstanceObject(int InstanceIndex, string InstanceId)
		{
			return new XmppActorBosh(this, this.Model, InstanceIndex, InstanceId);
		}

		/// <summary>
		/// Type of XRD link representing endpoint.
		/// </summary>
		protected override string EndpointType => "urn:xmpp:alt-connections:xbosh";
	}
}
