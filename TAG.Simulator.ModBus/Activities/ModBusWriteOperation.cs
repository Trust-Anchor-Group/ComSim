using System;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel;
using TAG.Simulator.ObjectModel.Values;

namespace TAG.Simulator.ModBus.Activities
{
	/// <summary>
	/// Abstract base class for ModBus write operations.
	/// </summary>
	public abstract class ModBusWriteOperation : ModBusOperation, IValueRecipient
	{
		/// <summary>
		/// Response variable attribute.
		/// </summary>
		protected StringAttribute responseVariable;

		/// <summary>
		/// Value to write.
		/// </summary>
		protected IValue value = null;

		/// <summary>
		/// Abstract base class for ModBus write operations.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public ModBusWriteOperation(ISimulationNode Parent, Model Model)
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

		/// <summary>
		/// Registers a value for the argument.
		/// </summary>
		/// <param name="Value">Value node</param>
		public void Register(IValue Value)
		{
			if (this.value is null)
				this.value = Value;
			else
				throw new Exception("Write node already has a value defined.");
		}
	}
}
