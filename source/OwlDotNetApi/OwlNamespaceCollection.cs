/*****************************************************************************
 * OwlNamespaceCollection.cs
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
using System.Collections;
using System.Runtime.InteropServices;

namespace OwlDotNetApi
{
	/// <summary>
	/// Represents a collection of Namespaces
	/// </summary>
	public class OwlNamespaceCollection : IOwlNamespaceCollection
	{
		#region Variables
		/// <summary>
		/// The OWL Namespace.
		/// </summary>
		public const string OwlNamespace = "http://www.w3.org/2002/07/owl#";

		/// <summary>
		/// The standard prefix of the OWL namespace.
		/// </summary>
		public const string OwlNamespacePrefix = "owl";

		/// <summary>
		/// The RDF Namespace.
		/// </summary>
		public const string RdfNamespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#";

		/// <summary>
		/// The standard prefix of the RDF namespace.
		/// </summary>
		public const string RdfNamespacePrefix = "rdf";

		/// <summary>
		/// The RDF Schema Namespace.
		/// </summary>
		public const string RdfSchemaNamespace = "http://www.w3.org/2000/01/rdf-schema#";

		/// <summary>
		/// The standard prefix of the RDF Schema Namespace.
		/// </summary>
		public const string RdfSchemaNamespacePrefix = "rdfs";

		/// <summary>
		/// The XML Schema namespace.
		/// </summary>
		public const string XmlSchemaNamespace = "http://www.w3.org/2001/XMLSchema#";

		/// <summary>
		/// The standard prefix of the XML Schema namespace.
		/// </summary>
		public const string XmlSchemaNamespacePrefix = "xsd";
		
		/// <summary>
		/// The collection of namespaces
		/// </summary>
		private Hashtable _nameSpaces;

		#endregion

		#region Creators
		/// <summary>
		/// Initializes a new instance of the OwlNamespaceCollection class.
		/// </summary>
		public OwlNamespaceCollection()
		{
			_nameSpaces = new Hashtable();
		}

		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the Namespace with the specified name
		/// </summary>
		/// <exception cref="ArgumentException">Attempt to set a namespace with an empty name.</exception>
		/// <exception cref="ArgumentNullException">Attempt to store a null namespace.</exception>
		/// <exception cref="ArgumentNullException">Attempt to set a namespace with a null name.</exception>
		/// <remarks>The name of the namspace is a string consisting of the namespace prefix prefaced with xmlns:. 
		/// The one exception is xml:base where the value of the base URI of the OWL document is stored under the
		/// name xml:base.</remarks>
		public string this[string name]
		{
			get
			{
				return (String)_nameSpaces[name];
			}
			set
			{
				if(name == null)
					throw(new ArgumentNullException("The namespace name cannot be null."));
				if(name.Length == 0)
					throw(new ArgumentException("The namespace name cannot be an empty string."));
				if(value == null)
					throw(new ArgumentNullException("The namespace value cannot be null"));
				_nameSpaces[name] = value;
			}
		}

		#endregion

		#region Accessors
		/// <summary>
		/// Gets an enumerator that can iterate through this collection.
		/// </summary>
		/// <returns>An object that implements that implements the <see cref="IEnumerator"/> interface.</returns>
		public IEnumerator GetEnumerator()
		{
			return _nameSpaces.GetEnumerator();
		}

		#endregion

		#region Manipulators

		/// <summary>
		/// Removes a namespace from the namespace collection
		/// </summary>
		/// <param name="name">The name of the namespace to remove</param>
		/// <exception cref="ArgumentNullException">The specified name is a null reference.</exception>
		/// <remarks>Removes the name from the collection.</remarks>
		public void Remove(string name)
		{
			_nameSpaces.Remove(name);
		}

		/// <summary>
		/// Removes all the namespaces from this collection
		/// </summary>
		public void RemoveAll()
		{
			_nameSpaces.Clear();
		}
		/// <summary>
		/// Gets the total number of namespaces in this collection.
		/// </summary>
		public int Count
		{
			get
			{
				return _nameSpaces.Count;
			}
		}

		#endregion
	}
}
