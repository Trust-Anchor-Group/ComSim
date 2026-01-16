using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel.Actors;
using Waher.Content.Xml;
using Waher.Events;
using Waher.Networking.Sniffers;

namespace TAG.Simulator.Web.Actors
{
	/// <summary>
	/// Represents a web-socket actor.
	/// </summary>
	public class WebSocketActor : Actor
	{
		private readonly CancellationTokenSource cancel = new();
		private ClientWebSocket client;
		private ISniffer sniffer;
		private Task connectionTask;
		private string url;
		private string protocol;
		private string sessionActor;
		private bool closed = false;

		/// <summary>
		/// Represents a web-socket actor.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public WebSocketActor(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Represents a web-socket actor.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <param name="InstanceIndex">Instance index.</param>
		/// <param name="InstanceId">ID of instance</param>
		public WebSocketActor(ISimulationNode Parent, Model Model, int InstanceIndex, string InstanceId)
			: base(Parent, Model, InstanceIndex, InstanceId)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(WebSocketActor);

		/// <summary>
		/// XML Namespace where the element is defined.
		/// </summary>
		public override string Namespace => WebActor.WebNamespace;

		/// <summary>
		/// Points to the embedded XML Schema resource defining the semantics of the XML namespace.
		/// </summary>
		public override string SchemaResource => WebActor.WebSchema;

		/// <summary>
		/// WebSocket Client
		/// </summary>
		public ClientWebSocket Client => this.client;

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.url = XML.Attribute(Definition, "url");
			this.protocol = XML.Attribute(Definition, "protocol");
			this.sessionActor = XML.Attribute(Definition, "sessionActor");

			return base.FromXml(Definition);
		}

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new WebSocketActor(Parent, Model);
		}

		/// <summary>
		/// Creates an instance of the actor.
		/// 
		/// Note: Parent of newly created actor should point to this node (the creator of the instance object).
		/// </summary>
		/// <param name="InstanceIndex">Instance index.</param>
		/// <param name="InstanceId">ID of instance</param>
		/// <returns>Actor instance.</returns>
		public override Task<Actor> CreateInstanceAsync(int InstanceIndex, string InstanceId)
		{
			WebSocketActor Result = new(this, this.Model, InstanceIndex, InstanceId)
			{
				url = this.url,
				protocol = this.protocol,
				sessionActor = this.sessionActor
			};

			return Task.FromResult<Actor>(Result);
		}

		/// <summary>
		/// Initializes an instance of an actor.
		/// </summary>
		public override Task InitializeInstance()
		{
			this.sniffer = this.Model.GetSniffer(this.InstanceId);
			this.connectionTask = this.DoConnect(true);

			return Task.CompletedTask;
		}

		private async Task DoConnect(bool StartReading)
		{
			try
			{
				this.sniffer.Information("Connecting to " + this.url);

				this.client?.Dispose();
				this.client = null;

				this.client = new ClientWebSocket();
				this.client.Options.AddSubProtocol(this.protocol);

				Uri Uri = new(this.url);

				if (!string.IsNullOrEmpty(this.sessionActor))
				{
					string SessionInstance = this.sessionActor + this.InstanceId.Substring(this.Id.Length);

					this.sniffer.Information("Web session instance: " + SessionInstance);

					if (!this.Model.TryGetActor(this.sessionActor, out IActor Actor))
						throw new Exception("Session actor " + this.sessionActor + " not found.");

					if (!Actor.TryGetInstance(SessionInstance, out IActor Instance))
						throw new Exception("Session instance " + SessionInstance + " actor not found.");

					if (Instance is not WebActor WebActor)
						throw new Exception("Session instance " + SessionInstance + " is not a web actor.");

					this.client.Options.Cookies = WebActor.Client.Cookies;

					string s = this.client.Options.Cookies.GetCookieHeader(Uri);
					if (!string.IsNullOrEmpty(s))
						this.sniffer.Information("Cookie header: " + s);

					await this.client.ConnectAsync(Uri, this.cancel.Token);

					if (string.IsNullOrEmpty(s))
					{
						s = this.client.Options.Cookies.GetCookieHeader(Uri);
						if (!string.IsNullOrEmpty(s))
							this.sniffer.Information("Cookie received: " + s);
					}
				}
				else
					await this.client.ConnectAsync(Uri, this.cancel.Token);

				this.sniffer.Information("Connected to " + this.url);

				if (StartReading)
					_ = Task.Run(this.ReadIncoming);

				await this.Model.ExternalEvent(this, "OnConnected",
					new KeyValuePair<string, object>("Client", this));
			}
			catch (Exception ex)
			{
				this.sniffer.Exception(ex);
				ExceptionDispatchInfo.Capture(ex).Throw();
			}
		}

		/// <summary>
		/// Finalizes an instance of an actor.
		/// </summary>
		public override Task FinalizeInstance()
		{
			this.closed = true;
			this.cancel.Cancel();

			if (this.client is not null)
			{
				this.client.Dispose();
				this.client = null;
			}

			if (this.sniffer is not null)
			{
				if (this.sniffer is IDisposable Disposable)
					Disposable.Dispose();

				this.sniffer = null;
			}

			return Task.CompletedTask;
		}

		/// <summary>
		/// Starts an instance of an actor.
		/// </summary>
		public override async Task StartInstance()
		{
			await this.connectionTask;
		}

		public async Task SendText(string s)
		{
			byte[] Data = Encoding.UTF8.GetBytes(s);

			this.sniffer.TransmitText(s);

			await this.client.SendAsync(Data, WebSocketMessageType.Text, true,
				this.cancel.Token);
		}

		public async Task SendText(byte[] Data)
		{
			this.sniffer.TransmitText(Encoding.UTF8.GetString(Data));

			await this.client.SendAsync(Data, WebSocketMessageType.Text, true,
				this.cancel.Token);
		}

		public async Task SendBinary(byte[] Data)
		{
			this.sniffer.TransmitBinary(false, Data);

			await this.client.SendAsync(Data, WebSocketMessageType.Binary, true,
				this.cancel.Token);
		}

		private async void ReadIncoming()
		{
			try
			{
				MemoryStream ms = null;
				byte[] Buffer = new byte[65536];

				while (!this.cancel.Token.IsCancellationRequested)
				{
					WebSocketReceiveResult Result = await this.client.ReceiveAsync(Buffer, this.cancel.Token);

					if (Result.CloseStatus.HasValue)
					{
						this.sniffer.Information("Connection closed. Status: " +
							Result.CloseStatus.Value.ToString() + " (" +
							Result.CloseStatusDescription + ")");

						await this.Model.ExternalEvent(this, "OnClosed",
							new KeyValuePair<string, object>("Client", this),
							new KeyValuePair<string, object>("Status", Result.CloseStatus.Value),
							new KeyValuePair<string, object>("Description", Result.CloseStatusDescription));

						if (this.closed)
						{
							Log.Informational("WebSocket closed.",
								this.InstanceId,
								new KeyValuePair<string, object>("Status", Result.CloseStatus.Value),
								new KeyValuePair<string, object>("Description", Result.CloseStatusDescription));
							return;
						}
						else
						{
							Log.Warning("WebSocket closed.",
								this.InstanceId,
								new KeyValuePair<string, object>("Status", Result.CloseStatus.Value),
								new KeyValuePair<string, object>("Description", Result.CloseStatusDescription));

							await this.DoConnect(false);
						}

						continue;
					}

					if (!Result.EndOfMessage)
					{
						ms ??= new MemoryStream();
						ms.Write(Buffer, 0, Result.Count);
						continue;
					}

					byte[] Data;

					if (ms is null)
					{
						Data = new byte[Result.Count];
						System.Buffer.BlockCopy(Buffer, 0, Data, 0, Result.Count);
					}
					else
					{
						ms.Write(Buffer, 0, Result.Count);
						Data = ms.ToArray();
						ms.Dispose();
						ms = null;
					}

					switch (Result.MessageType)
					{
						case WebSocketMessageType.Text:
							string Text = System.Text.Encoding.UTF8.GetString(Data);
							this.sniffer.ReceiveText(Text);

							await this.Model.ExternalEvent(this, "OnTextReceived",
								new KeyValuePair<string, object>("Client", this),
								new KeyValuePair<string, object>("Text", Text));
							break;

						case WebSocketMessageType.Binary:
							this.sniffer.ReceiveBinary(true, Data);

							await this.Model.ExternalEvent(this, "OnBinaryReceived",
								new KeyValuePair<string, object>("Client", this),
								new KeyValuePair<string, object>("Data", Data));
							break;
					}
				}
			}
			catch (TaskCanceledException)
			{
				Log.Informational("WebSocket reading task canceled.", this.InstanceId);
			}
			catch (Exception ex)
			{
				Log.Exception(ex, this.InstanceId);
			}
		}

		/// <summary>
		/// Returns the object that will be used by the actor for actions during an activity.
		/// </summary>
		public override object ActivityObject
		{
			get
			{
				return new WebSocketActorActivityObject()
				{
					Client = this,
					Protocol = this.protocol,
					InstanceId = this.InstanceId,
					InstanceIndex = this.InstanceIndex
				};
			}
		}

	}
}
