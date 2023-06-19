using System.Collections.Generic;
using System.Threading.Tasks;
using TAG.Simulator.ModBus.Activities;
using TAG.Simulator.ObjectModel.Activities;
using Waher.Networking.Modbus;
using Waher.Script;

namespace TAG.Simulator.ModBus.Registers.Activities
{
	/// <summary>
	/// Reads a holding floating-point register value.
	/// </summary>
	public class ReadModBusHoldingFloatingPointRegister : ModBusReadOperation
	{
		/// <summary>
		/// Reads a holding floating-point register value.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public ReadModBusHoldingFloatingPointRegister(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(ReadModBusHoldingFloatingPointRegister);

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new ReadModBusHoldingFloatingPointRegister(Parent, Model);
		}

		/// <summary>
		/// Executes a node.
		/// </summary>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>Next node of execution, if different from the default, otherwise null (for default).</returns>
		public override async Task<LinkedListNode<IActivityNode>> Execute(Variables Variables)
		{
			ModbusTcpClient Client = await this.GetClient(Variables);
			byte Address = await this.address.GetUInt8ValueAsync(Variables);
			ushort Register = await this.register.GetUInt16ValueAsync(Variables);

			ushort[] Words = await Client.ReadMultipleRegisters(Address, Register, 1);
			string VariableName = await this.responseVariable.GetValueAsync(Variables);

			Variables[VariableName] = Words[0];

			return null;
		}
	}
}
