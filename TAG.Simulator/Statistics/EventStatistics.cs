using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Waher.Content;
using Waher.Events;

namespace TAG.Simulator.Statistics
{
	/// <summary>
	/// Basic event statistics
	/// </summary>
	public class EventStatistics : EventSink
	{
		private readonly Buckets buckets;

		/// <summary>
		/// Basic event statistics
		/// </summary>
		/// <param name="StartTime">Starting time</param>
		/// <param name="BucketTime">Duration of one bucket, where statistics is collected.</param>
		/// <param name="Model">Simulation Model.</param>
		public EventStatistics(DateTime StartTime, Duration BucketTime, Model Model)
			: base("Event Statistics")
		{
			List<string> IDs = new List<string>();

			foreach (Enum T in Enum.GetValues(typeof(EventType)))
				IDs.Add(T.ToString());

			this.buckets = new Buckets(StartTime, BucketTime, "Total event counts", "Count (/ " + Model.BucketTimeStr + ")", Model, false, IDs.ToArray());
		}

		/// <summary>
		/// Processes an event.
		/// </summary>
		/// <param name="Event">Event</param>
		public override Task Queue(Event Event)
		{
			this.buckets.CountEvent(Event.Type.ToString());
			return Task.CompletedTask;
		}

		/// <summary>
		/// Exports accumulated statistics to markdown
		/// </summary>
		/// <param name="Output">Markdown output</param>
		/// <param name="Model">Simulation model</param>
		public void ExportMarkdown(StreamWriter Output, Model Model)
		{
			CountTable Table = this.GetTable(out string[] Order);

			Output.WriteLine("Events");
			Output.WriteLine("=========");
			Output.WriteLine();

			Table.ExportTableGraph(Output, "Total event counts");

			SKColor[] Palette = new SKColor[]
			{
				SKColors.DarkBlue,
				SKColors.LightGray,
				SKColors.LightYellow,
				SKColors.Yellow,
				SKColors.Red,
				SKColors.DarkRed,
				SKColors.Purple,
				SKColors.Black
			};

			this.buckets.ExportCountHistoryGraph("Events", Order, Output, Model, null, Palette);
		}

		private CountTable GetTable(out string[] Order)
		{
			Order = new string[]
			{
				EventType.Debug.ToString(),
				EventType.Informational.ToString(),
				EventType.Notice.ToString(),
				EventType.Warning.ToString(),
				EventType.Error.ToString(),
				EventType.Critical.ToString(),
				EventType.Alert.ToString(),
				EventType.Emergency.ToString()
			};
			
			CountTable Table = this.buckets.GetTotalCountTable(Order);

			Table.SetBgColor(Order[0], "DarkBlue");
			Table.SetFgColor(Order[0], "White");

			Table.SetBgColor(Order[1], "LightGray");
			Table.SetFgColor(Order[1], "Black");

			Table.SetBgColor(Order[2], "LightYellow");
			Table.SetFgColor(Order[2], "Black");

			Table.SetBgColor(Order[3], "Yellow");
			Table.SetFgColor(Order[3], "Black");

			Table.SetBgColor(Order[4], "Red");
			Table.SetFgColor(Order[4], "Yellow");

			Table.SetBgColor(Order[5], "DarkRed");
			Table.SetFgColor(Order[5], "White");

			Table.SetBgColor(Order[6], "Purple");
			Table.SetFgColor(Order[6], "White");

			Table.SetBgColor(Order[7], "Black");
			Table.SetFgColor(Order[7], "White");

			return Table;
		}

		/// <summary>
		/// Exports accumulated statistics to XML
		/// </summary>
		/// <param name="Output">XML output</param>
		public void ExportXml(XmlWriter Output)
		{
			this.buckets.ExportXml(Output, "Events", "EventType");
		}

	}
}
