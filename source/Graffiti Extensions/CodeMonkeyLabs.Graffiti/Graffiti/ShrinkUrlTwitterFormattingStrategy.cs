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
using System.Text;
using Graffiti.Core;

namespace CodeMonkeyLabs.Graffiti
{
	/// <summary>
	/// Shortens the URL only.
	/// </summary>
	public class ShrinkUrlTwitterFormattingStrategy : DefaultTwitterFormattingStrategy
	{
		private readonly IUrlShortener urlShortener;

		/// <summary>
		/// Initializes a new instance of the <see cref="ShrinkUrlTwitterFormattingStrategy"/> class.
		/// </summary>
		/// <param name="urlShortener">The URL shortener.</param>
		public ShrinkUrlTwitterFormattingStrategy(IUrlShortener urlShortener)
		{
			if (urlShortener == null)
				throw new ArgumentNullException("urlShortener");

			this.urlShortener = urlShortener;
		}

		/// <summary>
		/// Shrinks the URL.
		/// </summary>
		/// <param name="post">The post.</param>
		/// <returns>The shortened URL.</returns>
		protected override string FormatUrl(Post post)
		{
			return this.urlShortener.Shorten(base.FormatUrl(post));
		}
	}
}