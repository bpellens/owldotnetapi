/*****************************************************************************
 * IOwlNamespaceCollection.cs
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

namespace OwlDotNetApi
{
	/// <summary>
	/// Represents a collection of Namespaces
	/// </summary>
	public interface IOwlNamespaceCollection
	{
		/// <summary>
		/// When implemented by a class, gets or sets the namespace with the specified name
		/// </summary>
		string this[string name]
		{
			get;
			set;
		}

		/// <summary>
		/// Gets an enumerator that can iterate through this collection.
		/// </summary>
		/// <returns>An object that implements that implements the <see cref="IEnumerator"/> interface.</returns>
		IEnumerator GetEnumerator();

		/// <summary>
		/// Gets the total number of namespaces in this collection.
		/// </summary>
		int Count
		{
			get;
		}

		/// <summary>
		/// Removes a namespace from this collection
		/// </summary>
		/// <param name="name">The name of the namespace to remove</param>
		void Remove(string name);

		/// <summary>
		/// When implemented by a class removes all the namespaces from this collection
		/// </summary>
		void RemoveAll();
	}
}
