﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Waher.Events;
using Waher.Runtime.Inventory;

namespace TAG.Simulator
{
	/// <summary>
	/// Factory of simulation objects.
	/// </summary>
	public static class Factory
	{
		private static readonly Dictionary<string, ISimulationNode> nodeTypes = new Dictionary<string, ISimulationNode>();
		private static readonly Dictionary<string, KeyValuePair<string, Assembly>> schemas = new Dictionary<string, KeyValuePair<string, Assembly>>();
		private static bool initialized = false;

		/// <summary>
		/// Initializes the factory
		/// </summary>
		public static void Initialize()
		{
			if (!initialized)
			{
				object[] Arguments = new object[] { null };

				foreach (Type T in Types.GetTypesImplementingInterface(typeof(ISimulationNode)))
				{
					TypeInfo TI = T.GetTypeInfo();
					if (TI.IsAbstract)
						continue;

					try
					{
						ISimulationNode Node = (ISimulationNode)Activator.CreateInstance(T, Arguments);
						nodeTypes[Node.Namespace + "#" + Node.LocalName] = Node;

						string s = Node.Namespace;
						if (!schemas.ContainsKey(s))
							schemas[s] = new KeyValuePair<string, Assembly>(Node.SchemaResource, TI.Assembly);
					}
					catch (Exception ex)
					{
						Log.Critical(ex);
						continue;
					}
				}

				initialized = true;
			}
		}

		/// <summary>
		/// Tries to get the embedded resource name of the schema defining a namespace, and the corresponding assembly.
		/// </summary>
		/// <param name="Namespace">Namespace</param>
		/// <param name="Result">Embedded resource name and assembly.</param>
		/// <returns>If the namespace was found.</returns>
		public static bool TryGetSchemaResource(string Namespace, out KeyValuePair<string, Assembly> Result)
		{
			return schemas.TryGetValue(Namespace, out Result);
		}

		/// <summary>
		/// Creates a simulation objected, based on its XML definition.
		/// </summary>
		/// <param name="Definition">XML definition.</param>
		/// <param name="Parent">Parent node.</param>
		/// <returns>Created simulation object</returns>
		public static async Task<ISimulationNode> Create(XmlElement Definition, ISimulationNode Parent)
		{
			string Key;

			if (!initialized)
				throw new NotSupportedException("Factory not initialized.");

			Key = Definition.NamespaceURI + "#" + Definition.LocalName;
			if (!nodeTypes.TryGetValue(Key, out ISimulationNode Result))
				throw new Exception("Unable to instantiate objects of type " + Definition.NamespaceURI + "#" + Definition.LocalName);

			Result = Result.Create(Parent);
			await Result.FromXml(Definition);

			return Result;
		}
	}
}