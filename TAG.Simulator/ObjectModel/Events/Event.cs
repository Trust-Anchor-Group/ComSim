using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel.Activities;
using Waher.Content.Xml;
using Waher.Events;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Events
{
	/// <summary>
	/// Abstract base class for events
	/// </summary>
	public abstract class Event : SimulationNodeChildren, IEvent
	{
		private LinkedList<IEventPreparation> preparationNodes = null;
		private IActivity activity;
		private string activityId;
		private string id;

		/// <summary>
		/// Abstract base class for events
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public Event(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// ID of event.
		/// </summary>
		public string Id => this.id;

		/// <summary>
		/// ID of Activity to execute when event is triggered.
		/// </summary>
		public string ActivityId => this.activityId;

		/// <summary>
		/// Activity to execute when event is triggered.
		/// </summary>
		public IActivity Activity => this.activity;

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.id = XML.Attribute(Definition, "id");
			this.activityId = XML.Attribute(Definition, "activity");

			return base.FromXml(Definition);
		}

		/// <summary>
		/// Initialized the node before simulation.
		/// </summary>
		public override Task Initialize()
		{
			this.Model.Register(this);

			return base.Initialize();
		}

		/// <summary>
		/// Starts the node.
		/// </summary>
		public override Task Start()
		{
			if (!this.Model.TryGetActivity(this.activityId, out this.activity))
				throw new Exception("Activity not found: " + this.activityId);

			return base.Start();
		}

		/// <summary>
		/// Registers an event preparation node.
		/// </summary>
		/// <param name="PreparationNode">Preparation node.</param>
		public void Register(IEventPreparation PreparationNode)
		{
			if (this.preparationNodes is null)
				this.preparationNodes = new LinkedList<IEventPreparation>();

			this.preparationNodes.AddLast(PreparationNode);
		}

		/// <summary>
		/// Triggers the event.
		/// </summary>
		/// <param name="Variables">Event variables</param>
		public async void Trigger(Variables Variables)
		{
			KeyValuePair<string, object>[] Tags2 = null;
			DateTime Start = DateTime.Now;

			try
			{
				List<KeyValuePair<string, object>> Tags = new List<KeyValuePair<string, object>>();

				if (!(this.preparationNodes is null))
				{
					foreach (IEventPreparation Node in this.preparationNodes)
						Node.Prepare(Variables, Tags);
				}

				Tags2 = Tags.ToArray();

				try
				{
					this.Model.IncActivityStartCount(this.activityId, this.id, Tags2);
					await this.activity.ExecuteTask(Variables);
					this.Model.IncActivityFinishedCount(this.activityId, this.id, DateTime.Now - Start, Tags2);
				}
				finally
				{
					if (!(this.preparationNodes is null))
					{
						foreach (IEventPreparation Node in this.preparationNodes)
							Node.Release(Variables);
					}
				}
			}
			catch (Exception ex)
			{
				if (Tags2 is null)
					Tags2 = new KeyValuePair<string, object>[0];

				this.Model.IncActivityErrorCount(this.activityId, this.id, ex.Message, DateTime.Now - Start, Tags2);
			}
		}

	}
}
