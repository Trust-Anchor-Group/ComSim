using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel.Activities;
using TAG.Simulator.ObjectModel.Values;
using Waher.Content.Xml;
using Waher.Script;

namespace TAG.Simulator.Performance.Values
{
	/// <summary>
	/// Value of a performance counter
	/// </summary>
	public class Performance : Value
	{
		private PerformanceCounterCategory performanceCategory;
		private PerformanceCounter performanceCounter;
		private string category;
		private string instance;
		private string counter;
		private double? multiplier;
		private double? divider;

		/// <summary>
		/// Value of a performance counter
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <param name="Model">Model in which the node is defined.</param>
		public Performance(ISimulationNode Parent, Model Model)
			: base(Parent, Model)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "Performance";

		/// <summary>
		/// XML Namespace where the element is defined.
		/// </summary>
		public override string Namespace => "http://trustanchorgroup.com/Schema/ComSim/ComSimPerformance.xsd";

		/// <summary>
		/// Points to the embedded XML Schema resource defining the semantics of the XML namespace.
		/// </summary>
		public override string SchemaResource => "TAG.Simulator.Performance.Schema.ComSimPerformance.xsd";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new Performance(Parent, Model);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.category = XML.Attribute(Definition, "category");
			this.instance = XML.Attribute(Definition, "instance");
			this.counter = XML.Attribute(Definition, "counter");

			if (Definition.HasAttribute("multiplier"))
				this.multiplier = XML.Attribute(Definition, "multiplier", 1.0);
			else
				this.multiplier = null;

			if (Definition.HasAttribute("divider"))
				this.divider = XML.Attribute(Definition, "divider", 1.0);
			else
				this.divider = null;

			return Task.CompletedTask;
		}

		/// <summary>
		/// Initialized the node before simulation.
		/// </summary>
		public override Task Initialize()
		{
			foreach (PerformanceCounterCategory Category in categories)
			{
				if (Category.CategoryName == this.category)
				{
					this.performanceCategory = Category;
					break;
				}
			}

			if (this.performanceCategory is null)
				throw new Exception("Performance category not found: " + this.category);

			PerformanceCounter[] Counters;

			if (string.IsNullOrEmpty(this.instance))
				Counters = this.performanceCategory.GetCounters();
			else
			{
				if (!string.IsNullOrEmpty(this.instance) && !this.performanceCategory.InstanceExists(this.instance))
					throw new Exception("Performance category " + this.category + " does not have an instance named " + this.instance + ".");

				Counters = this.performanceCategory.GetCounters(this.instance);
			}

			foreach (PerformanceCounter Counter in Counters)
			{
				if (Counter.CounterName == this.counter)
				{
					this.performanceCounter = Counter;
					break;
				}
			}

			if (this.performanceCounter is null)
				throw new Exception("Performance counter not found: " + this.counter);

			return base.Initialize();
		}

		private static readonly PerformanceCounterCategory[] categories = PerformanceCounterCategory.GetCategories();

		/// <summary>
		/// Evaluates the value.
		/// </summary>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>Evaluated value.</returns>
		public override object Evaluate(Variables Variables)
		{
			double Result = this.performanceCounter.NextValue();

			if (this.multiplier.HasValue)
				Result *= this.multiplier.Value;

			if (this.divider.HasValue)
				Result /= this.divider.Value;

			return Result;
		}

		/// <summary>
		/// Exports PlantUML
		/// </summary>
		/// <param name="Output">Output</param>
		/// <param name="Indentation">Number of tabs to indent.</param>
		/// <param name="QuoteChar">Quote character.</param>
		public override void ExportPlantUml(StreamWriter Output, int Indentation, char QuoteChar)
		{
			Output.Write("Performance(");
			Output.Write(this.category.Replace('"', QuoteChar));
			Output.WriteLine(',');

			Indent(Output, Indentation + 1);

			if (!string.IsNullOrEmpty(this.instance))
			{
				Output.Write(this.instance.Replace('"', QuoteChar));
				Output.WriteLine(',');

				Indent(Output, Indentation + 1);
			}

			Output.Write(this.counter.Replace('"', QuoteChar));
			Output.Write(')');
		}

	}
}
