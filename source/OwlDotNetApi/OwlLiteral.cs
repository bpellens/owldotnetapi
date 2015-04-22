/*****************************************************************************
 * OwlLiteral.cs
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
using System.Text;
using System.Runtime.InteropServices;

namespace OwlDotNetApi
{
	/// <summary>
	/// Represents a Literal in the OWL Graph.
	/// </summary>
	/// <remarks>A literal is uniquely identified by its value, LanguageID and Datatype URI. 
	/// The ID of a Literal is composed of a concatenated string of these three value. If the Datatype and Language ID are 
	/// not specified then they are assumed to be null (default for the locale or data) and the ID is set to the value.</remarks>
	public class OwlLiteral : OwlNode, IOwlLiteral
	{
		#region Variables
		/// <summary>
		/// The Datatype URI of this Literal
		/// </summary>
		private Uri _dataType;

		/// <summary>
		/// The value for this literal.
		/// </summary>
		private string _literalValue;

		#endregion

		#region Creators
		/// <summary>
		/// Initializes a new instance of the OwlLiteral class.
		/// </summary>
		/// <remarks>Sets the Datatype URI to null and the Value and the Language ID to empty strings.</remarks>
		public OwlLiteral()
		{
			_dataType = null;
		}

		/// <summary>
		/// Initializes a new instance of the OwlLiteral class.
		/// </summary>
		/// <param name="literalValue">A string representing the value of this Literal.</param>
		/// <remarks>Sets the Datatype URI to null and the Labguage ID to an empty string.</remarks>
		public OwlLiteral(string literalValue)
		{
			_dataType = null;
			_literalValue = literalValue;
		}

		/// <summary>
		/// Initializes a new instance if the OwlLiteral class.
		/// </summary>
		/// <param name="literalValue">A string representing the value of this literal.</param>
		/// <param name="languageID">A string representing the Language ID of this literal.</param>
		/// <param name="datatypeUri">A string representing the Datatype URI of this Literal.</param>
		/// <exception cref="UriFormatException">The specified datatypeUri is not null and is not a well formed URI.</exception>
		public OwlLiteral(string literalValue, string languageID, string datatypeUri)
		{
			if(datatypeUri != null)
				_dataType = new Uri(datatypeUri);
			_literalValue = literalValue;
			LangID = languageID;
		}

		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the Datatype URI of this Literal
		/// </summary>
		/// <exception cref="UriFormatException">Attempt to set the Datatype to a URI string that is not a well formed URI.</exception>
		public string Datatype
		{
			get
			{
				if(_dataType == null)
					return "";
				return _dataType.ToString();
			}
			set
			{
				if(_dataType == null)
                    _dataType = new Uri(value);
			}
		}
		
		/// <summary>
		/// Gets or sets the value of this Literal.
		/// </summary>
		public string Value
		{
			get
			{
				return _literalValue;
			}
			set
			{
				_literalValue = value;
			}
		}
		
		/// <summary>
		/// Gets the ID of this literal
		/// </summary>
		/// <exception cref="ArgumentException">Attempt to set the ID of this literal.</exception>
		/// <remarks>This is a string composed of the Value, LanguageID and the Datatype URI.
		/// You cannot use this property to set the ID of a literal.</remarks>
		public override string ID
		{
			get
			{
				string literalID = _literalValue;
				if((LangID != null) && (LangID.Length != 0))
					literalID+="@"+LangID;
				if(_dataType != null)
					literalID +="^^"+_dataType.ToString();
				return literalID;
			}
			set
			{
				throw(new NotSupportedException("Cannot directly set the ID of an OwlLiteral"));
			}
		}

		#endregion

		#region Manipulators

		/// <summary>
		/// Gets the string representation of this literal
		/// </summary>
		/// <returns>A string containg this literal</returns>
		public override string ToString()
		{
			return ID;
		}

		/// <summary>
		/// This is the accept method in the visitor pattern used for performing an action on the node.
		/// </summary>
		/// <param name="visitor">The visitor object itself</param>
		/// <param name="parent">The parent object of the node</param>
		public override void Accept(IOwlVisitor visitor, Object parent) 
		{
			visitor.Visit(this, parent);
		}

		#endregion
	}
}
