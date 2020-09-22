using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.Statistics;
using Waher.Content;
using Waher.Content.Xml;
using Waher.Script.Objects;
using Waher.Script.Units;

namespace TAG.Simulator.ObjectModel.Graphs
{
	/// <summary>
	/// Defines a custom graph (for a counter, variable, sample, etc.)
	/// </summary>
	public class CustomGraph : Graph, IBucket
	{
		private readonly LinkedList<KeyValuePair<DateTime, double>> statistics = new LinkedList<KeyValuePair<DateTime, double>>();
		private string _for;
		private string script;
		private StringBuilder times = null;
		private StringBuilder values = null;
		private DateTime start;
		private DateTime stop;
		private Duration bucketTime;
		private Unit unit = null;
		private int count = 0;
		private long last = 0;

		/// <summary>
		/// Defines a custom graph (for a counter, variable, sample, etc.)
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public CustomGraph(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "CustomGraph";

		/// <summary>
		/// If the graph represents the visualization of a given entity. (Otherwise, null, or the empty string.)
		/// </summary>
		public override string For => this._for;

		/// <summary>
		/// Bucket ID
		/// </summary>
		public string Id => this.For;

		/// <summary>
		/// Total Counter
		/// </summary>
		public long TotalCount => this.count;

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new CustomGraph(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this._for = XML.Attribute(Definition, "for");

			if (Definition.HasAttribute("timeVariable"))
			{
				this.times = new StringBuilder();
				this.times.Append(Definition.GetAttribute("timeVariable"));
				this.times.Append(":=[");
			}

			this.values = new StringBuilder();

			if (Definition.HasAttribute("valueVariable"))
				this.values.Append(Definition.GetAttribute("valueVariable"));
			else
				this.values.Append(this._for);

			this.values.Append(":=[");

			this.script = Values.Script.RemoveIndent(Definition.InnerText);

			return Task.CompletedTask;
		}

		/// <summary>
		/// Starts the node.
		/// </summary>
		public override Task Start()
		{
			this.start = this.Model.StartTime;
			this.bucketTime = this.Model.BucketTime;
			this.stop = this.start + this.bucketTime;

			return base.Start();
		}

		/// <summary>
		/// Increments counter.
		/// </summary>
		/// <returns>Start time of bucket that was incremented.</returns>
		public DateTime Inc()
		{
			double v;

			lock (this)
			{
				v = ++this.last;
			}

			return this.Sample(v);
		}

		/// <summary>
		/// Decrements counter.
		/// </summary>
		/// <returns>Start time of bucket that was incremented.</returns>
		public DateTime Dec()
		{
			double v;

			lock (this)
			{
				v = --this.last;
			}

			return this.Sample(v);
		}

		/// <summary>
		/// Adds a sample
		/// </summary>
		/// <param name="Value">Sample value reported</param>
		/// <returns>Start time of bucket to which the value was reported.</returns>
		public DateTime Sample(PhysicalQuantity Value)
		{
			double v;

			if (this.unit is null)
			{
				this.unit = Value.Unit;
				v = Value.Magnitude;
			}
			else if (!Unit.TryConvert(Value.Magnitude, Value.Unit, this.unit, out v))
				throw new Exception("Incompatible units: " + Value.Unit.ToString() + " and " + this.unit.ToString());

			return this.Sample(v);
		}

		/// <summary>
		/// Adds a sample
		/// </summary>
		/// <param name="Value">Sample value reported</param>
		/// <returns>Start time of bucket to which the value was reported.</returns>
		public DateTime Sample(double Value)
		{
			lock (this)
			{
				DateTime Now = DateTime.Now;

				while (Now >= this.stop)
				{
					this.start = this.stop;
					this.stop = this.start + this.bucketTime;
				}

				if (!(this.times is null))
				{
					if (this.count > 0)
						this.times.Append(',');

					this.times.Append(CommonTypes.Encode(this.Model.GetTimeCoordinates(Now)));
				}

				if (!(this.values is null))
				{
					if (this.count > 0)
						this.values.Append(',');

					this.values.Append(CommonTypes.Encode(Value));
				}

				this.statistics.AddLast(new KeyValuePair<DateTime, double>(Now, Value));
				this.count++;

				return this.start;
			}
		}

		/// <summary>
		/// Counts one occurrence
		/// </summary>
		/// <returns>Start time of bucket to which the value was reported.</returns>
		public DateTime CountOccurrence()
		{
			double v;

			lock (this)
			{
				v = ++this.last;
			}

			return this.Sample(v);
		}

		/// <summary>
		/// Terminates the ongoing collection of data.
		/// </summary>
		public void Flush()
		{
		}

		/// <summary>
		/// Exports historical counts as a graph.
		/// </summary>
		/// <param name="Title">Title of graph.</param>
		/// <param name="LabelY">Label for Y-axis.</param>
		/// <param name="Output">Export destination</param>
		/// <param name="Model">Simulation model</param>
		public void ExportSampleHistoryGraph(string Title, string LabelY, StreamWriter Output, Model Model)
		{
			this.ExportSampleHistoryGraph(Output);
		}

		/// <summary>
		/// Exports data to XML
		/// </summary>
		/// <param name="Output">XML Output</param>
		/// <param name="RowElement">XML Row element name.</param>
		public void ExportXml(XmlWriter Output, string RowElement)
		{
			this.Flush();

			lock (this)
			{
				Output.WriteStartElement(RowElement);
				Output.WriteAttributeString("type", this._for);
				Output.WriteAttributeString("count", this.count.ToString());

				if (!(this.unit is null))
					Output.WriteAttributeString("unit", this.unit.ToString());

				foreach (KeyValuePair<DateTime, double> Rec in this.statistics)
				{
					Output.WriteStartElement("Stat");
					Output.WriteAttributeString("ts", XML.Encode(Rec.Key));
					Output.WriteAttributeString("value", CommonTypes.Encode(Rec.Value));
					Output.WriteEndElement();
				}

				Output.WriteEndElement();
			}
		}

		/// <summary>
		/// Exports the graph to a markdown output.
		/// </summary>
		/// <param name="Output">Markdown output</param>
		public override void ExportSampleHistoryGraph(StreamWriter Output)
		{
			this.Flush();

			lock (this)
			{
				Output.WriteLine("{");

				if (this.count > 0)
				{
					if (!(this.times is null))
					{
						Output.Write(this.times.ToString());
						Output.WriteLine("];");
					}

					if (!(this.values is null))
					{
						Output.Write(this.values.ToString());
						Output.WriteLine("];");
					}
				}

				Output.WriteLine("GraphWidth:=1000;");
				Output.WriteLine("GraphHeight:=400;");
				Output.WriteLine(this.script);
				Output.WriteLine("}");
				Output.WriteLine();
			}
		}
	}
}
