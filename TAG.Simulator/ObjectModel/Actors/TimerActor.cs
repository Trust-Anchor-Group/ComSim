using System;
using System.IO;
using System.Threading;
using System.Xml;
using System.Threading.Tasks;
using Waher.Content;
using Waher.Content.Xml;

namespace TAG.Simulator.ObjectModel.Actors
{
	/// <summary>
	/// Represents an internal clock.
	/// </summary>
	public class TimerActor : Actor
	{
		private Duration period;
		private bool isPeriodic;
		private Timer timer;

		/// <summary>
		/// Represents an internal clock.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public TimerActor(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Represents an internal clock.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <param name="InstanceIndex">Instance index.</param>
		/// <param name="InstanceId">ID of instance</param>
		public TimerActor(ISimulationNode Parent, Model Model, int InstanceIndex, string InstanceId)
			: base(Parent, Model, InstanceIndex, InstanceId)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "Timer";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new TimerActor(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.period = XML.Attribute(Definition, "period", Duration.Zero);
			this.isPeriodic = this.period > Duration.Zero;

			return base.FromXml(Definition);
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
			return Task.FromResult<Actor>(new TimerActor(this, this.Model, InstanceIndex, InstanceId)
			{
				period = this.period,
				isPeriodic = this.isPeriodic
			});
		}

		/// <summary>
		/// Initializes an instance of an actor.
		/// </summary>
		public override Task InitializeInstance()
		{
			return Task.CompletedTask;
		}

		private void TimerCallback(object State)
		{
			this.Model.ExternalEvent(this, "Elapsed");
		}

		/// <summary>
		/// Starts an instance of an actor.
		/// </summary>
		public override Task StartInstance()
		{
			if (this.isPeriodic)
			{
				DateTime Now = DateTime.Now;
				DateTime TP = Now + this.period;
				TimeSpan TS = TP - Now;

				this.timer = new Timer(this.TimerCallback, null, TS, TS);
			}

			return Task.CompletedTask;
		}

		/// <summary>
		/// Finalizes an instance of an actor.
		/// </summary>
		public override Task FinalizeInstance()
		{
			this.timer?.Dispose();
			this.timer = null;

			return Task.CompletedTask;
		}

		/// <summary>
		/// Allows the actor to add notes related to the actor in use case diagrams.
		/// </summary>
		/// <param name="Output">Use Case diagram output.</param>
		/// <param name="Id">ID of actor in use case diagram.</param>
		public override void AnnotateActorUseCaseUml(StreamWriter Output, string Id)
		{
			if (this.isPeriodic)
			{
				Output.Write("note right of ");
				Output.Write(Id);
				Output.Write(" : Elapses periodically every ");

				Values.Duration.ExportText(this.period, Output);

				Output.WriteLine();
			}
		}

	}
}
