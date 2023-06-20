using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TAG.Simulator.ModBus.Activities;
using TAG.Simulator.ObjectModel.Activities;
using Waher.Networking.Modbus;
using Waher.Script;

namespace TAG.Simulator.ModBus.Registers.Activities
{
	/// <summary>
	/// Order of bytes in floating-point value.
	/// </summary>
	public enum FloatByteOrder
	{
		/// <summary>
		/// A B C D
		/// </summary>
		NetworkOrder,

		/// <summary>
		/// B A D C
		/// </summary>
		ByteSwap,

		/// <summary>
		/// C D A B
		/// </summary>
		WordSwap,

		/// <summary>
		/// D C B A
		/// </summary>
		ByteAndWordSwap
	}

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

			ushort[] Words = await Client.ReadMultipleRegisters(Address, Register, 2);
			float Value = ToFloat(FloatByteOrder.NetworkOrder, Words[0], Words[1]);
			string VariableName = await this.responseVariable.GetValueAsync(Variables);

			Variables[VariableName] = Value;

			return null;
		}

		internal static float ToFloat(FloatByteOrder Order, ushort Value1, ushort Value2)
		{
			byte A = (byte)(Value1 >> 8);
			byte B = (byte)Value1;
			byte C = (byte)(Value2 >> 8);
			byte D = (byte)Value2;
			byte[] Bin = new byte[4];

			switch (Order)
			{
				case FloatByteOrder.NetworkOrder:
				default:
					Bin[0] = A;
					Bin[1] = B;
					Bin[2] = C;
					Bin[3] = D;
					break;

				case FloatByteOrder.ByteSwap:
					Bin[0] = B;
					Bin[1] = A;
					Bin[2] = D;
					Bin[3] = C;
					break;

				case FloatByteOrder.WordSwap:
					Bin[0] = C;
					Bin[1] = D;
					Bin[2] = A;
					Bin[3] = B;
					break;

				case FloatByteOrder.ByteAndWordSwap:
					Bin[0] = D;
					Bin[1] = C;
					Bin[2] = B;
					Bin[3] = A;
					break;
			}

			return BitConverter.ToSingle(Bin, 0);
		}

	}
}
