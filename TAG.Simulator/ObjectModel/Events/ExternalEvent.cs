using System;
using System.Threading.Tasks;
using System.Xml;
using Waher.Content.Xml;

namespace TAG.Simulator.ObjectModel.Events
{
	/// <summary>
	/// External Event
	/// </summary>
	public class ExternalEvent : Event
	{
		/// <summary>
		/// External Event
		/// </summary>
		/// <param name="Parent">Parent node</param>
		public ExternalEvent(ISimulationNode Parent)
			: base(Parent)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "ExternalEvent";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent)
		{
			return new ExternalEvent(Parent);
		}
	}
}
