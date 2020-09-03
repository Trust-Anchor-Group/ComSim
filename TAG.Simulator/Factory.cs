using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using Waher.Runtime.Inventory;

namespace TAG.Simulator
{
	/// <summary>
	/// Factory of simulation objects.
	/// </summary>
	public static class Factory
	{
		private static readonly Dictionary<string, ISimulationNode> nodeTypes = new Dictionary<string, ISimulationNode>();
		private static bool initialized = false;

		/// <summary>
		/// Creates a simulation objected, based on its XML definition.
		/// </summary>
		/// <param name="Definition">XML definition.</param>
		/// <returns>Created simulation object</returns>
		public static async Task<ISimulationNode> Create(XmlElement Definition)
		{
			ISimulationNode Result;
			string Key;

			lock (nodeTypes)
			{
				if (!initialized)
				{
					foreach (Type T in Types.GetTypesImplementingInterface(typeof(ISimulationNode)))
					{
						TypeInfo TI = T.GetTypeInfo();
						if (TI.IsAbstract)
							continue;

						try
						{
							ISimulationNode Node = (ISimulationNode)Activator.CreateInstance(T);
							nodeTypes[Node.Namespace + "#" + Node.LocalName] = Node;
						}
						catch (Exception)
						{
							continue;
						}
					}

					initialized = true;
				}

				Key = Definition.NamespaceURI + "#" + Definition.LocalName;
				if (!nodeTypes.TryGetValue(Key, out Result))
					throw new Exception("Unable to instantiate objects of type " + Definition.NamespaceURI + "#" + Definition.LocalName);
			}

			Result = Result.Create();
			await Result.FromXml(Definition);

			return Result;
		}
	}
}
