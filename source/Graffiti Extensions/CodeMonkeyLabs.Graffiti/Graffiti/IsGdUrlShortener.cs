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
using System.IO;
using System.Net;
using System.Web;
using Graffiti.Core;

namespace CodeMonkeyLabs.Graffiti
{
	/// <summary>
	/// Uses http://is.gd/ to shorten URLs.
	/// </summary>
	public class IsGdUrlShortener : IUrlShortener
	{
		/// <summary>
		/// Shortens the specified URL.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <returns>The shortened URL.</returns>
		public string Shorten(string url)
		{
			if (String.IsNullOrEmpty(url))
				throw new ArgumentNullException("url");

			// Documentation: http://is.gd/api_info.php
			HttpWebRequest request = GRequest.CreateRequest(String.Format("http://is.gd/api.php?longurl={0}", HttpUtility.UrlEncode(url)));
			try
			{
				// If everything works, the shortened URL is the response text.
				HttpWebResponse response = (HttpWebResponse) request.GetResponse();
				using (StreamReader responseReader = new StreamReader(response.GetResponseStream()))
					return responseReader.ReadToEnd();
			}
			catch (WebException ex)
			{
				// If there was any issue, a 500 error is returned and the error is the response text.
				using (StreamReader responseReader = new StreamReader(ex.Response.GetResponseStream()))
					throw new ApplicationException(responseReader.ReadToEnd());
			}
		}
	}
}