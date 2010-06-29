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
	/// The base twitter formatting strategy.
	/// </summary>
	public class DefaultTwitterFormattingStrategy : ITwitterFormattingStrategy
	{
		/// <summary>
		/// Formats the post into 140 characters or less.
		/// </summary>
		/// <param name="post">The post.</param>
		/// <param name="prefix">The prefix for the twitter post.</param>
		/// <returns>140 characters or less to link to the post.</returns>
		/// <remarks>
		/// Classes implementing this function do not necessarily need to stick to
		/// 140 characters or less - if more than 140 characters are returned the
		/// output will not be used.
		/// </remarks>
		public virtual string Format(Post post, string prefix)
		{
			StringBuilder text = new StringBuilder(FormatPrefix(post, prefix));

			text.AppendFormat(" {0} {1}", FormatTitle(post), FormatUrl(post));

			string tags = FormatTags(post);
			if (!String.IsNullOrEmpty(tags))
				text.AppendFormat(" {0}", tags);

			return text.ToString().Trim();
		}

		/// <summary>
		/// Formats the prefix.
		/// </summary>
		/// <param name="post">The post.</param>
		/// <param name="prefix">The prefix.</param>
		/// <returns>The prefix.</returns>
		protected virtual string FormatPrefix(Post post, string prefix)
		{
			if (String.IsNullOrEmpty(prefix))
				return String.Empty;

			return String.Format("{0}:", prefix.Replace(":", ""));
		}

		/// <summary>
		/// Formats the tags.
		/// </summary>
		/// <param name="post">The post.</param>
		/// <returns>The post tags.</returns>
		protected virtual string FormatTags(Post post)
		{
			StringBuilder tags = new StringBuilder();
			foreach (string tag in post.TagList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
			{
				if (tags.Length != 0)
					tags.Append(" ");

				tags.AppendFormat("#{0}", tag);
			}

			return tags.ToString();
		}

		/// <summary>
		/// Formats the title.
		/// </summary>
		/// <param name="post">The post.</param>
		/// <returns>The post title.</returns>
		protected virtual string FormatTitle(Post post)
		{
			return post.Title;
		}

		/// <summary>
		/// Shrinks the URL.
		/// </summary>
		/// <param name="post">The post.</param>
		/// <returns>The shortened URL.</returns>
		protected virtual string FormatUrl(Post post)
		{
			return new Macros().FullUrl(post.Url);
		}
	}
}