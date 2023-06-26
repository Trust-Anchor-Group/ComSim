using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel;
using TAG.Simulator.ObjectModel.Activities;
using TAG.Simulator.ObjectModel.Values;
using Waher.Content.Xml;
using Waher.Script;

namespace TAG.Simulator.XMLDSIG.Activities
{
	/// <summary>
	/// Verifies a Signed XML document.
	/// </summary>
	public class Verify : ActivityNode, IValueRecipient
	{
		private IValue value;
		private StringAttribute rsaKeyName;
		private string rootName;

		/// <summary>
		/// Verifies a Signed XML document.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public Verify(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(Verify);

		/// <summary>
		/// XML Namespace where the element is defined.
		/// </summary>
		public override string Namespace => "http://lab.tagroot.io/Schema/ComSim/XmlDSig.xsd";

		/// <summary>
		/// Points to the embedded XML Schema resource defining the semantics of the XML namespace.
		/// </summary>
		public override string SchemaResource => "TAG.Simulator.XMLDSIG.Schema.ComSimXmlDSig.xsd";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new Verify(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.rsaKeyName = new StringAttribute(XML.Attribute(Definition, "rsaKeyName"));

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
				throw new Exception("The Verify element can only have one value child element.");
		}

		/// <summary>
		/// Executes a node.
		/// </summary>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>Next node of execution, if different from the default, otherwise null (for default).</returns>
		public override async Task<LinkedListNode<IActivityNode>> Execute(Variables Variables)
		{
			object Result = await this.value.EvaluateAsync(Variables);

			if (!(Result is XmlDocument Doc))
			{
				if (Result is XmlElement E)
				{
					Doc = new XmlDocument()
					{
						PreserveWhitespace = true
					};

					Doc.LoadXml(E.OuterXml);
				}
				else if (Result is string s)
				{
					Doc = new XmlDocument()
					{
						PreserveWhitespace = true
					};

					Doc.LoadXml(s);
				}
				else
					throw new Exception("Expected XML to verify.");
			}

			if (string.IsNullOrEmpty(this.rootName))
				this.rootName = Doc.DocumentElement.LocalName;

			SignedXml SignedXml = new SignedXml(Doc);
			XmlNodeList SignatureElements = Doc.GetElementsByTagName("Signature");
			if (SignatureElements.Count == 0)
				throw new Exception("XML not signed.");

			SignedXml.LoadXml((XmlElement)SignatureElements[0]);

			CspParameters CspParams = new CspParameters()
			{
				KeyContainerName = await this.rsaKeyName.GetValueAsync(Variables),
				Flags = CspProviderFlags.UseExistingKey
			};

			RSACryptoServiceProvider RsaKey = new RSACryptoServiceProvider(CspParams);

			if (!SignedXml.CheckSignature(RsaKey))
				throw new Exception("XML signature invalid.");

			return null;
		}

		/// <summary>
		/// Exports PlantUML
		/// </summary>
		/// <param name="Output">Output</param>
		/// <param name="Indentation">Number of tabs to indent.</param>
		/// <param name="QuoteChar">Quote character.</param>
		public override void ExportPlantUml(StreamWriter Output, int Indentation, char QuoteChar)
		{
			if (string.IsNullOrEmpty(this.rootName))
			{
				if (this.value is Xml Xml)
					this.rootName = Xml.RootName;
				else
					this.rootName = this.value?.LocalName;
			}

			Indent(Output, Indentation);
			Output.Write(":Verify(");
			Output.Write(this.rootName);
			Output.WriteLine(");");
		}
	}
}
