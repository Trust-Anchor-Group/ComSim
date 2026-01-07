using System.IO;
using TAG.Simulator.ObjectModel.Activities.Execution;
using TAG.Simulator.ObjectModel.Values;

namespace TAG.Simulator.Extensions
{
	/// <summary>
	/// UML-related extensions
	/// </summary>
	public static class UmlExtensions
	{
		/// <summary>
		/// Adds indentation to the current row.
		/// </summary>
		/// <param name="Output">Output.</param>
		/// <param name="Indentation">Number of tabs to indent.</param>
		public static void Indent(this StreamWriter Output, int Indentation)
		{
			if (Indentation > 0)
				Output.Write(new string('\t', Indentation));
		}

		/// <summary>
		/// Appends an argument
		/// </summary>
		/// <param name="Output">Output</param>
		/// <param name="Indentation">Indentation</param>
		/// <param name="Name">Name to append</param>
		/// <param name="Value">Value</param>
		/// <param name="Quotes">Quotes</param>
		/// <param name="QuoteChar">Quote character</param>
		public static void AppendUmlArgument(this StreamWriter Output, int Indentation, string Name, string Value, bool Quotes, char QuoteChar)
		{
			Output.AppendUmlArgument(Indentation, Name);

			if (Quotes)
				Eval.ExportPlantUml("\"" + Value.Replace("\"", "\\\"") + "\"", Output, Indentation, QuoteChar, false);
			else
				Eval.ExportPlantUml(Value, Output, Indentation, QuoteChar, false);
		}

		/// <summary>
		/// Appends an argument
		/// </summary>
		/// <param name="Output">Output</param>
		/// <param name="Indentation">Indentation</param>
		/// <param name="Name">Name to append</param>
		/// <param name="Value">Value</param>
		/// <param name="QuoteChar">Quote character</param>
		public static void AppendUmlArgument(this StreamWriter Output, int Indentation, string Name, IValue Value, char QuoteChar)
		{
			Output.AppendUmlArgument(Indentation, Name);
			Value.ExportPlantUml(Output, Indentation, QuoteChar);
		}

		/// <summary>
		/// Appends an argument
		/// </summary>
		/// <param name="Output">Output</param>
		/// <param name="Indentation">Indentation</param>
		/// <param name="Name">Name to append</param>
		public static void AppendUmlArgument(this StreamWriter Output, int Indentation, string Name)
		{
			Output.WriteLine();
			Output.Indent(Indentation);

			Output.Write(Name);
			Output.Write(": ");
		}
	}
}
