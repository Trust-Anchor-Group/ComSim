﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TAG.Simulator.ModBus.Actors;
using TAG.Simulator.ModBus.Registers;
using TAG.Simulator.ModBus.Registers.Activities;
using TAG.Simulator.ModBus.Registers.Registers;
using TAG.Simulator.ObjectModel.Activities;
using TAG.Simulator.ObjectModel.Values;
using Waher.Networking.Modbus;
using Waher.Script;

namespace TAG.Simulator.ModBus.Activities
{
	/// <summary>
	/// Returns a value during a register readout.
	/// </summary>
	public class ReturnValue : ActivityNode, IValueRecipient
	{
		private IValue value = null;

		/// <summary>
		/// Returns a value during a register readout.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public ReturnValue(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// XML Namespace where the element is defined.
		/// </summary>
		public override string Namespace => ModBusActor.ComSimModBusNamespace;

		/// <summary>
		/// Points to the embedded XML Schema resource defining the semantics of the XML namespace.
		/// </summary>
		public override string SchemaResource => ModBusActor.ComSimModBusSchema;

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(ReturnValue);

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new ReturnValue(Parent, Model);
		}

		/// <summary>
		/// Registers a value for the argument.
		/// </summary>
		/// <param name="Value">Value node</param>
		public void Register(IValue Value)
		{
			if (this.value is null)
				this.value = Value;
			else
				throw new Exception("Sample node already has a value defined.");
		}

		/// <summary>
		/// Executes a node.
		/// </summary>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>Next node of execution, if different from the default, otherwise null (for default).</returns>
		public override async Task<LinkedListNode<IActivityNode>> Execute(Variables Variables)
		{
			if (!Variables.TryGetVariable("e", out Waher.Script.Variable v))
				throw new Exception("Event arguments not found.");

			object Obj = v.ValueObject;
			object Value = await this.value.EvaluateAsync(Variables);

			if (Obj is ReadBitsEventArgs ReadBitsEventArgs)
			{
				ushort RegisterNr = GetRegisterNr(Variables);
				ReadBitsEventArgs[RegisterNr] = Expression.ToDouble(Value) != 0;
			}
			else if (Obj is WriteBitEventArgs WriteBitEventArgs)
			{
				WriteBitEventArgs.Value = Expression.ToDouble(Value) != 0;
			}
			else if (Obj is ReadWordsEventArgs ReadWordsEventArgs)
			{
				ModBusRegister Register = GetRegister(Variables);
				ushort RegisterNr = GetRegisterNr(Variables);
				double FloatValue = Expression.ToDouble(Value);

				if (Register is ModBusHoldingFloatingPointRegister ||
					Register is ModBusInputFloatingPointRegister)
				{
					ReadModBusHoldingFloatingPointRegister.FromFloat(FloatByteOrder.NetworkOrder,
						(float)FloatValue, out ushort Value1, out ushort Value2);

					if (RegisterNr >= ReadWordsEventArgs.ReferenceNr &&
						RegisterNr < ReadWordsEventArgs.ReferenceNr + ReadWordsEventArgs.NrWords)
					{
						ReadWordsEventArgs[RegisterNr] = Value1;
					}

					RegisterNr++;

					if (RegisterNr >= ReadWordsEventArgs.ReferenceNr &&
						RegisterNr < ReadWordsEventArgs.ReferenceNr + ReadWordsEventArgs.NrWords)
					{
						ReadWordsEventArgs[RegisterNr] = Value2;
					}
				}
				else
					ReadWordsEventArgs[RegisterNr] = (ushort)FloatValue;
			}
			else if (Obj is WriteWordEventArgs WriteWordEventArgs)
			{
				ModBusRegister Register = GetRegister(Variables);

				if (Register is ModBusHoldingFloatingPointRegister ||
					Register is ModBusInputFloatingPointRegister)
				{
					throw new NotSupportedException("Returning values in floating-point set operations not supported.");
				}
				else
					WriteWordEventArgs.Value = (ushort)Expression.ToDouble(Value);
			}
			else
				throw new Exception("Unrecognized event arguments type: " + Obj.GetType().FullName);

			return null;
		}

		private static ushort GetRegisterNr(Variables Variables)
		{
			if (!Variables.TryGetVariable("RegisterNr", out Waher.Script.Variable v))
				throw new Exception("RegisterNr variable not found.");

			return (ushort)Expression.ToDouble(v.ValueObject);
		}

		private static ModBusRegister GetRegister(Variables Variables)
		{
			if (!Variables.TryGetVariable("Register", out Waher.Script.Variable v))
				throw new Exception("Register variable not found.");

			if (v.ValueObject is ModBusRegister Register)
				return Register;
			else
				throw new Exception("Register variable does not contain a ModBusRegister object reference.");
		}
	}
}
