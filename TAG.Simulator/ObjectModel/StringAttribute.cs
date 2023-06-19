using System.Threading.Tasks;
using Waher.Script;

namespace TAG.Simulator.ObjectModel
{
	/// <summary>
	/// Contains the value of a string attribute, possibly with embedded script.
	/// </summary>
	public class StringAttribute
	{
		private readonly Expression expression;
		private readonly string value;
		private readonly bool hasEmbeddedScript;
		private readonly bool isOnlyScript;

		/// <summary>
		/// Contains the value of a string attribute, possibly with embedded script.
		/// </summary>
		/// <param name="Value">String value, from definition</param>
		public StringAttribute(string Value)
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
		}

		/// <summary>
		/// String value, from definition
		/// </summary>
		public string Value => this.value;

		/// <summary>
		/// Gets the value of the attribute.
		/// </summary>
		/// <param name="Variables">Current set of variables.</param>
		/// <returns>Value</returns>
		public async Task<string> GetValueAsync(Variables Variables)
		{
			if (!this.hasEmbeddedScript)
				return this.value;

			if (this.isOnlyScript)
			{
				object Obj = await this.expression.EvaluateAsync(Variables);
				return Obj?.ToString() ?? string.Empty;
			}
			else
				return await Expression.TransformAsync(this.value, "{", "}", Variables);
		}
	}
}
