using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Values.BinaryElements
{
	/// <summary>
	/// Single value.
	/// </summary>
	public class Single : SimulationNode, IBinaryElement
	{
		private System.Single? value;
		private Expression script;

		/// <summary>
		/// Single value.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public Single(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of Binary element defining contents of class.
		/// </summary>
		public override string LocalName => "Single";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new Single(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with Binary definition.
		/// </summary>
		/// <param name="Definition">Binary definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			if (System.Single.TryParse(Definition.InnerText, out System.Single Value))
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
				((Result = await this.script.EvaluateAsync(Variables)) is System.Single Value ? Value :
				Convert.ToSingle(Result))), 0, 4);
		}

	}
}
