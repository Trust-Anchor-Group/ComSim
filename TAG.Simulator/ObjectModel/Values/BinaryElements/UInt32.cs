using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Values.BinaryElements
{
	/// <summary>
	/// UInt32 value.
	/// </summary>
	public class UInt32 : SimulationNode, IBinaryElement
	{
		private System.UInt32? value;
		private Expression script;

		/// <summary>
		/// UInt32 value.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public UInt32(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of Binary element defining contents of class.
		/// </summary>
		public override string LocalName => "UInt32";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new UInt32(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with Binary definition.
		/// </summary>
		/// <param name="Definition">Binary definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			if (System.UInt32.TryParse(Definition.InnerText, out System.UInt32 Value))
			{
				this.value = Value;
				this.script = null;
			}
			else
			{
				this.value = null;
				this.script = new Expression(Definition.InnerText);
			}

			return Task.CompletedTask;
		}

		/// <summary>
		/// Appends the binary element to the output stream.
		/// </summary>
		/// <param name="Output">Output stream.</param>
		/// <param name="Variables">Set of variables for the activity.</param>
		public async Task Append(MemoryStream Output, Variables Variables)
		{
			object Result;

			Output.Write(BitConverter.GetBytes(this.value ?? 
				((Result = await this.script.EvaluateAsync(Variables)) is System.UInt32 Value ? Value :
				Convert.ToUInt32(Result))), 0, 4);
		}
	}
}
