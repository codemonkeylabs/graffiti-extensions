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
	public class SmugMugTests
	{
		private readonly SmugMugWidget smugMugWidget;

		public SmugMugTests()
		{
			this.smugMugWidget = new SmugMugWidget();
		}

		[Theory]
		[InlineData("http://nickname.smugmug.com/photos/444398058_xXe2Z-Th.jpg")]
		public void ImagesUrlsReturnThemselves(string imageUrl)
		{
			string imageUrl2 = smugMugWidget.FixVideoUrl(imageUrl);

			Assert.Equal(imageUrl, imageUrl2);
		}

		[Theory]
		[InlineData("http://nickname.smugmug.com/photos/444398058_xXe2Z-640.mp4")]
		public void VideoUrlsReturnThumbnailUrls(string videoUrl)
		{
			string imageUrl = smugMugWidget.FixVideoUrl(videoUrl);

			Assert.NotEqual(videoUrl, imageUrl);
		}
	}
}