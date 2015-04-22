/*****************************************************************************
 * IOwlNodeCollection.cs
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
	/// Represents a collection of IOwlNode objects
	/// </summary>
	public interface IOwlNodeCollection
	{
		/// <summary>
		/// When implemented by a class, gets the total number of IOwlNodes in this collection
		/// </summary>
		int Count
		{
			get;
		}

		/// <summary>
		/// When implemented by a class, gets or sets the node with the given ID
		/// </summary>
		IOwlNode this[string nodeID]
		{
			get;
			set;
		}

		/// <summary>
		/// When implemented by a class, gets an enumerator that can iterate through the collection
		/// </summary>
		IEnumerator GetEnumerator();

		/// <summary>
		/// When implemented by a class, adds a new node to the collection
		/// </summary>
		/// <param name="nodeID">A string containing the ID of the new node</param>
		/// <param name="newNode">The new node to add</param>
		void Add(string nodeID, IOwlNode newNode);

		/// <summary>
		/// When implemented by a class, adds a new node to the collection
		/// </summary>
		/// <param name="newNode">The new node to add</param>
		void Add(IOwlNode newNode);

		/// <summary>
		/// When implemented by a class, removes a node from the collection
		/// </summary>
		/// <param name="node"></param>
		/// <returns>True if the node was found and removed</returns>
		bool Remove(IOwlNode node);

		/// <summary>
		/// When implemented by a class removes all the nodes from this collection
		/// </summary>
		void RemoveAll();

		/// <summary>
		/// When implemented by a class, determines whether the specified node exists in the collection
		/// </summary>
		/// <param name="node">An IOwlNode</param>
		/// <returns>True if the specified node was found in the collection</returns>
		bool Contains(IOwlNode node);
	}
}
