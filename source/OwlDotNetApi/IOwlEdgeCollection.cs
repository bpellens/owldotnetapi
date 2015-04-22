/*****************************************************************************
 * IOwlEdgeCollection.cs
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
	/// Represents a collection of objects that implement the IOwlEdge interface. 
	/// This collection maps edge IDs to objects that implement the IOwlEdgeList interface
	/// </summary>
	public interface IOwlEdgeCollection
	{
		/// <summary>
		/// When implemented by a class, returns the IOwlEdge at the specified index
		/// </summary>
		IOwlEdge this[int index]
		{
			get;
		}
		/// <summary>
		/// When implemented by a class, returns a list of edges in this collection with the specified ID
		/// </summary>
		IOwlEdgeList this[string edgeID]
		{
			get;
		}

		/// <summary>
		/// When implemented by a class, returns the IOwlEdge at the given index from the list of edges with the specified ID
		/// </summary>
		IOwlEdge this[string edgeID, int index]
		{
			get;
		}

		/// <summary>
		/// When implemented by a class, adds the specified edge to this collection
		/// </summary>
		/// <param name="edgeID">The ID of the edge</param>
		/// <param name="edge">The IOwlEdge object</param>
		void Add(string edgeID, IOwlEdge edge);

		/// <summary>
		/// When implemented by a class, adds the specified edge to this collection
		/// </summary>
		/// <param name="edge">The IOwlEdge object</param>
		void Add(IOwlEdge edge);

		/// <summary>
		/// When implemented by a class, returns the total number of edges in this collection
		/// </summary>
		int Count
		{
			get;
		}

		/// <summary>
		/// When implemented by a class, gets an enumerator that can iterate through the collection
		/// </summary>
		IEnumerator GetEnumerator();

		/// <summary>
		/// When implemented by a class, determines whether the collection contains any edge with the specified ID
		/// </summary>
		/// <param name="edgeID">A string containing an ID</param>
		/// <returns>True if this collection contains any edge with the specified ID</returns>
		bool Contains(string edgeID);

		/// <summary>
		/// When implemented by a class, determines whether the collection contains the specified IOwlEdge object
		/// </summary>
		/// <param name="edge">An object that implements the IOwlEdge interface</param>
		/// <returns>True if this collection contains the specified object</returns>
		bool Contains(IOwlEdge edge);

		/// <summary>
		/// When implemented by a class, removes the specified IOwlEdge object
		/// </summary>
		/// <param name="edge">An object that implements the IOwlEdge interface</param>
		void Remove(IOwlEdge edge);

		/// <summary>
		/// When implemented by a class removes all the edges from this collection
		/// </summary>
		void RemoveAll();
	}
}
