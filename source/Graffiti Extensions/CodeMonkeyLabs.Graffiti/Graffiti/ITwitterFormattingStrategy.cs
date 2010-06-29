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

using Graffiti.Core;

namespace CodeMonkeyLabs.Graffiti
{
	/// <summary>
	/// A strategy for formatting posts for twitter.
	/// </summary>
	public interface ITwitterFormattingStrategy
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
		string Format(Post post, string prefix);
	}
}