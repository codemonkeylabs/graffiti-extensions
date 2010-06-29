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
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Web;
using Graffiti.Core;

namespace CodeMonkeyLabs.Graffiti
{
	/// <summary>
	/// Gets users' gravatars from http://www.gravatar.com/.
	/// </summary>
	[Chalk("gravatar")]
	public class Gravatar
	{
		private static readonly string GravatarUrlFormat = "http://www.gravatar.com/avatar/{0}?s={1}&r={2}&d={3}";
		private static readonly string GravatarUrlFormatSsl = "https://secure.gravatar.com/avatar/{0}?s={1}&r={2}&d={3}";
		private static readonly MD5CryptoServiceProvider Md5 = new MD5CryptoServiceProvider();
		private static readonly List<string> AllowedIcons = new List<string> {"", "identicon", "monsterid", "wavatar"};
		private static readonly List<string> AllowedRatings = new List<string> { "", "g", "pg", "r", "x" };

		/// <summary>
		/// Generates the gravatar URL for the specified email.
		/// </summary>
		/// <param name="email">The email.</param>
		/// <returns></returns>
		public string Url(string email)
		{
			return Url(email, 80, null, null);
		}

		/// <summary>
		/// Generates the gravatar URL for the specified email.
		/// </summary>
		/// <param name="email">The email.</param>
		/// <param name="size">The size.</param>
		/// <returns></returns>
		public string Url(string email, int size)
		{
			return Url(email, size, null, null);
		}

		/// <summary>
		/// Generates the gravatar URL for the specified email.
		/// </summary>
		/// <param name="email">The email.</param>
		/// <param name="size">The size.</param>
		/// <param name="icon">The icon.</param>
		/// <returns></returns>
		public string Url(string email, int size, string icon)
		{
			return Url(email, size, icon, null);
		}

		/// <summary>
		/// Generates the gravatar URL for the specified email.
		/// </summary>
		/// <param name="email">The email.</param>
		/// <param name="size">The size.</param>
		/// <param name="icon">The icon.</param>
		/// <param name="rating">The rating.</param>
		/// <returns></returns>
		public string Url(string email, int size, string icon, string rating)
		{
			if (String.IsNullOrEmpty(email))
				throw new ArgumentNullException("email");

			if (size < 1 || size > 512)
				throw new ArgumentOutOfRangeException("size", "Gravatar size must be between 1 and 512.");

			if (icon != null && !AllowedIcons.Contains(icon.ToLower()))
				throw new ArgumentOutOfRangeException("icon", "Allowed icons are 'identicon', 'monsterid', & 'wavatar'.");

			if (rating != null && !AllowedRatings.Contains(rating.ToLower()))
				throw new ArgumentOutOfRangeException("rating", "Allowed ratings are 'g', 'pg', 'r', & 'x'.");

			var hashedEmail = HashEmail(email);

			var format = GravatarUrlFormat;
			if (HttpContext.Current.Request.IsSecureConnection)
				format = GravatarUrlFormatSsl;

			return String.Format(format, hashedEmail, size, rating, icon).ToLower();
		}

		/// <summary>
		/// Hashes the email.
		/// </summary>
		/// <param name="email">The email.</param>
		/// <returns>The lower-cased email's hash.</returns>
		protected internal string HashEmail(string email)
		{
			byte[] valueArray = System.Text.Encoding.ASCII.GetBytes(email.ToLower());
			valueArray = Md5.ComputeHash(valueArray);
			string hashed = "";
			for (int i = 0; i < valueArray.Length; i++)
				hashed += valueArray[i].ToString("x2").ToLower();
			return hashed;
		}
	}
}