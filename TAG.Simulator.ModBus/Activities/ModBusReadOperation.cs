using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel;

namespace TAG.Simulator.ModBus.Activities
{
	/// <summary>
	/// Abstract base class for ModBus read operations.
	/// </summary>
	public abstract class ModBusReadOperation : ModBusOperation 
	{
		/// <summary>
		/// Response variable attribute.
		/// </summary>
		protected StringAttribute responseVariable;

		/// <summary>
		/// Abstract base class for ModBus read operations.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public ModBusReadOperation(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			foreach (XmlAttribute Attr in Definition.Attributes)
			{
				switch (Attr.Name)
				{
					case "responseVariable":
						this.responseVariable = new StringAttribute(Attr.Value);
						break;
				}
			}

			return base.FromXml(Definition);
		}
	}
}
