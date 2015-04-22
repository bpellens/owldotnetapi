/*****************************************************************************
 * InvalidOwlException.cs
 * 
 * Copyright (c) 2005, Bram Pellens.
 *  
 * This file is part of the OwlDotNetApi.
 * The OwlDotNetApi is free software; you can redistribute it and/or 
 * modify it under the terms of the GNU Lesser General Public License as published 
 * by the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 * 
 * The OwlDotNetApi is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public License
 * along with the OwlDotNetApi; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 * 
 * Author: 
 * 
 * Bram Pellens
 * bpellens@gmail.com
 ******************************************************************************/

using System;

namespace OwlDotNetApi
{
	/// <summary>
	/// Represents an exception that is thrown when invalid OWL Syntax is encountered by the parser
	/// </summary>
	public class InvalidOwlException : Exception
	{
		#region Variables
		private string _message;

		#endregion

		#region Creators
		/// <summary>
		/// Initializes a new instance of the InvalidOwlException class with an empty message
		/// </summary>
		public InvalidOwlException()
		{
			_message = "";
		}

		/// <summary>
		/// Initializes a new instance of the InvalidOwlException class with the given message
		/// </summary>
		/// <param name="message">A string describing a reason for the exception</param>
		public InvalidOwlException(string message)
		{
			_message = message;
		}

		#endregion

		#region Properties
		/// <summary>
		/// Gets a message describing the exeption or an empty string
		/// </summary>
		public override string Message
		{
			get
			{
				return _message;
			}
		}

		#endregion
	}
}
