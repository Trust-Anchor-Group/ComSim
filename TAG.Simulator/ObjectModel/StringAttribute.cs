using System;
using System.Collections.Generic;
using System.Text;
using Waher.Script;

namespace TAG.Simulator.ObjectModel
{
	/// <summary>
	/// Contains the value of a string attribute, possibly with embedded script.
	/// </summary>
	public class StringAttribute
	{
		private readonly string value;
		private readonly bool hasEmbeddedScript;

		/// <summary>
		/// Contains the value of a string attribute, possibly with embedded script.
		/// </summary>
		/// <param name="Value">String value, from definition</param>
		public StringAttribute(string Value)
		{
			this.value = Value;
			this.hasEmbeddedScript = false;

			int i = Value.IndexOf('{');
			if (i >= 0)
			{
				int j = Value.IndexOf('}', i + 1);
				this.hasEmbeddedScript = j > i;
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
		public string GetValue(Variables Variables)
		{
			if (this.hasEmbeddedScript)
				return Expression.Transform(this.value, "{", "}", Variables);
			else
				return this.value;
		}
	}
}
