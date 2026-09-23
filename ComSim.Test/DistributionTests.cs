using System.Text;
using System.Xml;
using TAG.Simulator;
using TAG.Simulator.ObjectModel.Distributions;
using Waher.Runtime.Collections;
using Waher.Runtime.Inventory;
using Waher.Script;
using Waher.Script.Graphs;
using Waher.Script.Statistics.Functions;

namespace ComSim.Test
{
	[TestClass]
	[DoNotParallelize]
	public sealed class DistributionTests
	{
		[AssemblyInitialize]
		public static void AssemblyInitialize(TestContext context)
		{
			Types.Initialize(
				typeof(DistributionTests).Assembly,
				typeof(Expression).Assembly,
				typeof(Graph).Assembly,
				typeof(Histogram).Assembly,
				typeof(ISimulationNode).Assembly);

			Factory.Initialize();
		}

		[TestMethod]
		[DataRow(1,
			"<Model timeBase='StartOfSimulation' bucketTime='PT1S' timeUnit='PT1S' timeCycle='PT1M' duration='PT1M' sampleEpsilon='true'/>",
			"<Uniform id='U0' N='200' from='0' to='60'/>")]
		[DataRow(1,
			"<Model timeBase='StartOfSimulation' bucketTime='PT1S' timeUnit='PT1S' timeCycle='PT1M' duration='PT1M' sampleEpsilon='true'/>",
			"<Uniform id='U1' N='200' from='10' to='50'/>")]
		[DataRow(1,
			"<Model timeBase='StartOfSimulation' bucketTime='PT1S' timeUnit='PT1S' timeCycle='PT1M' duration='PT1M' sampleEpsilon='true'/>",
			"<Uniform id='U2' N='200' from='50' to='10'/>")]
		[DataRow(1,
			"<Model timeBase='StartOfSimulation' bucketTime='PT1S' timeUnit='PT1S' timeCycle='PT1M' duration='PT1M' sampleEpsilon='true'/>",
			"<LinearIncrease id='I0' N='200' from='0' to='60'/>")]
		[DataRow(1,
			"<Model timeBase='StartOfSimulation' bucketTime='PT1S' timeUnit='PT1S' timeCycle='PT1M' duration='PT1M' sampleEpsilon='true'/>",
			"<LinearIncrease id='I1' N='200' from='10' to='50'/>")]
		[DataRow(1,
			"<Model timeBase='StartOfSimulation' bucketTime='PT1S' timeUnit='PT1S' timeCycle='PT1M' duration='PT1M' sampleEpsilon='true'/>",
			"<LinearIncrease id='I2' N='200' from='50' to='10'/>")]
		[DataRow(1,
			"<Model timeBase='StartOfSimulation' bucketTime='PT1S' timeUnit='PT1S' timeCycle='PT1M' duration='PT1M' sampleEpsilon='true'/>",
			"<LinearDecrease id='D0' N='200' from='0' to='60'/>")]
		[DataRow(1,
			"<Model timeBase='StartOfSimulation' bucketTime='PT1S' timeUnit='PT1S' timeCycle='PT1M' duration='PT1M' sampleEpsilon='true'/>",
			"<LinearDecrease id='D1' N='200' from='10' to='50'/>")]
		[DataRow(1,
			"<Model timeBase='StartOfSimulation' bucketTime='PT1S' timeUnit='PT1S' timeCycle='PT1M' duration='PT1M' sampleEpsilon='true'/>",
			"<LinearDecrease id='D2' N='200' from='50' to='10'/>")]
		[DataRow(3,
			"<Model timeBase='StartOfSimulation' bucketTime='PT1S' timeUnit='PT1S' timeCycle='PT1M' duration='PT3M' sampleEpsilon='true'/>",
			"<Uniform id='U3' N='200' from='0' to='60'/>")]
		[DataRow(3,
			"<Model timeBase='StartOfSimulation' bucketTime='PT1S' timeUnit='PT1S' timeCycle='PT1M' duration='PT3M' sampleEpsilon='true'/>",
			"<Uniform id='U4' N='200' from='10' to='50'/>")]
		[DataRow(3,
			"<Model timeBase='StartOfSimulation' bucketTime='PT1S' timeUnit='PT1S' timeCycle='PT1M' duration='PT3M' sampleEpsilon='true'/>",
			"<Uniform id='U5' N='200' from='50' to='10'/>")]
		[DataRow(3,
			"<Model timeBase='StartOfSimulation' bucketTime='PT1S' timeUnit='PT1S' timeCycle='PT1M' duration='PT3M' sampleEpsilon='true'/>",
			"<LinearIncrease id='I3' N='200' from='0' to='60'/>")]
		[DataRow(3,
			"<Model timeBase='StartOfSimulation' bucketTime='PT1S' timeUnit='PT1S' timeCycle='PT1M' duration='PT3M' sampleEpsilon='true'/>",
			"<LinearIncrease id='I4' N='200' from='10' to='50'/>")]
		[DataRow(3,
			"<Model timeBase='StartOfSimulation' bucketTime='PT1S' timeUnit='PT1S' timeCycle='PT1M' duration='PT3M' sampleEpsilon='true'/>",
			"<LinearIncrease id='I5' N='200' from='50' to='10'/>")]
		[DataRow(3,
			"<Model timeBase='StartOfSimulation' bucketTime='PT1S' timeUnit='PT1S' timeCycle='PT1M' duration='PT3M' sampleEpsilon='true'/>",
			"<LinearDecrease id='D3' N='200' from='0' to='60'/>")]
		[DataRow(3,
			"<Model timeBase='StartOfSimulation' bucketTime='PT1S' timeUnit='PT1S' timeCycle='PT1M' duration='PT3M' sampleEpsilon='true'/>",
			"<LinearDecrease id='D4' N='200' from='10' to='50'/>")]
		[DataRow(3,
			"<Model timeBase='StartOfSimulation' bucketTime='PT1S' timeUnit='PT1S' timeCycle='PT1M' duration='PT3M' sampleEpsilon='true'/>",
			"<LinearDecrease id='D5' N='200' from='50' to='10'/>")]
		public async Task Test_01_DrawGraphs(int Cycles, string ModelXml, string DistributionXml)
		{
			XmlDocument Def = new();
			Def.LoadXml(ModelXml);

			Model Model = new(null, null);
			await Model.FromXml(Def.DocumentElement);
			await Model.Initialize();

			Def.LoadXml(DistributionXml);

			ISimulationNode Node = await Factory.Create(Def.DocumentElement, null, Model);
			if (Node is not Distribution Distribution)
				Assert.Fail("Distribution XML does not define a Distribution object.");
			else
			{
				ChunkedList<DateTime> Timestamps = [];
				ChunkedList<double> Times = [];
				ChunkedList<double> TimesAxis = [];
				ChunkedList<double> Cumulative = [];
				DateTime TP = Model.StartTime;
				double t1;
				double t2 = 0;
				double t;
				double I;
				double LastI = 0;
				int NrCycles = 0;
				bool AllIncreasing = true;
				bool InRange = true;

				await Distribution.Initialize();

				while (TP <= Model.EndTime)
				{
					t = (TP - Model.StartTime).TotalMilliseconds;
					t1 = t2;
					t2 = Math.IEEERemainder(t, Model.TimeCycleMs);
					if (t2 < 0)
						t2 += Model.TimeCycleMs;
					t2 /= Model.TimeUnitMs;

					if (t2 < t1)
						NrCycles++;

					Times.Add(t2);
					TimesAxis.Add(t2 + NrCycles * Model.TimeCycleMs / Model.TimeUnitMs);

					I = Distribution!.GetCumulativeProbability(t2, NrCycles);
					AllIncreasing &= I >= LastI;
					InRange &= I >= 0 && I <= Cycles;
					LastI = I;

					Timestamps.Add(TP);
					Cumulative.Add(I);

					TP = TP + Model.TimeUnit;
				}

				Variables Variables = new(
					new Variable("TP", Timestamps.ToArray()),
					new Variable("t", Times.ToArray()),
					new Variable("tx", TimesAxis.ToArray()),
					new Variable("Cumulative", Cumulative.ToArray()));

				Graph G = (Graph)await Expression.EvalAsync(
					"plot2dline(TP,zeroes(count(TP)),'Black',1)+" +
					"plot2dline(TP,ones(count(TP)),'Black',1)+" +
					"plot2dline(TP,Cumulative,'Red',3)+" +
					"scatter2d(TP,Cumulative,'Blue',5)",
					Variables);

				GraphSettings Settings = new GraphSettings()
				{
					Width = 1680,
					Height = 1024
				};

				PixelInformation Pixels = G.CreatePixels(Settings);

				if (!Directory.Exists("Graphs"))
					Directory.CreateDirectory("Graphs");

				await File.WriteAllBytesAsync("Graphs\\I(P)_" + Distribution.Id + ".png",
					Pixels.EncodeAsPng(), CancellationToken.None);

				StringBuilder sb = new();
				sb.Append("P:=");
				Distribution.ExportPdfBody(sb);
				sb.Append(";plot2dline(tx,zeroes(count(t)),'Black',1)+");
				sb.Append("plot2dline(tx,P,'Red',3)+");
				sb.Append("scatter2d(tx,P,'Blue',5)");

				G = (Graph)await Expression.EvalAsync(sb.ToString(), Variables);
				Pixels = G.CreatePixels(Settings);

				await File.WriteAllBytesAsync("Graphs\\PDF_" + Distribution.Id + ".png",
					Pixels.EncodeAsPng(), CancellationToken.None);

				Assert.IsTrue(AllIncreasing, "Cumulative probability function is not increasing.");
				Assert.IsTrue(InRange, "Cumulative probability function is out of range.");
				Assert.AreEqual(Cycles, LastI, 1e-6, "Cumulative probability function does not reach 1.0 at end of interval.");
			}
		}
	}
}
