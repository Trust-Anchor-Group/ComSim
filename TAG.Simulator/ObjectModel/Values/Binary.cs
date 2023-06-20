using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel.Values.BinaryElements;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Values
{
	/// <summary>
	/// Binary value.
	/// </summary>
	public class Binary : Value, ISimulationNodeChildren
	{
		private IBinaryElement[] elements;

		/// <summary>
		/// Binary value.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public Binary(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of Binary element defining contents of class.
		/// </summary>
		public override string LocalName => "Binary";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new Binary(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with Binary definition.
		/// </summary>
		/// <param name="Definition">Binary definition</param>
		public override async Task FromXml(XmlElement Definition)
		{
			List<IBinaryElement> Children = new List<IBinaryElement>();

			foreach (XmlNode N in Definition.ChildNodes)
			{
				if (N is XmlElement E)
				{
					ISimulationNode Node = await Factory.Create(E, this, this.Model);
					if (Node is IBinaryElement BinaryElement)
						Children.Add(BinaryElement);
					else
						throw new Exception(Node.GetType() + " is not a binary element.");
				}
			}

			this.elements = Children.ToArray();
		}

		/// <summary>
		/// Child nodes.
		/// </summary>
		public ISimulationNode[] Children => this.elements;

		/// <summary>
		/// Evaluates <paramref name="Method"/> on each node in the subtree defined by the current node.
		/// </summary>
		/// <param name="Method">Method to call.</param>
		/// <param name="DepthFirst">If children are iterated before parents.</param>
		public override async Task ForEach(ForEachCallbackMethod Method, bool DepthFirst)
		{
			if (!DepthFirst)
				await Method(this);

			if (!(this.elements is null))
			{
				foreach (ISimulationNode Child in this.elements)
					await Child.ForEach(Method, DepthFirst);
			}

			if (DepthFirst)
				await Method(this);
		}

		/// <summary>
		/// Exports Markdown
		/// </summary>
		/// <param name="Output">Output</param>
		public override async Task ExportMarkdown(StreamWriter Output)
		{
			if (!(this.elements is null))
			{
				foreach (ISimulationNode Child in this.elements)
					await Child.ExportMarkdown(Output);
			}
		}

		/// <summary>
		/// Exports XML
		/// </summary>
		/// <param name="Output">Output</param>
		public override async Task ExportXml(XmlWriter Output)
		{
			if (!(this.elements is null))
			{
				foreach (ISimulationNode Child in this.elements)
					await Child.ExportXml(Output);
			}
		}

		/// <summary>
		/// Evaluates the value.
		/// </summary>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>Evaluated value.</returns>
		public override Task<object> EvaluateAsync(Variables Variables)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				foreach (IBinaryElement Element in this.elements)
					Element.Append(ms, Variables);

				return Task.FromResult<object>(ms.ToArray());
			}
		}

		/// <summary>
		/// Exports PlantUML
		/// </summary>
		/// <param name="Output">Output</param>
		/// <param name="Indentation">Number of tabs to indent.</param>
		/// <param name="QuoteChar">Quote character.</param>
		public override void ExportPlantUml(StreamWriter Output, int Indentation, char QuoteChar)
		{
			Activities.Eval.ExportPlantUml("BINARY", Output, Indentation, QuoteChar, false);
		}

		/// <summary>
		/// Copies contents of the node to a new node.
		/// </summary>
		/// <param name="To">Node to receive copied contents.</param>
		public override void CopyContents(ISimulationNode To)
		{
			Binary TypedTo = (Binary)To;

			TypedTo.elements = Copy(this.elements, To, this.Model);
		}
	}
}
