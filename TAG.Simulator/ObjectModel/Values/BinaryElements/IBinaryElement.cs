﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Values.BinaryElements
{
	/// <summary>
	/// Interface for binary elements
	/// </summary>
	public interface IBinaryElement : ISimulationNode
	{
		/// <summary>
		/// Appends the binary element to the output stream.
		/// </summary>
		/// <param name="Output">Output stream.</param>
		/// <param name="Variables">Set of variables for the activity.</param>
		void Append(MemoryStream Output, Variables Variables);
	}
}
