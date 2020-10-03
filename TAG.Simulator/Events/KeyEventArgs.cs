using System;
using System.Collections.Generic;
using System.Text;

namespace TAG.Simulator.Events
{
	/// <summary>
	/// Delegate for key event handlers.
	/// </summary>
	/// <param name="Sender">Sender of event.</param>
	/// <param name="e">Event arguments.</param>
	public delegate void KeyEventHandler(object Sender, KeyEventArgs e);

	/// <summary>
	/// Event arguments for key events.
	/// </summary>
	public class KeyEventArgs : EventArgs
	{
		private readonly string name;
		private readonly string lookupValue;
		private string value;

		/// <summary>
		/// Event arguments for key events.
		/// </summary>
		/// <param name="Name">Key name.</param>
		/// <param name="LookupValue">Lookup value.</param>
		public KeyEventArgs(string Name, string LookupValue)
		{
			this.name = Name;
			this.lookupValue = LookupValue;
			this.value = null;
		}

		/// <summary>
		/// Key name
		/// </summary>
		public string Name => this.name;

		/// <summary>
		/// Key lookup value.
		/// </summary>
		public string LookupValue => this.lookupValue;

		/// <summary>
		/// Key value
		/// </summary>
		public string Value
		{
			get => this.value;
			set => this.value = value;
		}
	}
}
