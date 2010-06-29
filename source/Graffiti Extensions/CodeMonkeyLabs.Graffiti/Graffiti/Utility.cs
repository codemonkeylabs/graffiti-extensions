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
using System.Collections.Specialized;
using System.Text;
using System.Web;

namespace CodeMonkeyLabs.Graffiti
{
	/// <summary>
	/// Utilities of great use.
	/// </summary>
	internal static class Utility
	{
		/// <summary>
		/// Gets the 'created by...' tagline.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <returns>The 'created by...' tagline.</returns>
		public static string CreatedBy(string source)
		{
			return String.Format("Created by <a href=\"{0}\">Code Monkey Labs</a>.", TrackingUrlWithVersion(source));
		}

		/// <summary>
		/// Gets the current version of the Code Monkey Labs Graffiti Extensions.
		/// </summary>
		/// <value>The current version.</value>
		public static Version CurrentVersion
		{
			get { return typeof (Utility).Assembly.GetName().Version; }
		}

		/// <summary>
		/// Determines if the current request is at the specified URL.
		/// </summary>
		/// <param name="url">The app relative URL.</param>
		/// <returns>true if the current request is at the specified URL, otherwise false.</returns>
		public static bool AmIHere(string url)
		{
			if (VirtualPathUtility.IsAbsolute(url))
				url = VirtualPathUtility.ToAppRelative(url);

			return HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.Equals(url, StringComparison.InvariantCultureIgnoreCase);
		}

		public static string FullUrl(string url)
		{
			if (VirtualPathUtility.IsAppRelative(url))
				url = VirtualPathUtility.ToAbsolute(url);

			return new Uri(HttpContext.Current.Request.Url, url).ToString();
		}

		/// <summary>
		/// Generates a tracking URL back to Code Monkey Labs.
		/// </summary>
		/// <param name="parameters">The parameters.</param>
		/// <returns>A tracking URL back to Code Monkey Labs</returns>
		public static string TrackingUrl(NameValueCollection parameters)
		{
			var url = new StringBuilder();
			foreach (string key in parameters.AllKeys)
			{
				if (url.Length == 0)
					url.Append("?");
				else
					url.Append("&");

				url.AppendFormat("{0}={1}", HttpContext.Current.Server.UrlEncode(key),
				                 HttpContext.Current.Server.UrlEncode(parameters[key]));
			}

			url.Insert(0, "http://codemonkeylabs.com/");

			return url.ToString();
		}

		/// <summary>
		/// Generates a tracking URL back to Code Monkey Labs with a version.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <returns>A tracking URL back to Code Monkey Labs with a version.</returns>
		public static string TrackingUrlWithVersion(string source)
		{
			return TrackingUrl(new NameValueCollection
           	{
           		{"source", source},
           		{"version", CurrentVersion.ToString()}
           	});
		}
	}
}