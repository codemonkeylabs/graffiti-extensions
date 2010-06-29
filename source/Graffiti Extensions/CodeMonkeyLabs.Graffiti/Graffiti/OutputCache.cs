//	Copyright(c) 2009 Code Monkey Labs - http://codemonkeylabs.com/
//
//	Licensed under the Apache License, Version 2.0 (the "License"); 
//	you may not use this file except in compliance with the License. 
//	You may obtain a copy of the License at 
//
//	http://www.apache.org/licenses/LICENSE-2.0 
//
//	Unless required by applicable law or agreed to in writing, software 
//	distributed under the License is distributed on an "AS IS" BASIS, 
//	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
//	See the License for the specific language governing permissions and 
//	limitations under the License. 

using System;
using System.Web;
using Graffiti.Core;

namespace CodeMonkeyLabs.Graffiti
{
	/// <summary>
	/// Output caches content.
	/// </summary>
	[Serializable]
	public class OutputCache : GraffitiEvent
	{
		/// <summary>
		/// Gets the description.
		/// </summary>
		/// <value>The description.</value>
		public override string Description
		{
			get { return "Output caches content for blazing performance. " + Utility.CreatedBy("outputcache"); }
		}

		/// <summary>
		/// Gets a value indicating whether this instance is editable.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is editable; otherwise, <c>false</c>.
		/// </value>
		public override bool IsEditable
		{
			get { return false; }
		}

		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>The name.</value>
		public override string Name
		{
			get { return "Output Cache"; }
		}

		/// <summary>
		/// Initializes this module.
		/// </summary>
		/// <param name="ga">The Graffiti application.</param>
		public override void Init(GraffitiApplication ga)
		{
			ga.RenderHtmlHeader += this.OnPreSendRequestHeaders;
		}

		/// <summary>
		/// Pre send request headers handler.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void OnPreSendRequestHeaders(object sender, EventArgs e)
		{
			if (HttpContext.Current == null)
				return;

			if (GraffitiContext.Current == null)
				return;

			var where = GraffitiContext.Current["where"] as string;
			if (String.IsNullOrEmpty(where))
				return;

			if (where.Equals("home") || where.Equals("post") || where.Equals("category") || where.Equals("tag"))
			{
				DateTime timestamp = HttpContext.Current.Timestamp;
				TimeSpan duration = TimeSpan.FromSeconds(30);

				HttpCachePolicy cachePolicy = HttpContext.Current.Response.Cache;
				cachePolicy.SetCacheability(HttpCacheability.Public);
				cachePolicy.SetExpires(timestamp.Add(duration));
				cachePolicy.SetMaxAge(duration);
				cachePolicy.SetValidUntilExpires(true);
				cachePolicy.SetLastModified(timestamp);
			}
		}
	}
}