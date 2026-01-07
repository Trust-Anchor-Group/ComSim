using System;
using System.Threading.Tasks;
using Waher.Script;

namespace TAG.Simulator.ObjectModel
{
	/// <summary>
	/// Contains the value of an enumeration attribute, possibly with embedded script.
	/// </summary>
	public class EnumAttribute<T>
		where T : struct
	{
		private readonly Expression expression;
		private readonly T value;
		private readonly string valueString;
		private readonly bool hasEmbeddedScript;
		private readonly bool isOnlyScript;

		/// <summary>
		/// Contains the value of an enumeration attribute, possibly with embedded script.
		/// </summary>
		/// <param name="Value">String value, from definition</param>
		public EnumAttribute(string Value)
			: this(Value, null)
		{
		}

		/// <summary>
		/// Contains the value of an enumeration attribute, possibly with embedded script.
		/// </summary>
		/// <param name="Value">String value, from definition</param>
		/// <param name="DefaultValue">Default value, if attribute value not provided.</param>
		public EnumAttribute(string Value, T? DefaultValue)
		{
			this.valueString = Value;
			this.hasEmbeddedScript = false;
			this.isOnlyScript = false;
			this.expression = null;

			if (DefaultValue.HasValue && string.IsNullOrEmpty(Value))
				this.value = DefaultValue.Value;
			else if (Enum.TryParse(Value, out T Parsed))
				this.value = Parsed;
			else
			{
				int i = Value.IndexOf('{');
				if (i >= 0)
				{
					int j = Value.IndexOf('}', i + 1);
					if (j <= i)
						throw new Exception("Invalid enumeration value: " + Value);

					this.hasEmbeddedScript = true;
					this.isOnlyScript = this.hasEmbeddedScript && i == 0 && j == Value.Length - 1;
					if (this.isOnlyScript)
						this.expression = new Expression(Value.Substring(1, j - i - 1));
				}
				else
					throw new Exception("Invalid enumeration value: " + Value);
			}
		}

		/// <summary>
		/// String value, from definition
		/// </summary>
		public string ValueString => this.valueString;

		/// <summary>
		/// Gets the value of the attribute.
		/// </summary>
		/// <param name="Variables">Current set of variables.</param>
		/// <returns>Value</returns>
		public async Task<T> GetValueAsync(Variables Variables)
		{
			if (!this.hasEmbeddedScript)
				return this.value;

			if (this.isOnlyScript)
			{
				object Obj = await this.expression.EvaluateAsync(Variables);
				if (Obj is T Typed)
					return Typed;
				else if (Obj is string s && Enum.TryParse(s, out Typed))
					return Typed;
				else
					throw new Exception("Invalid enumeration value: " + Obj?.ToString());
			}
			else
			{
				string s = await Expression.TransformAsync(this.valueString, "{", "}", Variables);
				if (Enum.TryParse(s, out T Typed))
					return Typed;
				else
					throw new Exception("Invalid enumeration value: " + s);
			}
		}
	}
}
