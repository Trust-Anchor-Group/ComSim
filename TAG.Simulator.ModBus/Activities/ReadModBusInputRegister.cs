using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TAG.Simulator.ModBus.Activities;
using TAG.Simulator.ModBus.Actors;
using TAG.Simulator.ObjectModel.Activities;
using Waher.Networking.Modbus;
using Waher.Script;

namespace TAG.Simulator.ModBus.Registers.Activities
{
	/// <summary>
	/// Reads an input register value.
	/// </summary>
	public class ReadModBusInputRegister : ModBusReadOperation
	{
		/// <summary>
		/// Reads an input register value.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public ReadModBusInputRegister(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(ReadModBusInputRegister);

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new ReadModBusInputRegister(Parent, Model);
		}

		/// <summary>
		/// Executes a node.
		/// </summary>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>Next node of execution, if different from the default, otherwise null (for default).</returns>
		public override async Task<LinkedListNode<IActivityNode>> Execute(Variables Variables)
		{
			ModBusClient Client = await this.GetClient(Variables);
			byte Address = await this.address.GetUInt8ValueAsync(Variables);
			ushort Register = await this.register.GetUInt16ValueAsync(Variables);
			string VariableName = await this.responseVariable.GetValueAsync(Variables);

			await Client.Lock();
			try
			{
				ushort[] Words = await Client.Client.ReadInputRegisters(Address, Register, 1);
				Variables[VariableName] = Words[0];
			}
			finally
			{
				Client.Unlock();
			}

			return null;
		}
	}
}
