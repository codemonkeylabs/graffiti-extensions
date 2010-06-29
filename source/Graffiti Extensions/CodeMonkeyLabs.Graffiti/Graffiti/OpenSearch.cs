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
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Web;
using System.Xml;
using Graffiti.Core;

namespace CodeMonkeyLabs.Graffiti
{
	/// <summary>
	/// Provides OpenSearch capabilities to the Graffiti site.
	/// </summary>
	[Serializable]
	public class OpenSearch : GraffitiEvent
	{
		private const string HandlerUrl = "~/opensearch.ashx";
		private const string LinkFormat = "<link rel=\"search\" type=\"application/opensearchdescription+xml\" href=\"{0}\" title=\"{1}\">";

		/// <summary>
		/// Gets the description.
		/// </summary>
		/// <value>The description.</value>
		public override string Description
		{
			get { return "Adds <a href='http://www.opensearch.org/'>OpenSearch</a> functionality to the site. " + Utility.CreatedBy("opensearch"); }
		}

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
			get { return "OpenSearch"; }
		}

		/// <summary>
		/// Gets or sets the search description.
		/// </summary>
		/// <value>The search description.</value>
		public string SearchDescription { get; set; }

		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		/// <value>The title.</value>
		public string SearchName { get; set; }

		/// <summary>
		/// Gets the data as a name value collection.
		/// </summary>
		/// <returns></returns>
		protected override NameValueCollection DataAsNameValueCollection()
		{
			return new NameValueCollection
	       	{
	       		{"codemonkeylabs.opensearch.shortname", this.SearchName},
	       		{"codemonkeylabs.opensearch.description", this.SearchDescription}
	       	};
		}

		/// <summary>
		/// Initializes this module.
		/// </summary>
		/// <param name="ga">The Graffiti application.</param>
		public override void Init(GraffitiApplication ga)
		{
			ga.BeginRequest += this.OnBeginRequest;
			ga.RenderHtmlHeader += this.OnRenderHtmlHeader;
		}

		/// <summary>
		/// Sets the values.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="nvc">The NVC.</param>
		/// <returns></returns>
		public override StatusType SetValues(HttpContext context, NameValueCollection nvc)
		{
			string name = nvc["codemonkeylabs.opensearch.shortname"];
			if (!String.IsNullOrEmpty(name) && name.Length > 16)
			{
				SetMessage(context, "The name must be 16 characters or less.");
				return StatusType.Error;
			}

			string description = nvc["codemonkeylabs.opensearch.description"];
			if (!String.IsNullOrEmpty(description) && description.Length > 1024)
			{
				SetMessage(context, "The description must be 1024 characters or less.");
				return StatusType.Error;
			}

			this.SearchName = name.Trim();
			this.SearchDescription = description.Trim();

			ZCache.RemoveByPattern("opensearch-");

			return StatusType.Success;
		}

		/// <summary>
		/// Adds the form elements.
		/// </summary>
		/// <returns>The form elements.</returns>
		protected override FormElementCollection AddFormElements()
		{
			return new FormElementCollection
	       	{
	       		new TextFormElement("codemonkeylabs.opensearch.shortname", "Name", "The text displayed in the OpenSearch box."),
	       		new TextFormElement("codemonkeylabs.opensearch.description", "Description", "A description of the search.")
	       	};
		}

		/// <summary>
		/// Builds the search description.
		/// </summary>
		/// <returns>The search description.</returns>
		private string BuildSearchDescription()
		{
			StringBuilder xml = new StringBuilder();
			using (Utf8EncodedStringWriter encodedStringWriter = new Utf8EncodedStringWriter(xml))
			using (XmlWriter writer = XmlWriter.Create(encodedStringWriter, new XmlWriterSettings { Indent = true }))
			{
				writer.WriteStartDocument();
				writer.WriteStartElement("OpenSearchDescription", "http://a9.com/-/spec/opensearch/1.1/");
				writer.WriteElementString("ShortName", this.FormatShortName());
				writer.WriteElementString("Description", this.FormatDescription());
				writer.WriteStartElement("Url");
				writer.WriteAttributeString("type", "text/html");
				writer.WriteAttributeString("template", this.FormatSearchUrl());
				writer.WriteEndElement();
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

			this.SendOpenSearchDescription();
		}

		/// <summary>
		/// Called when the RenderHtmlHeader event is fired.
		/// </summary>
		/// <param name="sb">The header content.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void OnRenderHtmlHeader(StringBuilder sb, EventArgs e)
		{
			sb.AppendLine(String.Format(LinkFormat, VirtualPathUtility.ToAbsolute(HandlerUrl), this.FormatShortName()));
		}

		/// <summary>
		/// Formats the description.
		/// </summary>
		/// <returns></returns>
		private string FormatDescription()
		{
			if (!String.IsNullOrEmpty(this.SearchDescription))
				return this.SearchDescription;

			SiteSettings settings = SiteSettings.Get();
			if (!String.IsNullOrEmpty(settings.TagLine))
				return settings.TagLine;

			return String.Empty;
		}

		/// <summary>
		/// Formats the short name.
		/// </summary>
		/// <returns>The short name.</returns>
		private string FormatShortName()
		{
			if (!String.IsNullOrEmpty(this.SearchName))
				return this.SearchName;

			return String.Format("{0} Search", SiteSettings.Get().Title);
		}

		/// <summary>
		/// Formats the search URL.
		/// </summary>
		/// <returns></returns>
		private string FormatSearchUrl()
		{
			return Utility.FullUrl(new Urls().Search) + "?q={searchTerms}";
		}

		/// <summary>
		/// Sends the open search description to the client.
		/// </summary>
		private void SendOpenSearchDescription()
		{
			// Check to see if we're responding to a request for the xml document. If we aren't, just return.
			if (!Utility.AmIHere(HandlerUrl))
				return;

			// Build the XML document
			var description = ZCache.Get<string>(String.Format("opensearch-{0}", HttpContext.Current.Request.Url.AbsoluteUri.ToLowerInvariant()));
			if (description == null)
				ZCache.InsertCache(String.Format("opensearch-{0}", HttpContext.Current.Request.Url.AbsoluteUri.ToLowerInvariant()), description = BuildSearchDescription(), 90);

			// Send the document
			HttpResponse response = HttpContext.Current.Response;
			response.ClearContent();
			response.ContentType = "text/xml";
			response.Write(description);
			response.End();
		}
	}
}