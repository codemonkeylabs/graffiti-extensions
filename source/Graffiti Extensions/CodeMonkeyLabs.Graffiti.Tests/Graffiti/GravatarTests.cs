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
using Xunit;
using Xunit.Extensions;

namespace CodeMonkeyLabs.Graffiti
{
	public class GravatarTests
	{
		private readonly Gravatar gravatar;

		public GravatarTests()
		{
			this.gravatar = new Gravatar();
		}

		[Theory]
		[InlineData("iHaveAn@email.com", "3b3be63a4c2a439b013787725dfce802")]
		public void EmailHashesCorrectly(string email, string hash)
		{
			Assert.Equal(this.gravatar.HashEmail(email), hash);
		}

		[Theory]
		[InlineData("")]
		[InlineData((string)null)]
		public void InvalidEmailThrowsException(string email)
		{
			Assert.Throws<ArgumentNullException>(() => this.gravatar.Url(email));
		}

		[Theory]
		[InlineData("icon")]
		public void InvalidIconThrowsException(string icon)
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => this.gravatar.Url("dummy@email.com", 80, icon));
		}

		[Theory]
		[InlineData("rating")]
		public void InvalidRatingThrowsException(string rating)
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => this.gravatar.Url("dummy@email.com", 80, null, rating));
		}

		[Theory]
		[InlineData(0)]
		[InlineData(513)]
		public void InvalidSizeThrowsException(int size)
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => this.gravatar.Url("dummy@email.com", size));
		}
	}
}