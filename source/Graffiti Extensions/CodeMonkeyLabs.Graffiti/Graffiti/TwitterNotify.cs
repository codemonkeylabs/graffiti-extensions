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
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using Graffiti.Core;

namespace CodeMonkeyLabs.Graffiti
{
	///<summary>
	/// Tweets when you make a post.
	///</summary>
	[Serializable]
	public class TwitterNotify : GraffitiEvent
	{
		private static readonly IEnumerable<ITwitterFormattingStrategy> formattingStrategies;

		static TwitterNotify()
		{
			formattingStrategies = new List<ITwitterFormattingStrategy>
           	{
           		new DefaultTwitterFormattingStrategy(),
           		new ShrinkUrlTwitterFormattingStrategy(new IsGdUrlShortener())
           	};
		}
		/// <summary>
		/// Gets the description.
		/// </summary>
		/// <value>The description.</value>
		public override string Description
		{
			get { return "Sends a tweet when you make a post. " + Utility.CreatedBy("twitternotify"); }
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
			get { return "Twitter Notify"; }
		}

		///<summary>
		/// Gets and sets the Twitter password.
		///</summary>
		public string Password { get; set; }

		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		/// <value>The title.</value>
		public string Title { get; set; }

		///<summary>
		/// Gets and sets the Twitter username.
		///</summary>
		public string Username { get; set; }

		/// <summary>
		/// Initializes this module.
		/// </summary>
		/// <param name="ga">The Graffiti application.</param>
		public override void Init(GraffitiApplication ga)
		{
			ga.AfterCommit += this.OnAfterCommit;
		}

		/// <summary>
		/// Sets the values.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="nvc">The NVC.</param>
		/// <returns></returns>
		public override StatusType SetValues(HttpContext context, NameValueCollection nvc)
		{
			string username = nvc["codemonkeylabs.twitternotify.username"];
			if (String.IsNullOrEmpty(username))
			{
				SetMessage(context, "The username can not be empty.");
				return StatusType.Error;
			}
			username = username.Trim();

			string password = nvc["codemonkeylabs.twitternotify.password"];
			if (String.IsNullOrEmpty(password))
			{
				SetMessage(context, "The password can not be empty.");
				return StatusType.Error;
			}
			password = password.Trim();

			if (!TwitterClient.ValidateCredentials(username, password))
			{
				SetMessage(context, "The credentials don't seem to be valid.");
				return StatusType.Error;
			}

			string title = (nvc["codemonkeylabs.twitternotify.title"] ?? String.Empty).Trim();

			this.Username = username;
			this.Password = password;
			this.Title = title;

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
	       		new TextFormElement("codemonkeylabs.twitternotify.username", "Username", "Your Twitter username."),
	       		new PasswordFormElement("codemonkeylabs.twitternotify.password", "Password", "Your Twitter password."),
	       		new TextFormElement("codemonkeylabs.twitternotify.title", "Title", "Optional heading for your tweets. 'Blogged' or 'Posted' are good examples.")
	       	};
		}

		/// <summary>
		/// Gets the data as a name value collection.
		/// </summary>
		/// <returns></returns>
		protected override NameValueCollection DataAsNameValueCollection()
		{
			return new NameValueCollection
	       	{
	       		{"codemonkeylabs.twitternotify.username", this.Username},
	       		{"codemonkeylabs.twitternotify.password", this.Password},
	       		{"codemonkeylabs.twitternotify.title", this.Title}
	       	};
		}

		/// <summary>
		/// Formats the tweet.
		/// </summary>
		/// <param name="post">The post.</param>
		/// <returns></returns>
		protected virtual string FormatTweet(Post post)
		{
			string updateText = String.Empty;
			foreach (ITwitterFormattingStrategy strategy in formattingStrategies)
			{
				updateText = strategy.Format(post, this.Title) ?? String.Empty;
				if (updateText.Length <= 140)
					break;
			}

			return updateText;
		}

		/// <summary>
		/// Called after an item has been committed.
		/// </summary>
		/// <param name="dataObject">The data object.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected virtual void OnAfterCommit(DataBuddyBase dataObject, EventArgs e)
		{
			// We only care about posts
			Post post = dataObject as Post;
			if (post == null)
				return;

			// Determine if we should tweet this post
			if (!ShouldTweet(post))
				return;

			try
			{
				// We've made it this far...the post is being published for the first time...alert the twitterverse!
				string updateText = FormatTweet(post);

				if (updateText.Length > 140)
				{
					Log.Warn(this.Name, "Unable to format '{0}' for twitter.", post.Title);
					return;
				}

				TwitterClient.UpdateStatus(this.Username, this.Password, updateText);
			}
			catch (WebException ex)
			{
				Log.Error("Twitter Notify", ex.Message);
			}
		}

		/// <summary>
		/// Determines if the post should be tweeted.
		/// </summary>
		/// <param name="post">The post.</param>
		/// <returns>True if we should tweet, otherwise false.</returns>
		protected virtual bool ShouldTweet(Post post)
		{
			if (post == null)
				throw new ArgumentNullException("post");

			// If the post is not currently published, we have nothing to do at this point
			if (!post.IsPublished || post.IsDeleted)
				return false;

			// I don't see any cleaner way to make sure that we only tweet a post once and only once
			foreach (VersionStore previousVersionData in VersionStore.GetVersionHistory(post.Id))
			{
				// If we find any published versions in the history, we have nothing to do
				Post previousVersion = Post.FromXML(previousVersionData.Data);
				if (previousVersion.IsPublished)
					return false;
			}

			return true;
		}
	}
}