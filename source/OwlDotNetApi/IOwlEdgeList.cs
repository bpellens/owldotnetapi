/*****************************************************************************
 * IOwlEdgeList.cs
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
	/// Represents a collection of IOwlEdge objects
	/// </summary>
	public interface IOwlEdgeList
	{
		/// <summary>
		/// When implemented by a class, gets the total number of members in this collection
		/// </summary>
		int Count
		{
			get;
		}

		/// <summary>
		/// When implemented by a class, gets the IOwlEdge at the specified index
		/// </summary>
		IOwlEdge this[int index]
		{
			get;
			set;
		}

		/// <summary>
		/// When implemented by a class, returns an enumerator that can iterate through the collection
		/// </summary>
		IEnumerator GetEnumerator();

		/// <summary>
		/// When implemented by a class, adds an IOwlEdge object to the collection
		/// </summary>
		/// <param name="edge">The IOwlEdge to add to the collection</param>
		void Add(IOwlEdge edge);

		/// <summary>
		/// When implemented by a class, determines whether the specified IOwlEdge is a member of this collection
		/// </summary>
		/// <param name="edge">An IOwlEdge</param>
		/// <returns>True if the specified edge belongs to the collection.</returns>
		bool Contains(IOwlEdge edge);

		/// <summary>
		/// When implemented by a class, removes the specified IOwlEdge object from the collection
		/// </summary>
		/// <param name="edge">The edge to remove</param>
		void Remove(IOwlEdge edge);

		/// <summary>
		/// When implemented by a class removes all the edges from this collection
		/// </summary>
		void RemoveAll();
	}
}
