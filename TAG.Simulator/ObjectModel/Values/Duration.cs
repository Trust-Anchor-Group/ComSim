using System;
using System.IO;
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
		public Duration(ISimulationNode Parent)
			: base(Parent)
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
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent)
		{
			return new Duration(Parent);
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
			ExportPlantUml(this.value, Output);
		}

		/// <summary>
		/// Exports PlantUML
		/// </summary>
		/// <param name="Duration">Duration value.</param>
		/// <param name="Output">Output node</param>
		public static void ExportPlantUml(Waher.Content.Duration Duration, StreamWriter Output)
		{
			if (Duration.Negation)
				Output.Write("-(");

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
				Output.Write(')');
		}

		private static void Append(StreamWriter Output, int Nr, string Unit, ref bool First)
		{
			if (Nr != 0)
			{
				if (First)
					First = false;
				else
					Output.Write(", ");

				Output.Write(Nr.ToString());
				Output.Write(' ');
				Output.Write(Unit);
			}
		}
	}
}
