using System;
using System.Threading.Tasks;
using Waher.Content;
using Waher.Script;

namespace TAG.Simulator.ObjectModel
{
	/// <summary>
	/// Contains the value of a Boolean attribute, possibly with embedded script.
	/// </summary>
	public class BooleanAttribute
	{
		private readonly Expression expression;
		private readonly string value;
		private readonly bool parsed;
		private readonly bool hasEmbeddedScript;
		private readonly bool isOnlyScript;

		/// <summary>
		/// Contains the value of a Boolean attribute, possibly with embedded script.
		/// </summary>
		/// <param name="Value">Boolean value, from definition</param>
		public BooleanAttribute(string Value)
		{
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
					this.expression = new Expression(Value.Substring(1, j - i - 1));
			}

			if (!this.hasEmbeddedScript && !CommonTypes.TryParse(Value, out this.parsed))
				throw new Exception("Invalid bool value: " + Value);
		}

		/// <summary>
		/// Boolean value, from definition
		/// </summary>
		public string Value => this.value;

		/// <summary>
		/// Gets the value of the attribute.
		/// </summary>
		/// <param name="Variables">Current set of variables.</param>
		/// <returns>Value</returns>
		public async Task<bool> GetValueAsync(Variables Variables)
		{
			if (!this.hasEmbeddedScript)
				return this.parsed;

			if (this.isOnlyScript)
			{
				object Obj = await this.expression.EvaluateAsync(Variables);
				if (Obj is bool b)
					return b;
				else
					throw new Exception("Invalid Boolean value: " + Obj?.ToString());
			}
			else
			{
				string s = await Expression.TransformAsync(this.value, "{", "}", Variables);

				if (!CommonTypes.TryParse(s, out bool d))
					throw new Exception("Invalid bool value: " + s);

				return d;
			}
		}
	}
}
