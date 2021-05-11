using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using SkiaSharp;
using TAG.Simulator.ObjectModel;
using TAG.Simulator.ObjectModel.Activities;
using TAG.Simulator.ObjectModel.Values;
using Waher.Content;
using Waher.Content.Xml;
using Waher.Networking.XMPP;
using Waher.Networking.XMPP.Control;
using Waher.Script;
using Waher.Things;

namespace TAG.Simulator.XMPP.IoT.Activities
{
	/// <summary>
	/// Controls an actuator by setting control parameters.
	/// </summary>
	public class ControlActuator : XmppIoTActivityNode, IValueRecipient
	{
		private ThingReference[] nodes;
		private IValue value = null;
		private StringAttribute to;
		private StringAttribute parameter;
		private StringAttribute controller;

		/// <summary>
		/// Controls an actuator by setting control parameters.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public ControlActuator(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Value node
		/// </summary>
		public IValue Value => this.value;

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "ControlActuator";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new ControlActuator(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override async Task FromXml(XmlElement Definition)
		{
			this.controller = new StringAttribute(XML.Attribute(Definition, "controller"));
			this.to = new StringAttribute(XML.Attribute(Definition, "to"));
			this.parameter = new StringAttribute(XML.Attribute(Definition, "parameter"));

			await base.FromXml(Definition);

			List<ThingReference> Nodes = new List<ThingReference>();

			foreach (ISimulationNode Node in this.Children)
			{
				if (Node is NodeReference NodeRef)
					Nodes.Add(NodeRef.ThingReference);
			}

			this.nodes = Nodes.ToArray();
		}

		/// <summary>
		/// Registers a value for the argument.
		/// </summary>
		/// <param name="Value">Value node</param>
		public void Register(IValue Value)
		{
			if (this.value is null)
				this.value = Value;
			else
				throw new Exception("Value already defined.");
		}

		/// <summary>
		/// Executes a node.
		/// </summary>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>Next node of execution, if different from the default, otherwise null (for default).</returns>
		public override Task<LinkedListNode<IActivityNode>> Execute(Variables Variables)
		{
			string ControllerName = this.controller.GetValue(Variables);

			if (!Variables.TryGetVariable(ControllerName, out Waher.Script.Variable v))
				throw new Exception("Controller not found: " + ControllerName);

			object Obj = v.ValueObject;
			if (!(Obj is ControlClient Controller))
				throw new Exception("Not a control client object: " + ControllerName);

			string To = this.to.GetValue(Variables);
			string Parameter = this.parameter.GetValue(Variables);
			object Value = this.value.Evaluate(Variables);

			if (XmppClient.BareJidRegEx.IsMatch(To))
			{
				RosterItem Item = Controller.Client[To];
				if (Item is null)
					throw new Exception("No connection in roster with Bare JID: " + To);

				if (!Item.HasLastPresence || !Item.LastPresence.IsOnline)
					throw new Exception("Contact not online: " + To);

				To = Item.LastPresenceFullJid;
			}

			if (Value is bool b)
				Controller.Set(To, Parameter, b, this.nodes);
			else if (Value is double d)
				Controller.Set(To, Parameter, d, this.nodes);
			else if (Value is Enum e)
				Controller.Set(To, Parameter, e, this.nodes);
			else if (Value is int i)
				Controller.Set(To, Parameter, i, this.nodes);
			else if (Value is long l)
				Controller.Set(To, Parameter, l, this.nodes);
			else if (Value is string s)
				Controller.Set(To, Parameter, s, this.nodes);
			else if (Value is TimeSpan TS)
				Controller.Set(To, Parameter, TS, this.nodes);
			else if (Value is System.DateTime TP)
				Controller.Set(To, Parameter, TP, TP.TimeOfDay == TimeSpan.Zero, this.nodes);
			else if (Value is Waher.Content.Duration d2)
				Controller.Set(To, Parameter, d2, this.nodes);
			else if (Value is ColorReference cl)
				Controller.Set(To, Parameter, cl, this.nodes);
			else if (Value is SKColor cl2)
				Controller.Set(To, Parameter, new ColorReference(cl2.Red, cl2.Green, cl2.Red, cl2.Alpha), this.nodes);
			else
				throw new Exception("Unsupported control type: " + Value.GetType().FullName);

			return Task.FromResult<LinkedListNode<IActivityNode>>(null);
		}

		/// <summary>
		/// Exports PlantUML
		/// </summary>
		/// <param name="Output">Output</param>
		/// <param name="Indentation">Number of tabs to indent.</param>
		/// <param name="QuoteChar">Quote character.</param>
		public override void ExportPlantUml(StreamWriter Output, int Indentation, char QuoteChar)
		{
			Indent(Output, Indentation);
			Output.Write(":ControlActuator(");
			Output.Write(this.controller.Value);
			Output.Write(", ");
			Output.Write(this.to.Value);

			string s = NewMomentaryValues.Join(GetThingReferences(this.nodes));
			if (!string.IsNullOrEmpty(s))
			{
				Output.Write(", ");
				Output.Write(s);
			}

			Output.WriteLine(");");
		}

		/// <summary>
		/// Gets referenced node references
		/// </summary>
		/// <param name="Nodes">Node references</param>
		/// <returns>Node references</returns>
		public static string[] GetThingReferences(ThingReference[] Nodes)
		{
			SortedDictionary<string, bool> Sorted = new SortedDictionary<string, bool>();

			foreach (ThingReference Node in Nodes)
				Sorted[NewMomentaryValues.GetReference(Node)] = true;

			string[] Result = new string[Sorted.Count];
			Sorted.Keys.CopyTo(Result, 0);

			return Result;
		}

	}
}
