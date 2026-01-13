using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using Waher.Content;
using Waher.Networking;
using Waher.Networking.Sniffers;

namespace TAG.Simulator.Web.Actors
{
	/// <summary>
	/// HTTP Client with cookie support.
	/// </summary>
	public class CookieWebClient : CommunicationLayer, IDisposable
	{
		private readonly Version protocolVersion;
		private readonly CookieContainer cookies = new();
		private readonly NetworkCredential credentials = null;
		private readonly ISniffer[] sniffers;

		/// <summary>
		/// HTTP Client with cookie support.
		/// </summary>
		/// <param name="ProtocolVersion">Protocol version</param>
		/// <param name="Sniffers">Sniffers</param>
		public CookieWebClient(Version ProtocolVersion, params ISniffer[] Sniffers)
			: this(ProtocolVersion, null, null, Sniffers)
		{
		}

		/// <summary>
		/// HTTP Client with cookie support.
		/// </summary>
		/// <param name="ProtocolVersion">Protocol version</param>
		/// <param name="UserName">User name</param>
		/// <param name="Password">Password</param>
		public CookieWebClient(Version ProtocolVersion, string UserName, string Password,
			params ISniffer[] Sniffers)
			: base(true, Sniffers)
		{
			this.protocolVersion = ProtocolVersion;
			this.sniffers = Sniffers;

			if (!string.IsNullOrEmpty(UserName))
				this.credentials = new NetworkCredential(UserName, Password);
			else
				this.credentials = null;
		}

		/// <summary>
		/// Disposes the client.
		/// </summary>
		public void Dispose()
		{
		}

		/// <summary>
		/// Contains cookies for the session.
		/// </summary>
		public CookieContainer Cookies
		{
			get => this.cookies;
		}

		/// <summary>
		/// Performs a HEAD operation.
		/// </summary>
		/// <param name="Url">URL</param>
		/// <param name="Headers">HTTP Headers</param>
		/// <returns>Response</returns>
		public Task<byte[]> HEAD(string Url,
			params KeyValuePair<string, string>[] Headers)
		{
			return this.Send(Url, HttpMethod.Head, Headers);
		}

		/// <summary>
		/// Performs a GET operation.
		/// </summary>
		/// <param name="Url">URL</param>
		/// <param name="Headers">HTTP Headers</param>
		/// <returns>Response</returns>
		public Task<byte[]> GET(string Url,
			params KeyValuePair<string, string>[] Headers)
		{
			return this.Send(Url, HttpMethod.Get, Headers);
		}

		/// <summary>
		/// Performs a POST operation.
		/// </summary>
		/// <param name="Url">URL</param>
		/// <param name="Data">Payload</param>
		/// <param name="Headers">HTTP Headers</param>
		/// <returns>Response</returns>
		public Task<byte[]> POST(string Url, byte[] Data,
			params KeyValuePair<string, string>[] Headers)
		{
			return this.Send(Url, HttpMethod.Post, Data, Headers);
		}

		/// <summary>
		/// Performs a PUT operation.
		/// </summary>
		/// <param name="Url">URL</param>
		/// <param name="Data">Payload</param>
		/// <param name="Headers">HTTP Headers</param>
		/// <returns>Response</returns>
		public Task<byte[]> PUT(string Url, byte[] Data,
			params KeyValuePair<string, string>[] Headers)
		{
			return this.Send(Url, HttpMethod.Put, Data, Headers);
		}

		/// <summary>
		/// Performs a DELETE operation.
		/// </summary>
		/// <param name="Url">URL</param>
		/// <param name="Headers">HTTP Headers</param>
		/// <returns>Response</returns>
		public Task<byte[]> DELETE(string Url,
			params KeyValuePair<string, string>[] Headers)
		{
			return this.Send(Url, HttpMethod.Delete, Headers);
		}

		/// <summary>
		/// Performs a OPTIONS operation.
		/// </summary>
		/// <param name="Url">URL</param>
		/// <param name="Headers">HTTP Headers</param>
		/// <returns>Response</returns>
		public Task<byte[]> OPTIONS(string Url,
			params KeyValuePair<string, string>[] Headers)
		{
			return this.Send(Url, HttpMethod.Options, Headers);
		}

		/// <summary>
		/// Performs a TRACE operation.
		/// </summary>
		/// <param name="Url">URL</param>
		/// <param name="Headers">HTTP Headers</param>
		/// <returns>Response</returns>
		public Task<byte[]> TRACE(string Url,
			params KeyValuePair<string, string>[] Headers)
		{
			return this.Send(Url, HttpMethod.Trace, Headers);
		}

		/// <summary>
		/// Performs a PATCH operation.
		/// </summary>
		/// <param name="Url">URL</param>
		/// <param name="Data">Payload</param>
		/// <param name="Headers">HTTP Headers</param>
		/// <returns>Response</returns>
		public Task<byte[]> PATCH(string Url, byte[] Data,
			params KeyValuePair<string, string>[] Headers)
		{
			return this.Send(Url, HttpMethod.Patch, Data, Headers);
		}

		/// <summary>
		/// Performs an HTTP method with no payload.
		/// </summary>
		/// <param name="Url">URL</param>
		/// <param name="Method">HTTP Method</param>
		/// <param name="Headers">HTTP Headers</param>
		/// <returns>Response</returns>
		private Task<byte[]> Send(string Url, HttpMethod Method,
			params KeyValuePair<string, string>[] Headers)
		{
			return this.Send(Url, Method, null, Headers);
		}

		/// <summary>
		/// Performs an HTTP method with payload.
		/// </summary>
		/// <param name="Url">URL</param>
		/// <param name="Method">HTTP Method</param>
		/// <param name="Data">Binary payload.</param>
		/// <param name="Headers">HTTP Headers</param>
		/// <returns>Response</returns>
		private async Task<byte[]> Send(string Url, HttpMethod Method, byte[] Data,
			params KeyValuePair<string, string>[] Headers)
		{
			using HttpClient Client = this.GetClient();
			HttpRequestMessage Request = this.GetRequest(Method, Url, Headers);

			if (Data is not null)
				Request.Content = new ByteArrayContent(Data);

			if (this.HasSniffers)
			{
				StringBuilder sb = new();

				sb.Append(Method.ToString().ToUpper());
				sb.Append(' ');
				sb.Append(Url);
				sb.Append(" HTTP/");
				sb.Append(this.protocolVersion.ToString());
				sb.AppendLine();

				foreach (KeyValuePair<string, string> Header in Headers)
				{
					sb.Append(Header.Key);
					sb.Append(": ");
					sb.AppendLine(Header.Value);
				}

				this.TransmitText(sb.ToString());

				if (Data is not null)
					this.TransmitBinary(false, Data);
			}

			HttpResponseMessage Response = await Client.SendAsync(Request,
				HttpCompletionOption.ResponseHeadersRead);

			if (this.HasSniffers)
			{
				StringBuilder sb = new();

				sb.Append((int)Response.StatusCode);
				sb.Append(' ');
				sb.AppendLine(Response.ReasonPhrase);

				foreach (KeyValuePair<string, IEnumerable<string>> Header in Response.Headers)
				{
					sb.Append(Header.Key);
					sb.Append(": ");
					sb.AppendLine(string.Join(", ", Header.Value));
				}

				this.ReceiveText(sb.ToString());
			}

			Data = await Response.Content.ReadAsByteArrayAsync();

			if (this.HasSniffers && Data is not null)
				this.ReceiveBinary(false, Data);

			Response.EnsureSuccessStatusCode();

			return Data;
		}

		private HttpClient GetClient()
		{
			SocketsHttpHandler Handler = new()
			{
				Credentials = this.credentials,
				CookieContainer = this.cookies,
				AllowAutoRedirect = true,
				UseCookies = true,
				AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.Brotli,
				InitialHttp2StreamWindowSize = 65535,
				ConnectTimeout = TimeSpan.FromSeconds(10),
				SslOptions = new SslClientAuthenticationOptions()
				{
					RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
				}
			};

			HttpClient Client = new(Handler)
			{
				DefaultRequestVersion = this.protocolVersion,
				DefaultVersionPolicy = HttpVersionPolicy.RequestVersionExact,
				Timeout = TimeSpan.FromSeconds(240)
			};

			Client.DefaultRequestHeaders.ConnectionClose = false;

			return Client;
		}

		private HttpRequestMessage GetRequest(HttpMethod Method, string Url,
			params KeyValuePair<string, string>[] Headers)
		{
			HttpRequestMessage Request = new(Method, Url)
			{
				Version = this.protocolVersion,
				VersionPolicy = HttpVersionPolicy.RequestVersionExact
			};

			if (Headers is not null)
			{
				foreach (KeyValuePair<string, string> Header in Headers)
				{
					switch (Header.Key)
					{
						case "Accept":
							if (!Request.Headers.Accept.TryParseAdd(Header.Value))
								throw new InvalidOperationException("Invalid Accept header value: " + Header.Value);
							break;

						case "Authorization":
							int i = Header.Value.IndexOf(' ');
							if (i < 0)
								Request.Headers.Authorization = new AuthenticationHeaderValue(Header.Value);
							else
								Request.Headers.Authorization = new AuthenticationHeaderValue(Header.Value[..i], Header.Value[(i + 1)..].TrimStart());
							break;

						case "Cookie":
							foreach (KeyValuePair<string, string> P in CommonTypes.ParseFieldValues(Header.Value))
								this.cookies.Add(Request.RequestUri, new Cookie(P.Key, P.Value));
							break;

						default:
							Request.Headers.Add(Header.Key, Header.Value);
							break;
					}
				}
			}

			return Request;
		}
	}
}
