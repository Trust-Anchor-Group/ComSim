using System.Collections.Generic;
using System.Threading.Tasks;
using TAG.Simulator.Web.Actors;
using Waher.Content;

namespace TAG.Simulator.Web.Activities
{
	/// <summary>
	/// Performs a TRACE request.
	/// </summary>
	/// <param name="Parent">Parent node</param>
	/// <param name="Model">Model in which the node is defined.</param>
	public class TRACE(ISimulationNode Parent, Model Model)
		: WebCall(Parent, Model)
	{
		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => nameof(TRACE);

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <param name="Model">Model in which the node is defined.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent, Model Model)
		{
			return new TRACE(Parent, Model);
		}

		/// <summary>
		/// Calls the web method.
		/// </summary>
		/// <param name="Client">Client performing the call.</param>
		/// <param name="Url">URL</param>
		/// <param name="Content">Content payload, if any.</param>
		/// <param name="Timeout">Duration before a request times out.</param>
		/// <param name="Headers">HTTP Headers.</param>
		/// <returns>Response.</returns>
		public override Task<ContentResponse> CallMethod(CookieWebClient Client, string Url, 
			byte[] Content, Duration Timeout, KeyValuePair<string, string>[] Headers)
		{
			return Client.TRACE(Url, Timeout, Headers);
		}
	}
}
