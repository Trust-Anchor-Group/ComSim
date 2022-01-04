using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel.Actors;
using TAG.Simulator.XMPP.IoT.Extensions.ControlParameters;
using Waher.Networking.XMPP.Provisioning;
using Waher.Networking.XMPP.Control;
using Waher.Things.ControlParameters;

namespace TAG.Simulator.XMPP.IoT.Extensions
{
	/// <summary>
	/// Control Server XMPP extension
	/// </summary>
	public class ControlServerExtension : IoTXmppExtension
	{
		private ControlParameterNode[] parameters;

		/// <summary>
		/// Control Server XMPP extension
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public ControlServerExtension(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "ControlServerExtension";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new ControlServerExtension(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override async Task FromXml(XmlElement Definition)
		{
			await base.FromXml(Definition);

			List<ControlParameterNode> Parameters = new List<ControlParameterNode>();

			foreach (ISimulationNode Node in this.Children)
			{
				if (Node is ControlParameterNode Parameter)
					Parameters.Add(Parameter);
			}

			this.parameters = Parameters.ToArray();
		}

		/// <summary>
		/// Adds the extension to the client.
		/// </summary>
		/// <param name="Instance">Actor instance.</param>
		/// <param name="Client">XMPP Client</param>
		/// <returns>Extension object.</returns>
		public override Task<object> Add(IActor Instance, Waher.Networking.XMPP.XmppClient Client)
		{
			ControlServer Extension;

			if (Client.TryGetTag("ProvisioningClient", out object Obj) && Obj is ProvisioningClient ProvisioningClient)
				Extension = new ControlServer(Client, ProvisioningClient);
			else
				Extension = new ControlServer(Client);

			Client.SetTag("ControlServer", Extension);

			Extension.OnGetControlParameters += (Node) =>
			{
				this.Model.ExternalEvent(Instance, "OnGetControlParameters",
					new KeyValuePair<string, object>("Node", Node),
					new KeyValuePair<string, object>("Client", Client));

				List<ControlParameter> Parameters = new List<ControlParameter>();

				foreach (ControlParameterNode Parameter in this.parameters)
					Parameter.AddParameters(Parameters, Instance);

				return Task.FromResult<ControlParameter[]>(Parameters.ToArray());
			};

			return Task.FromResult<object>(Extension);
		}

	}
}
