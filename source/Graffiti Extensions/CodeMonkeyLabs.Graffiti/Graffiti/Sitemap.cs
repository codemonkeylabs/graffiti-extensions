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
using System.Text;
using System.Web;
using System.Xml;
using Graffiti.Core;

namespace CodeMonkeyLabs.Graffiti
{
	/// <summary>
	/// Creates a sitemap for the Graffiti site.
	/// </summary>
	[Serializable]
	public class Sitemap : GraffitiEvent
	{
		private const string HandlerUrl = "~/sitemap.ashx";

		/// <summary>
		/// Gets the description.
		/// </summary>
		/// <value>The description.</value>
		public override string Description
		{
			get { return "Generates a sitemap for search engines to consume. " + Utility.CreatedBy("sitemap"); }
		}

		/// <summary>
		/// Gets or sets a value indicating whether to include uncategorized posts.
		/// </summary>
		/// <value>
		/// <c>true</c> toinclude uncategorized posts; otherwise, <c>false</c>.
		/// </value>
		public bool IncludeUncategorizedPosts { get; set; }

		/// <summary>
		/// Gets a value indicating whether this instance is editable.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is editable; otherwise, <c>false</c>.
		/// </value>
		public override bool IsEditable
		{
			get { return true; }
		}

		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>The name.</value>
		public override string Name
		{
			get { return "Sitemap"; }
		}

		/// <summary>
		/// Initializes this module.
		/// </summary>
		/// <param name="ga">The Graffiti application.</param>
		public override void Init(GraffitiApplication ga)
		{
			ga.BeginRequest += this.OnBeginRequest;
		}

		/// <summary>
		/// Sets the values.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="nvc">The NVC.</param>
		/// <returns></returns>
		public override StatusType SetValues(HttpContext context, NameValueCollection nvc)
		{
			string includeuncategorizedposts = nvc["codemonkeylabs.sitemap.includeuncategorizedposts"] ?? "off";

			if (includeuncategorizedposts == "on")
				this.IncludeUncategorizedPosts = true;
			else if (includeuncategorizedposts == "off")
				this.IncludeUncategorizedPosts = false;

			ZCache.RemoveByPattern("sitemap-{0}");

			return StatusType.Success;
		}

		/// <summary>
		/// Gets the data as a name value collection.
		/// </summary>
		/// <returns></returns>
		protected override NameValueCollection DataAsNameValueCollection()
		{
			return new NameValueCollection
	       	{
	       		{"codemonkeylabs.sitemap.includeuncategorizedposts", this.IncludeUncategorizedPosts.ToString()}
	       	};
		}

		/// <summary>
		/// Adds the form elements.
		/// </summary>
		/// <returns>The form elements.</returns>
		protected override FormElementCollection AddFormElements()
		{
			return new FormElementCollection
	       	{
	       		new CheckFormElement("codemonkeylabs.sitemap.includeuncategorizedposts", "Include Uncategorized Posts", "Should uncategorized posts be included in the sitemap?", false)
	       	};
		}

		/// <summary>
		/// Builds the sitemap.
		/// </summary>
		/// <returns></returns>
		private string BuildSitemap()
		{
			StringBuilder xml = new StringBuilder();
			using (Utf8EncodedStringWriter encodedStringWriter = new Utf8EncodedStringWriter(xml))
			using (XmlWriter writer = XmlWriter.Create(encodedStringWriter, new XmlWriterSettings { Indent = true }))
			{
				// Root of the document
				writer.WriteStartDocument();
				writer.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");

				Macros macros = new Macros();
				Urls urls = new Urls();

				// Home
				WriteSitemapNode(writer, macros.FullUrl(urls.Home), DateTime.Today, "daily", "0.5");

				// Categories
				// Temporary solution for last modified date - use today
				foreach (Category category in CategoryCollection.FetchAll())
				{
					if (category.IsUncategorized || category.IsDeleted)
						continue;
					
					WriteSitemapNode(writer, macros.FullUrl(category.Url), DateTime.Today, "daily", "0.5");
				}

				// Posts
				PostCollection posts = PostCollection.FetchAll();
				posts.Sort(delegate(Post p1, Post p2)
	           	{
	           		return Comparer<DateTime>.Default.Compare(p1.Published, p2.Published);
	           	});

				foreach (Post post in posts)
				{
					if (!post.IsPublished || post.IsDeleted || post.IsDirty || post.Published > DateTime.Now)
						continue;

					if (post.Category.IsUncategorized && !this.IncludeUncategorizedPosts)
						continue;

					WriteSitemapNode(writer, macros.FullUrl(post.Url), post.ModifiedOn, "daily", "0.5");
				}

				writer.WriteEndDocument();
			}

			return xml.ToString();
		}

		/// <summary>
		/// Called when the BeginRequest event is fired.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void OnBeginRequest(object sender, EventArgs e)
		{
			if (HttpContext.Current == null)
				return;

			this.SendSitemap();
		}

		/// <summary>
		/// Sends the sitemap to the client.
		/// </summary>
		private void SendSitemap()
		{
			// Check to see if we're responding to a request for the xml document. If we aren't, just return.
			if (!Utility.AmIHere(HandlerUrl))
				return;

			// Build the XML document
			var sitemap = ZCache.Get<string>(String.Format("sitemap-{0}", HttpContext.Current.Request.Url.AbsoluteUri.ToLowerInvariant()));
			if (sitemap == null)
				ZCache.InsertCache(String.Format("opensearch-{0}", HttpContext.Current.Request.Url.AbsoluteUri.ToLowerInvariant()), sitemap = BuildSitemap(), 90);

			// Send the document
			HttpResponse response = HttpContext.Current.Response;
			response.ClearContent();
			response.ContentType = "text/xml";
			response.Write(sitemap);
			response.End();
		}

		/// <summary>
		/// Writes a single node to the Sitemap.
		/// </summary>
		/// <param name="writer">The writer that is building the sitemap.</param>
		/// <param name="url">The full URL of the page.</param>
		/// <param name="lastModified">The last modified date.</param>
		/// <param name="changeFrequency">The change frequency.</param>
		/// <param name="priority">The priority.</param>
		private void WriteSitemapNode(XmlWriter writer, string url, DateTime lastModified, string changeFrequency, string priority)
		{
			writer.WriteStartElement("url");
			writer.WriteElementString("loc", url);
			writer.WriteElementString("lastmod", lastModified.ToString("yyyy-MM-dd"));
			writer.WriteElementString("changefreq", changeFrequency);
			writer.WriteElementString("priority", priority);
			writer.WriteEndElement();
		}
	}
}