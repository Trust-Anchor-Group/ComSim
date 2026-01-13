using System;
using System.Threading.Tasks;
using Waher.Content;
using Waher.Script;

namespace TAG.Simulator.ObjectModel
{
	/// <summary>
	/// Contains the value of a double attribute, possibly with embedded script.
	/// </summary>
	public class DoubleAttribute
	{
		private readonly Expression expression;
		private readonly string name;
		private readonly string value;
		private readonly double parsed;
		private readonly bool hasEmbeddedScript;
		private readonly bool isOnlyScript;

		/// <summary>
		/// Contains the value of a string attribute, possibly with embedded script.
		/// </summary>
		/// <param name="Name">Name of argument</param>
		/// <param name="Value">Double value, from definition</param>
		public DoubleAttribute(string Name, string Value)
		{
			this.name = Name;
			this.value = Value;
			this.hasEmbeddedScript = false;
			this.isOnlyScript = false;
			this.expression = null;

			int i = Value.IndexOf('{');
			if (i >= 0)
			{
				int j = Value.IndexOf('}', i + 1);
				this.hasEmbeddedScript = j > i;
				this.isOnlyScript = this.hasEmbeddedScript && i == 0 && j == Value.Length - 1;
				if (this.isOnlyScript)
					this.expression = new Expression(Value[1..(j - i)]);
			}

			if (!this.hasEmbeddedScript && !CommonTypes.TryParse(Value, out this.parsed))
				throw new Exception("Invalid double value: " + Value);
		}

		/// <summary>
		/// Name of attribute.
		/// </summary>
		public string Name => this.name;

		/// <summary>
		/// Double value, from definition
		/// </summary>
		public double Value => this.parsed;

		/// <summary>
		/// If the attribute is empty.
		/// </summary>
		public bool IsEmpty => string.IsNullOrEmpty(this.value);

		/// <summary>
		/// Gets the value of the attribute.
		/// </summary>
		/// <param name="Variables">Current set of variables.</param>
		/// <returns>Value</returns>
		public async Task<double> GetValueAsync(Variables Variables)
		{
			if (!this.hasEmbeddedScript)
				return this.parsed;

			if (this.isOnlyScript)
			{
				object Obj = await this.expression.EvaluateAsync(Variables);
				return Expression.ToDouble(Obj);
			}
			else
			{
				string s = await Expression.TransformAsync(this.value, "{", "}", Variables);

				if (!CommonTypes.TryParse(s, out double d))
					throw new Exception("Invalid double value: " + s);

				return d;
			}
		}

		/// <summary>
		/// Gets the value of the attribute, as an unsigned 8-bit integer.
		/// </summary>
		/// <param name="Variables">Current set of variables.</param>
		/// <returns>Value</returns>
		public async Task<byte> GetUInt8ValueAsync(Variables Variables)
		{
			double d = await this.GetValueAsync(Variables);
			if (d < byte.MinValue || d > byte.MaxValue)
				throw new ArgumentOutOfRangeException(this.name, d, "Out of byte range.");

			return (byte)d;
		}

		/// <summary>
		/// Gets the value of the attribute, as an unsigned 16-bit integer.
		/// </summary>
		/// <param name="Variables">Current set of variables.</param>
		/// <returns>Value</returns>
		public async Task<ushort> GetUInt16ValueAsync(Variables Variables)
		{
			double d = await this.GetValueAsync(Variables);
			if (d < ushort.MinValue || d > ushort.MaxValue)
				throw new ArgumentOutOfRangeException(this.name, d, "Out of ushort range.");

			return (ushort)d;
		}

		/// <summary>
		/// Gets the value of the attribute, as an unsigned 32-bit integer.
		/// </summary>
		/// <param name="Variables">Current set of variables.</param>
		/// <returns>Value</returns>
		public async Task<uint> GetUInt32ValueAsync(Variables Variables)
		{
			double d = await this.GetValueAsync(Variables);
			if (d < uint.MinValue || d > uint.MaxValue)
				throw new ArgumentOutOfRangeException(this.name, d, "Out of uint range.");

			return (uint)d;
		}

		/// <summary>
		/// Gets the value of the attribute, as an unsigned 64-bit integer.
		/// </summary>
		/// <param name="Variables">Current set of variables.</param>
		/// <returns>Value</returns>
		public async Task<ulong> GetUInt64ValueAsync(Variables Variables)
		{
			double d = await this.GetValueAsync(Variables);
			if (d < ulong.MinValue || d > ulong.MaxValue)
				throw new ArgumentOutOfRangeException(this.name, d, "Out of ulong range.");

			return (ulong)d;
		}

		/// <summary>
		/// Gets the value of the attribute, as a signed 8-bit integer.
		/// </summary>
		/// <param name="Variables">Current set of variables.</param>
		/// <returns>Value</returns>
		public async Task<sbyte> GetInt8ValueAsync(Variables Variables)
		{
			double d = await this.GetValueAsync(Variables);
			if (d < sbyte.MinValue || d > sbyte.MaxValue)
				throw new ArgumentOutOfRangeException(this.name, d, "Out of sbyte range.");

			return (sbyte)d;
		}

		/// <summary>
		/// Gets the value of the attribute, as a signed 16-bit integer.
		/// </summary>
		/// <param name="Variables">Current set of variables.</param>
		/// <returns>Value</returns>
		public async Task<short> GetInt16ValueAsync(Variables Variables)
		{
			double d = await this.GetValueAsync(Variables);
			if (d < short.MinValue || d > short.MaxValue)
				throw new ArgumentOutOfRangeException(this.name, d, "Out of short range.");

			return (short)d;
		}

		/// <summary>
		/// Gets the value of the attribute, as a signed 32-bit integer.
		/// </summary>
		/// <param name="Variables">Current set of variables.</param>
		/// <returns>Value</returns>
		public async Task<int> GetInt32ValueAsync(Variables Variables)
		{
			double d = await this.GetValueAsync(Variables);
			if (d < int.MinValue || d > int.MaxValue)
				throw new ArgumentOutOfRangeException(this.name, d, "Out of int range.");

			return (int)d;
		}

		/// <summary>
		/// Gets the value of the attribute, as a signed 64-bit integer.
		/// </summary>
		/// <param name="Variables">Current set of variables.</param>
		/// <returns>Value</returns>
		public async Task<long> GetInt64ValueAsync(Variables Variables)
		{
			double d = await this.GetValueAsync(Variables);
			if (d < long.MinValue || d > long.MaxValue)
				throw new ArgumentOutOfRangeException(this.name, d, "Out of long range.");

			return (long)d;
		}
	}
}
