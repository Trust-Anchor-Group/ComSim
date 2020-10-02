using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading;
using IBM.WMQ;
using Waher.Events;
using Waher.Networking.Sniffers;

namespace TAG.Simulator.MQ
{
	/// <summary>
	/// IBM MQ client
	/// </summary>
	public class MqClient : Sniffable, IDisposable
	{
		/// <summary>
		/// Default port for IBM MQ is 1414.
		/// </summary>
		public const int DefaultPort = 1414;

		private readonly Dictionary<string, MQQueue> inputQueues = new Dictionary<string, MQQueue>();
		private readonly Dictionary<string, MQQueue> outputQueues = new Dictionary<string, MQQueue>();
		private readonly string queueManager;
		private readonly string channel;
		private readonly string cipher;
		private readonly string cipherSuite;
		private readonly string certificateStore;
		private readonly string host;
		private readonly int port;
		private MQQueueManager manager;

		/// <summary>
		/// IBM MQ client
		/// </summary>
		/// <param name="QueueManager">Name of Queue Manager</param>
		/// <param name="Channel">Name of channel</param>
		/// <param name="Host">Host</param>
		/// <param name="Port">Port number</param>
		/// <param name="Sniffers">Sniffers</param>
		public MqClient(string QueueManager, string Channel, string Host, int Port,
			params ISniffer[] Sniffers)
			: this(QueueManager, Channel, "TLS_RSA_WITH_AES_128_CBC_SHA256",
			"SSL_RSA_WITH_AES_128_CBC_SHA256", "*USER", Host, Port, Sniffers)
		{
		}

		/// <summary>
		/// IBM MQ client
		/// </summary>
		/// <param name="QueueManager">Name of Queue Manager</param>
		/// <param name="Channel">Name of channel</param>
		/// <param name="Cipher">Name of cipher</param>
		/// <param name="CipherSuite">Name of cipher suite</param>
		/// <param name="CertificateStore">Name of certificate store</param>
		/// <param name="Host">Host</param>
		/// <param name="Port">Port number</param>
		/// <param name="Sniffers">Sniffers</param>
		public MqClient(string QueueManager, string Channel, string Cipher, string CipherSuite, string CertificateStore,
			string Host, int Port, params ISniffer[] Sniffers)
			: base(Sniffers)
		{
			this.queueManager = QueueManager;
			this.channel = Channel;
			this.cipher = Cipher;
			this.cipherSuite = CipherSuite;
			this.certificateStore = CertificateStore;
			this.host = Host;
			this.port = Port;
		}

		/// <summary>
		/// <see cref="IDisposable.Dispose"/>
		/// </summary>
		public void Dispose()
		{
			this.Close();
		}

		/// <summary>
		/// Connects to the Queue Manager
		/// </summary>
		/// <param name="UserName">User name</param>
		/// <param name="Password">Password</param>
		public void Connect(string UserName, string Password)
		{
			this.Information("Connecting...");
			try
			{
				Hashtable ConnectionParameters = new Hashtable()
				{
					{ MQC.TRANSPORT_PROPERTY, MQC.TRANSPORT_MQSERIES_MANAGED },
					{ MQC.HOST_NAME_PROPERTY, this.host },
					{ MQC.PORT_PROPERTY, this.port },
					{ MQC.CHANNEL_PROPERTY, this.channel },
					{ MQC.USER_ID_PROPERTY, UserName },
					{ MQC.PASSWORD_PROPERTY, Password },
					//{ MQC.SSL_CIPHER_SPEC_PROPERTY, this.cipher },
					//{ MQC.SSL_CIPHER_SUITE_PROPERTY, this.cipherSuite },
					//{ MQC.SSL_CERT_STORE_PROPERTY, this.certificateStore }
				};

				this.manager = new MQQueueManager(this.queueManager, ConnectionParameters);
			}
			catch (Exception ex)
			{
				this.Exception(ex);
				ExceptionDispatchInfo.Capture(ex).Throw();
			}
		}

		/// <summary>
		/// Closes the connection.
		/// </summary>
		public void Close()
		{
			lock (this.inputQueues)
			{
				foreach (MQQueue Queue in this.inputQueues.Values)
					Queue.Close();
			}

			lock (this.outputQueues)
			{
				foreach (MQQueue Queue in this.outputQueues.Values)
					Queue.Close();
			}

			if (!(this.manager is null))
			{
				this.Information("Closing...");

				this.manager?.Close();
				this.manager = null;
			}
		}

		/// <summary>
		/// Pushes an item onto a queue.
		/// </summary>
		/// <param name="QueueName">Queue name.</param>
		/// <param name="Item">Item</param>
		public void Put(string QueueName, string Item)
		{
			try
			{
				MQQueue Queue;

				lock (this.outputQueues)
				{
					if (!this.outputQueues.TryGetValue(QueueName, out Queue))
					{
						Queue = this.manager.AccessQueue(QueueName, MQC.MQOO_OUTPUT | MQC.MQOO_FAIL_IF_QUIESCING);
						this.outputQueues[QueueName] = Queue;
					}
				}

				this.TransmitText(Item);

				MQMessage Message = new MQMessage()
				{
					CharacterSet = 1208, // UTF-8
					Format = MQC.MQFMT_STRING
				};

				Message.WriteString(Item);

				Queue.Put(Message);
			}
			catch (Exception ex)
			{
				this.Exception(ex);
			}
		}

		/// <summary>
		/// Read one message from a queue.
		/// </summary>
		/// <param name="QueueName">Name of queue.</param>
		/// <returns>Message read.</returns>
		public string ReadOne(string QueueName)
		{
			return this.ReadOne(QueueName, MQC.MQWI_UNLIMITED);
		}

		/// <summary>
		/// Read one message from a queue.
		/// </summary>
		/// <param name="QueueName">Name of queue.</param>
		/// <param name="TimeoutMilliseconds">Timeout, in milliseconds.</param>
		/// <returns>Message read.</returns>
		public string ReadOne(string QueueName, int TimeoutMilliseconds)
		{
			string Result = null;

			try
			{
				MQQueue Queue;

				lock (this.inputQueues)
				{
					if (!this.inputQueues.TryGetValue(QueueName, out Queue))
					{
						Queue = this.manager.AccessQueue(QueueName, MQC.MQOO_INPUT_AS_Q_DEF | MQC.MQOO_FAIL_IF_QUIESCING);
						this.inputQueues[QueueName] = Queue;
					}
				}

				MQMessage Message = new MQMessage();
				MQGetMessageOptions Options = new MQGetMessageOptions()
				{
					Options = MQC.MQGMO_FAIL_IF_QUIESCING | MQC.MQGMO_WAIT,
					WaitInterval = TimeoutMilliseconds
				};

				Queue.Get(Message, Options);

				Result = Message.ReadString(Message.MessageLength);
				this.ReceiveText(Result);
			}
			catch (Exception ex)
			{
				this.Exception(ex);
				ExceptionDispatchInfo.Capture(ex).Throw();
			}

			return Result;
		}

		/// <summary>
		/// Subscribes to incoming messages.
		/// </summary>
		/// <param name="QueueName">Queue name.</param>
		/// <param name="Callback">Method to call when new message has been read.</param>
		/// <param name="State">State object to pass on to callback method.</param>
		public void SubscribeIncoming(string QueueName, MqMessageEventHandler Callback, object State)
		{
			this.SubscribeIncoming(QueueName, null, null, Callback, State);
		}

		/// <summary>
		/// Subscribes to incoming messages.
		/// </summary>
		/// <param name="QueueName">Queue name.</param>
		/// <param name="Cancel">Cancel event. Set this event object, to cancel subscription.</param>
		/// <param name="Callback">Method to call when new message has been read.</param>
		/// <param name="State">State object to pass on to callback method.</param>
		public void SubscribeIncoming(string QueueName, ManualResetEvent Cancel, MqMessageEventHandler Callback, object State)
		{
			this.SubscribeIncoming(QueueName, Cancel, null, Callback, State);
		}

		/// <summary>
		/// Subscribes to incoming messages.
		/// </summary>
		/// <param name="QueueName">Queue name.</param>
		/// <param name="Cancel">Cancel event. Set this event object, to cancel subscription.</param>
		/// <param name="Stopped">Optional Event that will be set when the subscription has ended.</param>
		/// <param name="Callback">Method to call when new message has been read.</param>
		/// <param name="State">State object to pass on to callback method.</param>
		public void SubscribeIncoming(string QueueName, ManualResetEvent Cancel, ManualResetEvent Stopped,
			MqMessageEventHandler Callback, object State)
		{
			Thread T = new Thread(this.SubscriptionThread);
			T.Name = "MQ Queue Subcription " + QueueName;
			T.Priority = ThreadPriority.BelowNormal;
			T.Start(new object[] { QueueName, Cancel, Stopped, Callback, State });
		}

		private void SubscriptionThread(object Parameter)
		{
			object[] P = (object[])Parameter;
			string QueueName = (string)P[0];
			ManualResetEvent Cancel = (ManualResetEvent)P[1];
			ManualResetEvent Stopped = (ManualResetEvent)P[2];
			MqMessageEventHandler Callback = (MqMessageEventHandler)P[3];
			object State = P[4];

			try
			{
				while (!(Cancel?.WaitOne(0) ?? false))
				{
					string Message = this.ReadOne(QueueName, 1000);
					this.Callback(Callback, new MqMessageEventArgs(this, Message, State));
				}
			}
			catch (Exception ex)
			{
				Log.Critical("Subscription cancelled due to exception: " + ex.Message, QueueName, string.Empty, string.Empty,
					EventLevel.Major, string.Empty, string.Empty, ex.StackTrace);
			}
			finally
			{
				Stopped?.Set();
			}
		}

		private async void Callback(MqMessageEventHandler Method, MqMessageEventArgs e)
		{
			try
			{
				await Method(this, e);
			}
			catch (Exception ex)
			{
				Log.Critical(ex);
			}
		}

	}
}
