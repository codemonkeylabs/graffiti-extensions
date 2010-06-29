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
using System.Net;
using System.Text;
using System.Web;
using Graffiti.Core;

namespace CodeMonkeyLabs.Graffiti
{
	/// <summary>
	/// A simple Twitter client.
	/// </summary>
	public static class TwitterClient
	{
		/// <summary>
		/// Updates your status
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="password">The password.</param>
		/// <param name="status">The status.</param>
		public static void UpdateStatus(string username, string password, string status)
		{
			if (String.IsNullOrEmpty(username))
				throw new ArgumentNullException("username");

			if (String.IsNullOrEmpty(password))
				throw new ArgumentNullException("password");

			if (String.IsNullOrEmpty(status))
				throw new ArgumentNullException("status");

			if (status.Length > 140)
				throw new ArgumentException("Status must be 140 characters or less.", "status");

			status = HttpUtility.UrlEncode(status, Encoding.UTF8);

			HttpWebRequest request = GRequest.CreateRequest(String.Format("http://twitter.com/statuses/update.xml?status={0}", status));
			request.Credentials = new NetworkCredential(username, password);
			request.Method = "POST";

			// Get the response
			using (var response = (HttpWebResponse) request.GetResponse())
			{
			}
		}

		/// <summary>
		/// Validates the credentials.
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="password">The password.</param>
		/// <returns>True if the credentials are valid, otherwise false.</returns>
		public static bool ValidateCredentials(string username, string password)
		{
			if (String.IsNullOrEmpty(username))
				throw new ArgumentNullException("username");

			if (String.IsNullOrEmpty(password))
				throw new ArgumentNullException("password");

			HttpStatusCode statusCode = HttpStatusCode.Unauthorized;
			try
			{
				HttpWebRequest request = GRequest.CreateRequest("http://twitter.com/account/verify_credentials.xml");
				request.Credentials = new NetworkCredential(username, password);
				using (var response = (HttpWebResponse) request.GetResponse())
				{
					statusCode = response.StatusCode;
				}
			}
			catch (WebException ex)
			{
				statusCode = ((HttpWebResponse) ex.Response).StatusCode;
			}

			return statusCode == HttpStatusCode.OK;
		}
	}
}