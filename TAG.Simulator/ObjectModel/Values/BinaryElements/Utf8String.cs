using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Values.BinaryElements
{
	/// <summary>
	/// Utf8String value.
	/// </summary>
	public class Utf8String : SimulationNode, IBinaryElement
	{
		private StringAttribute value;

		/// <summary>
		/// Utf8String value.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public Utf8String(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of Binary element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(Utf8String);

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new Utf8String(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with Binary definition.
		/// </summary>
		/// <param name="Definition">Binary definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.value = new StringAttribute(Definition.InnerText);
			return Task.CompletedTask;
		}

		/// <summary>
		/// Appends the binary element to the output stream.
		/// </summary>
		/// <param name="Output">Output stream.</param>
		/// <param name="Variables">Set of variables for the activity.</param>
		public async Task Append(MemoryStream Output, Variables Variables)
		{
			string s = await this.value.GetValueAsync(Variables);
			byte[] Bin = Encoding.UTF8.GetBytes(s);
			int c = Bin.Length;
			int i = c;
			byte b;

			do
			{
				b = (byte)(i & 127);
				i >>= 7;
				if (i != 0)
					b |= 128;

				Output.WriteByte(b);
			}
			while (i != 0);

			Output.Write(Bin, 0, c);
		}
	}
}
