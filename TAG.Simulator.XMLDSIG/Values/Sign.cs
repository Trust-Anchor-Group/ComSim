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
		private Xml xml;
		private string rsaKeyName;
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
				if (this.xml is null)
					return new ISimulationNode[0];
				else
					return new ISimulationNode[] { this.xml };
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
					if (!(this.xml is null))
						throw new Exception("Already one XML document available.");

					ISimulationNode Node = await Factory.Create(E, this, this.Model);

					if (Node is Xml Xml)
						this.xml = Xml;
					else
						throw new Exception("The Sign element only accepts Xml child elements.");
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
			object Result = this.xml.Evaluate(Variables);

			if (!(Result is XmlDocument Doc))
				throw new Exception("Expected XML to sign.");

			CspParameters cspParams = new CspParameters()
			{
				KeyContainerName = Expression.Transform(this.rsaKeyName, "{", "}", Variables)
			};

			RSACryptoServiceProvider rsaKey = new RSACryptoServiceProvider(this.rsaKeySize, cspParams);

			SignedXml signedXml = new SignedXml(Doc)
			{
				SigningKey = rsaKey
			};

			Reference reference = new Reference()
			{
				Uri = string.Empty
			};

			XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
			reference.AddTransform(env);

			signedXml.AddReference(reference);
			signedXml.ComputeSignature();

			XmlElement xmlDigitalSignature = signedXml.GetXml();

			Doc.DocumentElement.AppendChild(Doc.ImportNode(xmlDigitalSignature, true));

			return Doc;
		}

		/// <summary>
		/// Exports PlantUML
		/// </summary>
		/// <param name="Output">Output node</param>
		/// <param name="Indentation">Number of tabs to indent.</param>
		/// <param name="QuoteChar">Quote character.</param>
		public override void ExportPlantUml(StreamWriter Output, int Indentation, char QuoteChar)
		{
			Eval.ExportPlantUml("Sign(<" + this.xml.RootName + "...>)", Output, Indentation, QuoteChar, false);
		}

	}
}
