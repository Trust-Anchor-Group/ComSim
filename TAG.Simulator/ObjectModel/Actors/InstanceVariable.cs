using System;
using System.Threading.Tasks;
using System.Xml;
using Waher.Content;
using Waher.Content.Xml;
using Waher.Runtime.Settings;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Actors
{
	/// <summary>
	/// Instance variable type.
	/// </summary>
	public enum InstanceVariableType
	{
		Boolean,
		Double,
		Int64,
		String,
		DateTime,
		TimeSpan
	}

	/// <summary>
	/// Defines an instance variable.
	/// </summary>
	public class InstanceVariable : SimulationNode
	{
		private InstanceVariableType type;
		private string name;

		/// <summary>
		/// Defines an instance variable.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public InstanceVariable(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Instance variable name.
		/// </summary>
		public string Name => this.name;

		/// <summary>
		/// Instance variable type.
		/// </summary>
		public InstanceVariableType Type => this.type;

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(InstanceVariable);

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new InstanceVariable(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.name = XML.Attribute(Definition, "name");
			this.type = XML.Attribute(Definition, "type", InstanceVariableType.String);

			return Task.CompletedTask;
		}

		/// <summary>
		/// Sets the instance variable.
		/// </summary>
		/// <param name="Variables">Collection of instance variables.</param>
		public async Task SetVariable(Actor ActorInstance, Variables Variables)
		{
			string KeyPrefix = ActorInstance.InstanceId + "." + this.name;
			string ValueKey = KeyPrefix + ".Value";
			string ConfiguredKey = KeyPrefix + ".Configured";
			object Value;

			if (!await RuntimeSettings.GetAsync(ConfiguredKey, false))
			{
				Value = null;

				while (Value is null)
				{
					string s = await this.Model.GetKey(ValueKey, string.Empty);

					switch (this.type)
					{
						case InstanceVariableType.Boolean:
							if (CommonTypes.TryParse(s, out bool b))
								Value = b;
							break;

						case InstanceVariableType.Double:
							if (CommonTypes.TryParse(s, out double d))
								Value = d;
							break;

						case InstanceVariableType.Int64:
							if (long.TryParse(s, out long l))
								Value = l;
							break;

						case InstanceVariableType.String:
							Value = s;
							break;

						case InstanceVariableType.DateTime:
							if (XML.TryParse(s, out DateTime dt))
								Value = dt;
							else if (DateTime.TryParse(s, out dt))
								Value = dt;
							else if (CommonTypes.TryParseRfc822(s, out DateTimeOffset dto))
								Value = dto.DateTime;
							break;

						case InstanceVariableType.TimeSpan:
							if (TimeSpan.TryParse(s, out TimeSpan ts))
								Value = ts;
							break;

						default:
							throw new Exception("Unrecognized instance variable type: " + this.type.ToString());
					}
				}
			}

			Value = this.type switch
			{
				InstanceVariableType.Boolean => await RuntimeSettings.GetAsync(ValueKey, false),
				InstanceVariableType.Double => await RuntimeSettings.GetAsync(ValueKey, 0.0),
				InstanceVariableType.Int64 => await RuntimeSettings.GetAsync(ValueKey, 0L),
				InstanceVariableType.String => await RuntimeSettings.GetAsync(ValueKey, string.Empty),
				InstanceVariableType.DateTime => await RuntimeSettings.GetAsync(ValueKey, DateTime.MinValue),
				InstanceVariableType.TimeSpan => await RuntimeSettings.GetAsync(ValueKey, TimeSpan.Zero),
				_ => throw new Exception("Unrecognized instance variable type: " + this.type.ToString()),
			};

			Variables[this.name] = Value;
		}
	}
}
