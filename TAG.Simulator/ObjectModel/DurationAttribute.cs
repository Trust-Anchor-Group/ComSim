using System;
using System.Threading.Tasks;
using Waher.Content;
using Waher.Script;

namespace TAG.Simulator.ObjectModel
{
	/// <summary>
	/// Contains the value of a Duration attribute, possibly with embedded script.
	/// </summary>
	public class DurationAttribute
	{
		private readonly Expression expression;
		private readonly string value;
		private readonly Duration parsed;
		private readonly bool hasEmbeddedScript;
		private readonly bool isOnlyScript;

		/// <summary>
		/// Contains the value of a string attribute, possibly with embedded script.
		/// </summary>
		/// <param name="Value">Duration value, from definition</param>
		public DurationAttribute(string Value)
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
					this.expression = new Expression(Value[1..(j - i)]);
			}

			if (!this.hasEmbeddedScript && !Duration.TryParse(Value, out this.parsed))
				throw new Exception("Invalid Duration value: " + Value);
		}

		/// <summary>
		/// Duration value, from definition
		/// </summary>
		public Duration Value => this.parsed;

		/// <summary>
		/// If the attribute is empty.
		/// </summary>
		public bool IsEmpty => string.IsNullOrEmpty(this.value);

		/// <summary>
		/// Gets the value of the attribute.
		/// </summary>
		/// <param name="Variables">Current set of variables.</param>
		/// <returns>Value</returns>
		public async Task<Duration> GetValueAsync(Variables Variables)
		{
			if (!this.hasEmbeddedScript)
				return this.parsed;

			if (this.isOnlyScript)
			{
				object Obj = await this.expression.EvaluateAsync(Variables);

				if (Obj is Duration D)
					return D;
				else
					throw new Exception("Invalid Duration value: " + Obj?.ToString());
			}
			else
			{
				string s = await Expression.TransformAsync(this.value, "{", "}", Variables);

				if (!Duration.TryParse(s, out Duration D))
					throw new Exception("Invalid Duration value: " + s);

				return D;
			}
		}
	}
}
