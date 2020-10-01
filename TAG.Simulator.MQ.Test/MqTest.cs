using System;
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
	}
}
