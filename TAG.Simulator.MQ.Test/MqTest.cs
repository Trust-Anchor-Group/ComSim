using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkiaSharp;
using Waher.Networking.Sniffers;

namespace TAG.Simulator.MQ.Test
{
	[TestClass]
	public class MqTests
	{
		private ConsoleOutSniffer sniffer;
		private MqClient client;

		[TestInitialize]
		public async Task TestInitialize()
		{
			this.sniffer = new ConsoleOutSniffer(BinaryPresentationMethod.Base64, LineEnding.NewLine);
			this.client = new MqClient("QM1", "DEV.APP.SVRCONN", "lab.tagroot.io", 1414, this.sniffer);
			await this.client.ConnectAsync("TestUser", "TestPassword1234");
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
		public async Task Test_02_Put()
		{
			await this.client.PutAsync("DEV.QUEUE.1", "Hello");
		}

		[TestMethod]
		public async Task Test_03_ReadOne()
		{
			Console.Out.WriteLine(await this.client.GetOneAsync("DEV.QUEUE.1"));
		}

		[TestMethod]
		public async Task Test_04_Subscribe()
		{
			ManualResetEvent Cancel = new ManualResetEvent(false);
			TaskCompletionSource<bool> Stopped = new TaskCompletionSource<bool>();
			int i;

			this.client.SubscribeIncoming("DEV.QUEUE.1", Cancel, Stopped,
				(sender, e) =>
				{
					if (int.TryParse(e.Message, out int i) && i == 10)
						Cancel.Set();

					return Task.CompletedTask;
				}, null);

			await Task.Delay(2000);

			for (i = 1; i <= 10; i++)
				await this.client.PutAsync("DEV.QUEUE.1", i.ToString());

			await Stopped.Task;
		}

		[TestMethod]
		public async Task Test_05_CharacterEncoding()
		{
			await this.client.PutAsync("DEV.QUEUE.1", "你好");
			Assert.AreEqual("你好", await this.client.GetOneAsync("DEV.QUEUE.1"));
		}
	}
}
