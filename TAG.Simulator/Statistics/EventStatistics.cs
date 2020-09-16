using System;
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
		private readonly Bucket bucketDebug;
		private readonly Bucket bucketInformational;
		private readonly Bucket bucketNotice;
		private readonly Bucket bucketWarning;
		private readonly Bucket bucketError;
		private readonly Bucket bucketCritical;
		private readonly Bucket bucketAlert;
		private readonly Bucket bucketEmergency;

		/// <summary>
		/// Basic event statistics
		/// </summary>
		/// <param name="StartTime">Starting time</param>
		/// <param name="BucketTime">Duration of one bucket, where statistics is collected.</param>
		public EventStatistics(DateTime StartTime, Duration BucketTime)
			: base("Event Statistics")
		{
			this.bucketDebug = new Bucket("Debug", false, StartTime, BucketTime);
			this.bucketInformational = new Bucket("Informational", false, StartTime, BucketTime);
			this.bucketNotice = new Bucket("Notice", false, StartTime, BucketTime);
			this.bucketWarning = new Bucket("Warning", false, StartTime, BucketTime);
			this.bucketError = new Bucket("Error", false, StartTime, BucketTime);
			this.bucketCritical = new Bucket("Critical", false, StartTime, BucketTime);
			this.bucketAlert = new Bucket("Alert", false, StartTime, BucketTime);
			this.bucketEmergency = new Bucket("Emergency", false, StartTime, BucketTime);
		}

		/// <summary>
		/// Processes an event.
		/// </summary>
		/// <param name="Event">Event</param>
		public override Task Queue(Event Event)
		{
			switch (Event.Type)
			{
				case EventType.Debug: this.bucketDebug.Inc(); break;
				case EventType.Informational: this.bucketInformational.Inc(); break;
				case EventType.Notice: this.bucketNotice.Inc(); break;
				case EventType.Warning: this.bucketWarning.Inc(); break;
				case EventType.Error: this.bucketError.Inc(); break;
				case EventType.Critical: this.bucketCritical.Inc(); break;
				case EventType.Alert: this.bucketAlert.Inc(); break;
				case EventType.Emergency: this.bucketEmergency.Inc(); break;
			}

			return Task.CompletedTask;
		}

		/// <summary>
		/// Exports accumulated statistics to markdown
		/// </summary>
		/// <param name="Output">Markdown output</param>
		public void ExportMarkdown(StreamWriter Output)
		{
			CountTable Table = this.GetTable();

			Output.WriteLine("Events");
			Output.WriteLine("=========");
			Output.WriteLine();

			//Table.ExportTableMarkdown(Output, "Type", "Total event counts", "TotalEventCounts");
			Table.ExportTableGraph(Output, "Total event counts");
		}

		private CountTable GetTable()
		{
			CountTable Table = new CountTable();

			Table.Add("Debug", this.bucketDebug.Count);
			Table.Add("Informational", this.bucketInformational.Count);
			Table.Add("Notice", this.bucketNotice.Count);
			Table.Add("Warning", this.bucketWarning.Count);
			Table.Add("Error", this.bucketError.Count);
			Table.Add("Critical", this.bucketCritical.Count);
			Table.Add("Alert", this.bucketAlert.Count);
			Table.Add("Emergency", this.bucketEmergency.Count);

			Table.SetBgColor("Debug", "DarkBlue");
			Table.SetFgColor("Debug", "White");

			Table.SetBgColor("Informational", "WhiteSmoke");
			Table.SetFgColor("Informational", "Black");

			Table.SetBgColor("Notice", "LightYellow");
			Table.SetFgColor("Notice", "Black");

			Table.SetBgColor("Warning", "Yellow");
			Table.SetFgColor("Warning", "Black");

			Table.SetBgColor("Error", "Red");
			Table.SetFgColor("Error", "Yellow");

			Table.SetBgColor("Critical", "DarkRed");
			Table.SetFgColor("Critical", "White");

			Table.SetBgColor("Alert", "Purple");
			Table.SetFgColor("Alert", "White");

			Table.SetBgColor("Emergency", "Black");
			Table.SetFgColor("Emergency", "White");

			return Table;
		}

		/// <summary>
		/// Exports accumulated statistics to XML
		/// </summary>
		/// <param name="Output">XML output</param>
		public void ExportXml(XmlWriter Output)
		{
			CountTable Table = this.GetTable();
			Table.ExportXml(Output, "Events", "EventType");
		}

	}
}
