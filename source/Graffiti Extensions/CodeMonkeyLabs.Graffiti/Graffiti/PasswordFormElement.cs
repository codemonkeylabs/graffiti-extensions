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
using System.Text;
using Graffiti.Core;

namespace CodeMonkeyLabs.Graffiti
{
    /// <summary>
    /// Creates a password input.
    /// </summary>
	public class PasswordFormElement : FormElement
	{
		private static readonly string format = "<input type=\"password\" id=\"{0}\" name=\"{0}\" class=\"large\" value=\"{1}\" />";

        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordFormElement"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="desc">The desc.</param>
        /// <param name="tip">The tip.</param>
		public PasswordFormElement(string name, string desc, string tip) : base(name, desc, tip)
		{
		}

        /// <summary>
        /// Writes the input field to the output.
        /// </summary>
        /// <param name="sb">The sb.</param>
        /// <param name="nvc">The NVC.</param>
		public override void Write(StringBuilder sb, NameValueCollection nvc)
		{
			sb.AppendLine("<h2>");
			sb.AppendLine(Description + ": " + SafeToolTip(false));
			sb.AppendLine("</h2>");

			sb.AppendLine(string.Format(format, Name, nvc[Name]));
		}
	}
}