using System.Threading.Tasks;
using Waher.Script;

namespace TAG.Simulator.ObjectModel
{
	/// <summary>
	/// Contains the value of an attribute, possibly with embedded script.
	/// </summary>
	public class ObjectAttribute
	{
		private readonly Expression expression;
		private readonly object value;
		private readonly string valueString;
		private readonly bool hasEmbeddedScript;
		private readonly bool isOnlyScript;

		/// <summary>
		/// Contains the value of an attribute, possibly with embedded script.
		/// </summary>
		/// <param name="Value">String value, from definition</param>
		public ObjectAttribute(string Value)
		{
			this.value = this.valueString = Value;
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
		}

		/// <summary>
		/// String value, from definition
		/// </summary>
		public string ValueString => this.valueString;

		/// <summary>
		/// If the attribute is empty.
		/// </summary>
		public bool IsEmpty => string.IsNullOrEmpty(this.valueString);

		/// <summary>
		/// Gets the value of the attribute.
		/// </summary>
		/// <param name="Variables">Current set of variables.</param>
		/// <returns>Value</returns>
		public async Task<object> GetValueAsync(Variables Variables)
		{
			if (!this.hasEmbeddedScript)
				return this.value;

			if (this.isOnlyScript)
				return await this.expression.EvaluateAsync(Variables);
			else
				return await Expression.TransformAsync(this.valueString, "{", "}", Variables);
		}
	}
}
