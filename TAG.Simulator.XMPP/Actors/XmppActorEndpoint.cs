using System;
using System.IO;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel.Actors;
using Waher.Content;
using Waher.Content.Xml;
using Waher.Networking.XMPP;

namespace TAG.Simulator.XMPP.Actors
{
	/// <summary>
	/// Abstract base class for XMPP actors with custom endpoint.
	/// </summary>
	public abstract class XmppActorEndpoint : XmppActor 
	{
		private string endpoint;

		/// <summary>
		/// Abstract base class for XMPP actors with custom endpoint.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public XmppActorEndpoint(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Abstract base class for XMPP actors with custom endpoint.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <param name="InstanceIndex">Instance index.</param>
		/// <param name="InstanceId">ID of instance</param>
		public XmppActorEndpoint(ISimulationNode Parent, Model Model, int InstanceIndex, string InstanceId)
			: base(Parent, Model, InstanceIndex, InstanceId)
		{
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			if (Definition.HasAttribute("endpoint"))
				this.endpoint = XML.Attribute(Definition, "endpoint");
			else
				this.endpoint = null;

			return base.FromXml(Definition);
		}

		/// <summary>
		/// Initialized the node before simulation.
		/// </summary>
		public override async Task Initialize()
		{
			await base.Initialize();

			if (string.IsNullOrEmpty(this.endpoint))
			{
				using (HttpClient HttpClient = new HttpClient(new HttpClientHandler()
				{
					ServerCertificateCustomValidationCallback = this.RemoteCertificateValidationCallback,
					UseCookies = false
				})
				{
					Timeout = TimeSpan.FromMilliseconds(60000)
				})
				{
					try
					{
						HttpResponseMessage Response = await HttpClient.GetAsync("http://" + this.Host + "/.well-known/host-meta");
						Response.EnsureSuccessStatusCode();

						Stream Stream = await Response.Content.ReadAsStreamAsync(); // Regardless of status code, we check for XML content.
						byte[] Bin = await Response.Content.ReadAsByteArrayAsync();
						string CharSet = Response.Content.Headers.ContentType.CharSet;
						Encoding Encoding;

						if (string.IsNullOrEmpty(CharSet))
							Encoding = Encoding.UTF8;
						else
							Encoding = InternetContent.GetEncoding(CharSet);

						string XmlResponse = Encoding.GetString(Bin);
						XmlDocument Doc = new XmlDocument()
						{
							PreserveWhitespace = true
						};
						Doc.LoadXml(XmlResponse);

						if (Doc.DocumentElement != null && Doc.DocumentElement.LocalName == "XRD")
							this.endpoint = this.FindEndpoint(Doc);
					}
					catch (Exception)
					{
						this.endpoint = null;
					}
				}

				if (string.IsNullOrEmpty(this.endpoint))
					throw new Exception("Unable to find endpoint of " + this.Host);
			}
		}

		/// <summary>
		/// Type of XRD link representing endpoint.
		/// </summary>
		protected abstract string EndpointType
		{
			get;
		}

		/// <summary>
		/// Finds the endpoint from the XRD XML definition.
		/// </summary>
		/// <param name="Xrd">XML definition</param>
		protected string FindEndpoint(XmlDocument Xrd)
		{
			string Rel = this.EndpointType;

			foreach (XmlNode N in Xrd.DocumentElement.ChildNodes)
			{
				if (N is XmlElement E && E.LocalName == "Link" && XML.Attribute(E, "rel") == Rel)
					return XML.Attribute(E, "href");
			}

			return null;
		}

		private bool RemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			bool Valid;

			if (sslPolicyErrors == SslPolicyErrors.None)
				Valid = true;
			else
				Valid = this.TrustServer;

			return Valid;
		}

		/// <summary>
		/// Creates an instance of the actor.
		/// 
		/// Note: Parent of newly created actor should point to this node (the creator of the instance object).
		/// </summary>
		/// <param name="InstanceIndex">Instance index.</param>
		/// <param name="InstanceId">ID of instance</param>
		/// <returns>Actor instance.</returns>
		public override Actor CreateInstance(int InstanceIndex, string InstanceId)
		{
			XmppActorEndpoint Result = (XmppActorEndpoint)base.CreateInstance(InstanceIndex, InstanceId);
			Result.endpoint = this.endpoint;
			return Result;
		}

		/// <summary>
		/// Gets XMPP credentials for the instance.
		/// </summary>
		/// <returns>XMPP Credentials</returns>
		protected async override Task<XmppCredentials> GetInstanceCredentials()
		{
			XmppCredentials Result = await base.GetInstanceCredentials();
			Result.UriEndpoint = this.endpoint;
			return Result;
		}
	}
}
