using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Waher.Networking.Sniffers;

namespace TAG.Simulator.MQ.Test
{
	[TestClass]
	public class MqTests
	{
		private ConsoleOutSniffer sniffer;
		private MqClient client;

		[TestInitialize]
		public void TestInitialize()
		{
			this.sniffer = new ConsoleOutSniffer(BinaryPresentationMethod.Base64, LineEnding.NewLine);
			this.client = new MqClient("QM1", "DEV.APP.SVRCONN", "lab.tagroot.io", 1414, this.sniffer);
			this.client.Connect("TestUser", "TestPassword1234");
		}

		[TestCleanup]
		public void TestCleanup()
		{
			this.client.Dispose();
			this.client = null;
			this.sniffer = null;
		}

		[TestMethod]
		public void Test_01_Connect()
		{
		}

		[TestMethod]
		public void Test_02_Put()
		{
			this.client.Put("DEV.QUEUE.1", "Hello");
		}

		[TestMethod]
		public void Test_03_ReadOne()
		{
			Console.Out.WriteLine(this.client.ReadOne("DEV.QUEUE.1"));
		}

		[TestMethod]
		public void Test_04_Subscribe()
		{
			ManualResetEvent Cancel = new ManualResetEvent(false);
			ManualResetEvent Stopped = new ManualResetEvent(false);
			int i;

			this.client.SubscribeIncoming("DEV.QUEUE.1", Cancel, Stopped,
				(sender, e) =>
				{
					Console.Out.WriteLine("Received: " + e.Message);

					if (int.TryParse(e.Message, out int i) && i == 10)
						Cancel.Set();

					return Task.CompletedTask;
				}, null);

			
			Thread.Sleep(2000);

			for (i = 1; i <= 10; i++)
				this.client.Put("DEV.QUEUE.1", i.ToString());

			Assert.IsTrue(Stopped.WaitOne(5000));
		}

		[TestMethod]
		public void Test_05_CharacterEncoding()
		{
			this.client.Put("DEV.QUEUE.1", "你好");
			Assert.AreEqual("你好", this.client.ReadOne("DEV.QUEUE.1"));
		}
	}
}
