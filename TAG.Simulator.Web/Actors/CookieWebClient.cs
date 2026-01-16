using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using TAG.Simulator.Web.Activities;
using Waher.Content;
using Waher.Content.Binary;
using Waher.Content.Getters;
using Waher.Content.Json;
using Waher.Content.Xml.Text;
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
		/// <param name="Timeout">Duration before a request times out.</param>
		/// <param name="Headers">HTTP Headers</param>
		/// <returns>Response</returns>
		public Task<ContentResponse> HEAD(string Url, Duration Timeout,
			params KeyValuePair<string, string>[] Headers)
		{
			return this.Send(Url, HttpMethod.Head, Timeout, Headers);
		}

		/// <summary>
		/// Performs a GET operation.
		/// </summary>
		/// <param name="Url">URL</param>
		/// <param name="Timeout">Duration before a request times out.</param>
		/// <param name="Headers">HTTP Headers</param>
		/// <returns>Response</returns>
		public Task<ContentResponse> GET(string Url, Duration Timeout,
			params KeyValuePair<string, string>[] Headers)
		{
			return this.Send(Url, HttpMethod.Get, Timeout, Headers);
		}

		/// <summary>
		/// Performs a POST operation.
		/// </summary>
		/// <param name="Url">URL</param>
		/// <param name="Data">Payload</param>
		/// <param name="Timeout">Duration before a request times out.</param>
		/// <param name="Headers">HTTP Headers</param>
		/// <returns>Response</returns>
		public Task<ContentResponse> POST(string Url, byte[] Data, Duration Timeout,
			params KeyValuePair<string, string>[] Headers)
		{
			return this.Send(Url, HttpMethod.Post, Data, Timeout, Headers);
		}

		/// <summary>
		/// Performs a PUT operation.
		/// </summary>
		/// <param name="Url">URL</param>
		/// <param name="Data">Payload</param>
		/// <param name="Timeout">Duration before a request times out.</param>
		/// <param name="Headers">HTTP Headers</param>
		/// <returns>Response</returns>
		public Task<ContentResponse> PUT(string Url, byte[] Data, Duration Timeout,
			params KeyValuePair<string, string>[] Headers)
		{
			return this.Send(Url, HttpMethod.Put, Data, Timeout, Headers);
		}

		/// <summary>
		/// Performs a DELETE operation.
		/// </summary>
		/// <param name="Url">URL</param>
		/// <param name="Timeout">Duration before a request times out.</param>
		/// <param name="Headers">HTTP Headers</param>
		/// <returns>Response</returns>
		public Task<ContentResponse> DELETE(string Url, Duration Timeout,
			params KeyValuePair<string, string>[] Headers)
		{
			return this.Send(Url, HttpMethod.Delete, Timeout, Headers);
		}

		/// <summary>
		/// Performs a OPTIONS operation.
		/// </summary>
		/// <param name="Url">URL</param>
		/// <param name="Timeout">Duration before a request times out.</param>
		/// <param name="Headers">HTTP Headers</param>
		/// <returns>Response</returns>
		public Task<ContentResponse> OPTIONS(string Url, Duration Timeout,
			params KeyValuePair<string, string>[] Headers)
		{
			return this.Send(Url, HttpMethod.Options, Timeout, Headers);
		}

		/// <summary>
		/// Performs a TRACE operation.
		/// </summary>
		/// <param name="Url">URL</param>
		/// <param name="Timeout">Duration before a request times out.</param>
		/// <param name="Headers">HTTP Headers</param>
		/// <returns>Response</returns>
		public Task<ContentResponse> TRACE(string Url, Duration Timeout,
			params KeyValuePair<string, string>[] Headers)
		{
			return this.Send(Url, HttpMethod.Trace, Timeout, Headers);
		}

		/// <summary>
		/// Performs a PATCH operation.
		/// </summary>
		/// <param name="Url">URL</param>
		/// <param name="Data">Payload</param>
		/// <param name="Timeout">Duration before a request times out.</param>
		/// <param name="Headers">HTTP Headers</param>
		/// <returns>Response</returns>
		public Task<ContentResponse> PATCH(string Url, byte[] Data, Duration Timeout,
			params KeyValuePair<string, string>[] Headers)
		{
			return this.Send(Url, HttpMethod.Patch, Data, Timeout, Headers);
		}

		/// <summary>
		/// Performs an HTTP method with no payload.
		/// </summary>
		/// <param name="Url">URL</param>
		/// <param name="Method">HTTP Method</param>
		/// <param name="Timeout">Duration before a request times out.</param>
		/// <param name="Headers">HTTP Headers</param>
		/// <returns>Response</returns>
		private Task<ContentResponse> Send(string Url, HttpMethod Method, Duration Timeout,
			params KeyValuePair<string, string>[] Headers)
		{
			return this.Send(Url, Method, null, Timeout, Headers);
		}

		/// <summary>
		/// Performs an HTTP method with payload.
		/// </summary>
		/// <param name="Url">URL</param>
		/// <param name="Method">HTTP Method</param>
		/// <param name="Data">Binary payload.</param>
		/// <param name="Timeout">Duration before a request times out.</param>
		/// <param name="Headers">HTTP Headers</param>
		/// <returns>Response</returns>
		private async Task<ContentResponse> Send(string Url, HttpMethod Method, byte[] Data,
			Duration Timeout, params KeyValuePair<string, string>[] Headers)
		{
			Uri Uri = new(Url);
			using HttpClient Client = this.GetClient(Timeout);
			HttpRequestMessage Request = this.GetRequest(Method, Uri, Data, Headers);

			if (this.HasSniffers)
			{
				StringBuilder sb = new();
				string ContentType = BinaryCodec.DefaultContentType;

				sb.Append(Method.ToString().ToUpper());
				sb.Append(' ');
				sb.Append(Url);

				if (this.protocolVersion is not null)
				{
					sb.Append(" HTTP/");
					sb.Append(this.protocolVersion.ToString());
				}

				sb.AppendLine();

				foreach (KeyValuePair<string, string> Header in Headers)
				{
					switch (Header.Key)
					{
						case "Accept":
						case "Authorization":
						case "Cookie":
							continue;

						case "Content-Type":
							ContentType = Header.Value;
							break;
					}

					sb.Append(Header.Key);
					sb.Append(": ");
					sb.AppendLine(Header.Value);
				}

				string s = this.cookies.GetCookieHeader(Uri);
				if (!string.IsNullOrEmpty(s))
				{
					sb.Append("Cookie: ");
					sb.AppendLine(s);
				}

				if (Request.Content?.Headers is not null)
				{
					foreach (KeyValuePair<string, IEnumerable<string>> Header in Request.Content.Headers)
					{
						sb.Append(Header.Key);
						sb.Append(": ");
						sb.AppendLine(string.Join(", ", Header.Value));
					}
				}

				this.TransmitText(sb.ToString());

				if (Data is not null)
				{
					if (IsText(ContentType, out Encoding Encoding))
						this.TransmitText(Encoding.GetString(Data));
					else
						this.TransmitBinary(false, Data);
				}
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

				if (Response.Content?.Headers is not null)
				{
					foreach (KeyValuePair<string, IEnumerable<string>> Header in Response.Content.Headers)
					{
						sb.Append(Header.Key);
						sb.Append(": ");
						sb.AppendLine(string.Join(", ", Header.Value));
					}
				}

				this.ReceiveText(sb.ToString());
			}

			Data = await Response.Content.ReadAsByteArrayAsync();

			if (this.HasSniffers && Data is not null)
			{
				string ContentType = Response.Content.Headers.ContentType?.ToString() ?? BinaryCodec.DefaultContentType;

				if (IsText(ContentType, out Encoding Encoding))
					this.ReceiveText(Encoding.GetString(Data));
				else
					this.ReceiveBinary(false, Data);
			}

			return await WebGetter.ProcessResponse(Response, Uri);
		}

		private static bool IsText(string ContentType, out Encoding Encoding)
		{
			int i = ContentType.IndexOf(';');
			if (i > 0)
			{
				KeyValuePair<string, string>[] Fields = CommonTypes.ParseFieldValues(ContentType.Trim());
				string Charset = null;

				foreach (KeyValuePair<string, string> P in Fields)
				{
					if (string.Compare(P.Key, "charset", true) == 0)
					{
						Charset = P.Value;
						break;
					}
				}

				if (Charset is null)
					Encoding = Encoding.UTF8;
				else
					Encoding = Encoding.GetEncoding(Charset);

				ContentType = ContentType[..i].TrimEnd();
			}
			else
				Encoding = Encoding.UTF8;

			if (ContentType.StartsWith("text/"))
				return true;

			if (Array.IndexOf(JsonCodec.JsonContentTypes, ContentType) >= 0 ||
				Array.IndexOf(XmlCodec.XmlContentTypes, ContentType) >= 0)
			{
				return true;
			}

			return false;
		}

		private HttpClient GetClient(Duration Timeout)
		{
			DateTime Now = DateTime.UtcNow;
			TimeSpan TS = (Now + Timeout).Subtract(Now);

			SocketsHttpHandler Handler = new()
			{
				Credentials = this.credentials,
				CookieContainer = this.cookies,
				AllowAutoRedirect = true,
				UseCookies = true,
				AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.Brotli,
				InitialHttp2StreamWindowSize = 65535,
				ConnectTimeout = TS.TotalSeconds < 10 ? TS : TimeSpan.FromSeconds(10),
				SslOptions = new SslClientAuthenticationOptions()
				{
					RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
				}
			};

			HttpClient Client = new(Handler)
			{
				Timeout = TS
			};

			if (this.protocolVersion is not null)
			{
				Client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionExact;
				Client.DefaultRequestVersion = this.protocolVersion;
			}

			Client.DefaultRequestHeaders.ConnectionClose = false;

			return Client;
		}

		private HttpRequestMessage GetRequest(HttpMethod Method, Uri Uri, byte[] Data,
			params KeyValuePair<string, string>[] Headers)
		{
			HttpRequestMessage Request = new(Method, Uri);

			if (Data is not null)
				Request.Content = new ByteArrayContent(Data);

			if (this.protocolVersion is not null)
			{
				Request.Version = this.protocolVersion;
				Request.VersionPolicy = HttpVersionPolicy.RequestVersionExact;
			}

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

						case "Content-Type":
							if (Request.Content is null)
								throw new InvalidOperationException("No data provided for Content-Type header.");

							Request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(Header.Value);
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
