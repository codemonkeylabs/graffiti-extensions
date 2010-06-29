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

using System.IO;
using System.Text;

namespace CodeMonkeyLabs.Graffiti
{
	/// <summary>
	/// A <see cref="StringWriter"/> with UTF-8 encoding.
	/// </summary>
	internal class Utf8EncodedStringWriter : StringWriter
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Utf8EncodedStringWriter"/> class.
		/// </summary>
		/// <param name="sb">The sb.</param>
		public Utf8EncodedStringWriter(StringBuilder sb) : base(sb)
		{
		}

		/// <summary>
		/// Gets the <see cref="T:System.Text.Encoding"/> in which the output is written.
		/// </summary>
		/// <value></value>
		/// <returns>
		/// The Encoding in which the output is written.
		/// </returns>
		public override Encoding Encoding
		{
			get { return Encoding.UTF8; }
		}
	}
}