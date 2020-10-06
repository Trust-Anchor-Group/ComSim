using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel.Activities;
using TAG.Simulator.ObjectModel.Values;
using Waher.Content.Xml;
using Waher.Script;

namespace TAG.Simulator.XMLDSIG.Values
{
	/// <summary>
	/// Signed XML document.
	/// </summary>
	public class Sign : Value, ISimulationNodeChildren
	{
		private IValue value;
		private string rsaKeyName;
		private string rootName;
		private int rsaKeySize;

		/// <summary>
		/// Signed XML document.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public Sign(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "Sign";

		/// <summary>
		/// XML Namespace where the element is defined.
		/// </summary>
		public override string Namespace => "http://trustanchorgroup.com/Schema/ComSim/ComSimXmlDSig.xsd";

		/// <summary>
		/// Points to the embedded XML Schema resource defining the semantics of the XML namespace.
		/// </summary>
		public override string SchemaResource => "TAG.Simulator.XMLDSIG.Schema.ComSimXmlDSig.xsd";

		/// <summary>
		/// Child nodes.
		/// </summary>
		public ISimulationNode[] Children
		{
			get
			{
				if (this.value is null)
					return new ISimulationNode[0];
				else
					return new ISimulationNode[] { this.value };
			}
		}

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new Sign(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override async Task FromXml(XmlElement Definition)
		{
			this.rsaKeyName = XML.Attribute(Definition, "rsaKeyName");
			this.rsaKeySize = XML.Attribute(Definition, "rsaKeySize", 0);

			foreach (XmlNode N in Definition.ChildNodes)
			{
				if (N is XmlElement E)
				{
					if (!(this.value is null))
						throw new Exception("The Sign element can only have one value child element.");

					ISimulationNode Node = await Factory.Create(E, this, this.Model);

					if (Node is IValue Value)
						this.value = Value;
					else
						throw new Exception("The Sign element only accepts a value child element.");
				}
			}
		}

		/// <summary>
		/// Evaluates the value.
		/// </summary>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>Evaluated value.</returns>
		public override object Evaluate(Variables Variables)
		{
			object Result = this.value.Evaluate(Variables);

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
					throw new Exception("Expected XML to sign.");
			}

			if (string.IsNullOrEmpty(this.rootName))
				this.rootName = Doc.DocumentElement.LocalName;

			CspParameters CspParams = new CspParameters()
			{
				KeyContainerName = Expression.Transform(this.rsaKeyName, "{", "}", Variables)
			};

			RSACryptoServiceProvider RsaKey = new RSACryptoServiceProvider(this.rsaKeySize, CspParams);

			SignedXml SignedXml = new SignedXml(Doc)
			{
				SigningKey = RsaKey
			};

			Reference Reference = new Reference()
			{
				Uri = string.Empty
			};

			XmlDsigEnvelopedSignatureTransform Env = new XmlDsigEnvelopedSignatureTransform();
			Reference.AddTransform(Env);

			SignedXml.AddReference(Reference);
			SignedXml.ComputeSignature();

			XmlElement XmlDigitalSignature = SignedXml.GetXml();

			Doc.DocumentElement.AppendChild(Doc.ImportNode(XmlDigitalSignature, true));

			return Doc;
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

			Eval.ExportPlantUml("Sign(" + this.rootName + ")", Output, Indentation, QuoteChar, false);
		}

	}
}
