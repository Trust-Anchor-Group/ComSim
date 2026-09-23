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
		[DataRow("U0", 1,
			"<Model timeBase='StartOfSimulation' bucketTime='PT1S' timeUnit='PT1S' timeCycle='PT1M' duration='PT1M' sampleEpsilon='true'/>",
			"<Uniform N='200' from='0' to='60'/>")]
		[DataRow("U1", 1,
			"<Model timeBase='StartOfSimulation' bucketTime='PT1S' timeUnit='PT1S' timeCycle='PT1M' duration='PT1M' sampleEpsilon='true'/>",
			"<Uniform N='200' from='10' to='50'/>")]
		[DataRow("U2", 1,
			"<Model timeBase='StartOfSimulation' bucketTime='PT1S' timeUnit='PT1S' timeCycle='PT1M' duration='PT1M' sampleEpsilon='true'/>",
			"<Uniform N='200' from='50' to='10'/>")]
		[DataRow("U3", 3,
			"<Model timeBase='StartOfSimulation' bucketTime='PT1S' timeUnit='PT1S' timeCycle='PT1M' duration='PT3M' sampleEpsilon='true'/>",
			"<Uniform N='200' from='0' to='60'/>")]
		[DataRow("U4", 3,
			"<Model timeBase='StartOfSimulation' bucketTime='PT1S' timeUnit='PT1S' timeCycle='PT1M' duration='PT3M' sampleEpsilon='true'/>",
			"<Uniform N='200' from='10' to='50'/>")]
		[DataRow("U5", 3,
			"<Model timeBase='StartOfSimulation' bucketTime='PT1S' timeUnit='PT1S' timeCycle='PT1M' duration='PT3M' sampleEpsilon='true'/>",
			"<Uniform N='200' from='50' to='10'/>")]
		[DataRow("I0", 1,
			"<Model timeBase='StartOfSimulation' bucketTime='PT1S' timeUnit='PT1S' timeCycle='PT1M' duration='PT1M' sampleEpsilon='true'/>",
			"<LinearIncrease N='200' from='0' to='60'/>")]
		[DataRow("I1", 1,
			"<Model timeBase='StartOfSimulation' bucketTime='PT1S' timeUnit='PT1S' timeCycle='PT1M' duration='PT1M' sampleEpsilon='true'/>",
			"<LinearIncrease N='200' from='10' to='50'/>")]
		[DataRow("I2", 1,
			"<Model timeBase='StartOfSimulation' bucketTime='PT1S' timeUnit='PT1S' timeCycle='PT1M' duration='PT1M' sampleEpsilon='true'/>",
			"<LinearIncrease N='200' from='50' to='10'/>")]
		[DataRow("I3", 3,
			"<Model timeBase='StartOfSimulation' bucketTime='PT1S' timeUnit='PT1S' timeCycle='PT1M' duration='PT3M' sampleEpsilon='true'/>",
			"<LinearIncrease N='200' from='0' to='60'/>")]
		[DataRow("I4", 3,
			"<Model timeBase='StartOfSimulation' bucketTime='PT1S' timeUnit='PT1S' timeCycle='PT1M' duration='PT3M' sampleEpsilon='true'/>",
			"<LinearIncrease N='200' from='10' to='50'/>")]
		[DataRow("I5", 3,
			"<Model timeBase='StartOfSimulation' bucketTime='PT1S' timeUnit='PT1S' timeCycle='PT1M' duration='PT3M' sampleEpsilon='true'/>",
			"<LinearIncrease N='200' from='50' to='10'/>")]
		[DataRow("D0", 1,
			"<Model timeBase='StartOfSimulation' bucketTime='PT1S' timeUnit='PT1S' timeCycle='PT1M' duration='PT1M' sampleEpsilon='true'/>",
			"<LinearDecrease N='200' from='0' to='60'/>")]
		[DataRow("D1", 1,
			"<Model timeBase='StartOfSimulation' bucketTime='PT1S' timeUnit='PT1S' timeCycle='PT1M' duration='PT1M' sampleEpsilon='true'/>",
			"<LinearDecrease N='200' from='10' to='50'/>")]
		[DataRow("D2", 1,
			"<Model timeBase='StartOfSimulation' bucketTime='PT1S' timeUnit='PT1S' timeCycle='PT1M' duration='PT1M' sampleEpsilon='true'/>",
			"<LinearDecrease N='200' from='50' to='10'/>")]
		[DataRow("D3", 3,
			"<Model timeBase='StartOfSimulation' bucketTime='PT1S' timeUnit='PT1S' timeCycle='PT1M' duration='PT3M' sampleEpsilon='true'/>",
			"<LinearDecrease N='200' from='0' to='60'/>")]
		[DataRow("D4", 3,
			"<Model timeBase='StartOfSimulation' bucketTime='PT1S' timeUnit='PT1S' timeCycle='PT1M' duration='PT3M' sampleEpsilon='true'/>",
			"<LinearDecrease N='200' from='10' to='50'/>")]
		[DataRow("D5", 3,
			"<Model timeBase='StartOfSimulation' bucketTime='PT1S' timeUnit='PT1S' timeCycle='PT1M' duration='PT3M' sampleEpsilon='true'/>",
			"<LinearDecrease N='200' from='50' to='10'/>")]
		[DataRow("N0", 1,
			"<Model timeBase='StartOfSimulation' bucketTime='PT1S' timeUnit='PT1S' timeCycle='PT1M' duration='PT1M' sampleEpsilon='true'/>",
			"<Normal N='200' μ='30' σ='10'/>")]
		[DataRow("N1", 1,
			"<Model timeBase='StartOfSimulation' bucketTime='PT1S' timeUnit='PT1S' timeCycle='PT1M' duration='PT1M' sampleEpsilon='true'/>",
			"<Normal N='200' μ='50' σ='10'/>")]
		[DataRow("N2", 1,
			"<Model timeBase='StartOfSimulation' bucketTime='PT1S' timeUnit='PT1S' timeCycle='PT1M' duration='PT1M' sampleEpsilon='true'/>",
			"<Normal N='200' μ='10' σ='10'/>")]
		[DataRow("N3", 1,
			"<Model timeBase='StartOfSimulation' bucketTime='PT1S' timeUnit='PT1S' timeCycle='PT1M' duration='PT1M' sampleEpsilon='true'/>",
			"<Normal N='200' μ='30' σ='30'/>")]
		[DataRow("N4", 3,
			"<Model timeBase='StartOfSimulation' bucketTime='PT1S' timeUnit='PT1S' timeCycle='PT1M' duration='PT3M' sampleEpsilon='true'/>",
			"<Normal N='200' μ='30' σ='10'/>")]
		[DataRow("N5", 3,
			"<Model timeBase='StartOfSimulation' bucketTime='PT1S' timeUnit='PT1S' timeCycle='PT1M' duration='PT3M' sampleEpsilon='true'/>",
			"<Normal N='200' μ='50' σ='10'/>")]
		[DataRow("N6", 3,
			"<Model timeBase='StartOfSimulation' bucketTime='PT1S' timeUnit='PT1S' timeCycle='PT1M' duration='PT3M' sampleEpsilon='true'/>",
			"<Normal N='200' μ='10' σ='10'/>")]
		[DataRow("N7", 3,
			"<Model timeBase='StartOfSimulation' bucketTime='PT1S' timeUnit='PT1S' timeCycle='PT1M' duration='PT3M' sampleEpsilon='true'/>",
			"<Normal N='200' μ='30' σ='30'/>")]
		public async Task Test_01_DrawGraphs(string Id, int Cycles, string ModelXml, string DistributionXml)
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

					Timestamps.Add(TP);
					Cumulative.Add(I);

					TP = TP + Model.TimeUnit;

					AllIncreasing &= I >= LastI;
					InRange &= (I >= 0 && I <= Cycles) || TP > Model.EndTime;
					LastI = I;
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

				await File.WriteAllBytesAsync("Graphs\\I(P)_" + Id + ".png",
					Pixels.EncodeAsPng(), CancellationToken.None);

				StringBuilder sb = new();
				sb.Append("P:=");
				Distribution.ExportPdfBody(sb);
				sb.Append(";plot2dline(tx,zeroes(count(t)),'Black',1)+");
				sb.Append("plot2dline(tx,P,'Red',3)+");
				sb.Append("scatter2d(tx,P,'Blue',5)");

				G = (Graph)await Expression.EvalAsync(sb.ToString(), Variables);
				Pixels = G.CreatePixels(Settings);

				await File.WriteAllBytesAsync("Graphs\\PDF_" + Id + ".png",
					Pixels.EncodeAsPng(), CancellationToken.None);

				Assert.IsTrue(AllIncreasing, "Cumulative probability function is not increasing.");
				Assert.IsTrue(InRange, "Cumulative probability function is out of range.");

				I = Distribution!.GetCumulativeProbability(t2 - 1e-10, Cycles);
				Assert.AreEqual(Cycles, I, 1e-2, "Cumulative probability function does not reach " + Cycles + " at end of interval.");
			}
		}
	}
}
