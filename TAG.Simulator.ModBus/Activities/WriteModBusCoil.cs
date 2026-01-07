using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TAG.Simulator.ModBus.Activities;
using TAG.Simulator.ModBus.Actors;
using TAG.Simulator.ObjectModel.Activities;
using Waher.Script;

namespace TAG.Simulator.ModBus.Registers.Activities
{
	/// <summary>
	/// Writes a coil value.
	/// </summary>
	public class WriteModBusCoil : ModBusWriteOperation
	{
		/// <summary>
		/// Writes a coil value.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public WriteModBusCoil(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(WriteModBusCoil);

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new WriteModBusCoil(Parent, Model);
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

			if (this.value is null)
				throw new Exception("Value not defined.");

			double Value = Expression.ToDouble(await this.value.EvaluateAsync(Variables));
			bool Result;

			await Client.Lock();
			try
			{
				if (!(Client.Client is null))
					Result = await Client.Client.WriteCoil(Address, Register, Value != 0);
				else
					return null;
			}
			finally
			{
				Client.Unlock();
			}

			if (!(this.responseVariable is null))
			{
				string VariableName = await this.responseVariable.GetValueAsync(Variables);
				Variables[VariableName] = Result;
			}

			return null;
		}
	}
}
