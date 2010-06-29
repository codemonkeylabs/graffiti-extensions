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
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Graffiti.Core;
using RssToolkit.Rss;

namespace CodeMonkeyLabs.Graffiti
{
	/// <summary>
	/// A SmugMug widget.
	/// </summary>
	[WidgetInfo("B6A6D44E-56FD-450c-AB1B-5D88233AA48C", "SmugMug Items", "Recent SmugMug photos.")]
	public class SmugMugWidget : WidgetFeed
	{
		private static readonly List<string> VideoExtensions = new List<string> { ".mp4" };
		private static readonly Regex SmugMugRegex = new Regex(@"^http(s)?://(?<nickname>\w+?)\.smugmug\.com/photos/(?<galleryId>\d+)_(?<photoId>\w+?)-(?<format>\w+?)\.(?<extension>\w+?)$");

		/// <summary>
		/// Gets the feed URL.
		/// </summary>
		/// <value>The feed URL.</value>
		public override string FeedUrl
		{
			get { return String.Format("http://www.smugmug.com/hack/feed.mg?Type=nicknameRecentPhotos&Data={0}&format=rss200", this.NickName); }
		}

		/// <summary>
		/// Gets or sets the number of items to display.
		/// </summary>
		/// <value>The number of items to display.</value>
		public int ItemsToDisplay { get; set; }

		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>The name.</value>
		public override string Name
		{
			get { return "SmugMug Items"; }
		}

		/// <summary>
		/// Gets or sets the SmugMug nickname.
		/// </summary>
		/// <value>The SmugMug nickname.</value>
		public string NickName { get; set; }

		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		/// <value>The title.</value>
		public override string Title
		{
			get
			{
				if (String.IsNullOrEmpty(base.Title))
					return "Recent Photos";

				return base.Title;
			}
			set { base.Title = value; }
		}

		/// <summary>
		/// Adds the form elements.
		/// </summary>
		/// <returns></returns>
		protected override FormElementCollection AddFormElements()
		{
			FormElementCollection form = new FormElementCollection();

			ListFormElement itemsToDisplay = new ListFormElement("itemstodisplay", "Number of Photos", "(how many photos do you want to display?)");
			itemsToDisplay.Add(new ListItemFormElement("3", "3"));
			itemsToDisplay.Add(new ListItemFormElement("6", "6", true));
			itemsToDisplay.Add(new ListItemFormElement("9", "9"));

			form.Add(AddTitleElement());
			form.Add(new TextFormElement("nickname", "NickName", "(your SmugMug nickname)"));
			form.Add(itemsToDisplay);

			return form;
		}

		/// <summary>
		/// Datas as name value collection.
		/// </summary>
		/// <returns></returns>
		protected override NameValueCollection DataAsNameValueCollection()
		{
			NameValueCollection nvc = base.DataAsNameValueCollection();
			nvc["nickname"] = this.NickName;
			nvc["itemstodisplay"] = this.ItemsToDisplay.ToString();
			return nvc;
		}

		/// <summary>
		/// If the enclosure is a video, morph the URL into that of the preview image.
		/// </summary>
		/// <param name="imageUrl">The image URL.</param>
		/// <returns>The URL of the preview image if the URL is a video, otherwise itself.</returns>
		internal string FixVideoUrl(string imageUrl)
		{
			if (String.IsNullOrEmpty(imageUrl))
				return imageUrl;

			// Locate the extension of the image; send back the original URL for extesionless URLs.
			string extension = Path.GetExtension(imageUrl);
			if (String.IsNullOrEmpty(extension))
				return imageUrl;

			if (!VideoExtensions.Contains(extension.ToLower()))
				return imageUrl;

			int formatStartIndex = imageUrl.LastIndexOf('-');
			if (formatStartIndex < 0)
				return imageUrl;

			return imageUrl.Replace(imageUrl.Substring(formatStartIndex), "-Th.jpg");
		}

		/// <summary>
		/// Renders the data.
		/// </summary>
		/// <returns></returns>
		public override string RenderData()
		{
			StringBuilder data = new StringBuilder();
			data.AppendLine("<ul class=\"pic\">");
			
			if (!String.IsNullOrEmpty(this.NickName))
			{
				try
				{
					RssChannel channel = Document();
					if (channel != null && channel.Items != null)
					{
						int numberOfPics = Math.Min(channel.Items.Count, this.ItemsToDisplay);
						for (int i = 0; i < numberOfPics; i++)
						{
							RssItem item = channel.Items[i];
							string imageUrl = item.Enclosure.Url;
							if (HttpContext.Current.Request.IsSecureConnection)
								imageUrl = imageUrl.Replace("http://", "https://");

							imageUrl = FixVideoUrl(imageUrl);

							data.AppendFormat("<li class=\"pic\"><a title=\"{0}\" href=\"{1}\"><img alt=\"{0}\" src=\"{2}\"/></a></li>", item.Title, item.Link, imageUrl);
						}
					}
				}
				catch (Exception)
				{
				}
			}

			data.AppendLine("</ul>");
			return data.ToString();
		}

		/// <summary>
		/// Sets the values.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="nvc">The NVC.</param>
		/// <returns></returns>
		public override StatusType SetValues(HttpContext context, NameValueCollection nvc)
		{
			StatusType status = base.SetValues(context, nvc);
			if (status == StatusType.Success)
			{
				// Save the values
				if (String.IsNullOrEmpty(nvc["nickname"]))
				{
					SetMessage(context, "Please enter a SmugMug nickname.");
					return StatusType.Error;
				}
				this.NickName = nvc["nickname"];
				this.ItemsToDisplay = Int32.Parse(nvc["itemstodisplay"]);

				// Activate the feed
				try
				{
					RegisterForSyndication();
				}
				catch(Exception ex)
				{
					SetMessage(context, ex.Message);
					return StatusType.Error;
				}
			}
			return status;
		}
	}
}