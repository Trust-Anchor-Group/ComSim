using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Waher.Content.Xml;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Values
{
	/// <summary>
	/// Duration value.
	/// </summary>
	public class Duration : Value
	{
		private Waher.Content.Duration value;

		/// <summary>
		/// Duration value.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public Duration(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Value
		/// </summary>
		public Waher.Content.Duration Value => this.value;

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "Duration";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new Duration(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.value = XML.Attribute(Definition, "value", Waher.Content.Duration.Zero);

			return Task.CompletedTask;
		}

		/// <summary>
		/// Evaluates the value.
		/// </summary>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>Evaluated value.</returns>
		public override object Evaluate(Variables Variables)
		{
			return this.value;
		}

		/// <summary>
		/// Exports PlantUML
		/// </summary>
		/// <param name="Output">Output node</param>
		/// <param name="Indentation">Number of tabs to indent.</param>
		public override void ExportPlantUml(StreamWriter Output, int Indentation)
		{
			ExportText(this.value, Output);
		}

		/// <summary>
		/// Exports PlantUML
		/// </summary>
		/// <param name="Duration">Duration value.</param>
		/// <param name="Output">Output node</param>
		public static void ExportText(Waher.Content.Duration Duration, StreamWriter Output)
		{
			StringBuilder sb = new StringBuilder();
			ExportText(Duration, sb);
			Output.Write(sb.ToString());
		}

		/// <summary>
		/// Exports PlantUML
		/// </summary>
		/// <param name="Duration">Duration value.</param>
		/// <param name="Output">Output node</param>
		public static void ExportText(Waher.Content.Duration Duration, StringBuilder Output)
		{
			if (Duration.Negation)
				Output.Append("-(");

			bool First = true;

			Append(Output, Duration.Years, "years", ref First);
			Append(Output, Duration.Months, "months", ref First);
			Append(Output, Duration.Days, "d", ref First);
			Append(Output, Duration.Hours, "h", ref First);
			Append(Output, Duration.Minutes, "min", ref First);

			int s = (int)Duration.Seconds;
			Append(Output, s, "s", ref First);

			int ms = (int)((Duration.Seconds - s) * 1000);
			Append(Output, ms, "ms", ref First);

			if (Duration.Negation)
				Output.Append(')');
		}

		private static void Append(StringBuilder Output, int Nr, string Unit, ref bool First)
		{
			if (Nr != 0)
			{
				if (First)
					First = false;
				else
					Output.Append(", ");

				Output.Append(Nr.ToString());
				Output.Append(' ');
				Output.Append(Unit);
			}
		}
	}
}
