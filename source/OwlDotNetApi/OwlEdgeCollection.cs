/*****************************************************************************
 * OwlEdgeCollection.cs
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
	/// Represents a collection of edges. This class maps edge IDs to lists of OwlEdge objects
	/// </summary>
	public class OwlEdgeCollection : IOwlEdgeCollection
	{
		#region Variables
		/// <summary>
		/// The hashtable containing the map
		/// </summary>
		private Hashtable _edgeMap;

		/// <summary>
		/// List of all the edges in this collection
		/// </summary>
		private OwlEdgeList _edges;

		/// <summary>
		/// Map of Edge objects to an index into the list of edges
		/// </summary>
		private Hashtable _edgeIndexMap;

		#endregion

		#region Creators
		/// <summary>
		/// Initializes a new instance of the OwlEdgeCollection class.
		/// </summary>
		public OwlEdgeCollection()
		{
			_edgeMap = new Hashtable();
			_edgeIndexMap = new Hashtable();
			_edges = new OwlEdgeList();
		}

		#endregion

		#region Properties
		/// <summary>
		/// Returns a list of edges with the specified Edge ID
		/// </summary>
		public IOwlEdgeList this[string edgeID]
		{
			get
			{
				OwlEdgeList edgeList = (OwlEdgeList)_edgeMap[edgeID];
				if(edgeList != null)
					return (IOwlEdgeList)edgeList;
				else
					return null;
			}
		}

		/// <summary>
		/// Returns the edge at the given index from the list of edges with the specified ID
		/// </summary>
		public IOwlEdge this[string edgeID, int index]
		{
			get
			{
				IOwlEdgeList edges = (IOwlEdgeList)_edgeMap[edgeID];
				if(edges == null)
					return null;
				try
				{
					return edges[index];
				}
				catch(ArgumentOutOfRangeException)
				{
					return null;
				}
			}
		}

		/// <summary>
		/// Returns an edge at the given index
		/// </summary>
		public IOwlEdge this[int index]
		{
			get
			{
				try
				{
					return _edges[index];
				}
				catch(ArgumentOutOfRangeException)
				{
					return null;
				}
			}
		}

		/// <summary>
		/// Returns the total number of edges contained in this collection
		/// </summary>
		public int Count
		{
			get
			{
				return _edges.Count;
			}
		}

		#endregion

		#region Accessors
		/// <summary>
		/// Gets an enumerator that can iterate through the collection
		/// </summary>
		public IEnumerator GetEnumerator()
		{
			return _edges.GetEnumerator();
		}

		#endregion

		#region Manipulators
		/// <summary>
		/// Adds an edge to this collection
		/// </summary>
		/// <param name="edge">An object that implements the IOwlEdge interface</param>
		/// <exception cref="ArgumentNullException">The specified edge is a null reference</exception>
		public void Add(IOwlEdge edge)
		{
			Add(edge.ID, edge);
		}

		/// <summary>
		/// Adds an edge to this collection
		/// </summary>
		/// <param name="edgeID">The ID of the edge</param>
		/// <param name="edge">An object that implements the IOwlEdge interface</param>
		/// <exception cref="ArgumentNullException">The specified edge is a null reference or the specified edgeID is a null reference</exception>
		public void Add(string edgeID, IOwlEdge edge)
		{
			IOwlEdgeList edgeList = (IOwlEdgeList)_edgeMap[edgeID];
			if(edgeList == null)
			{
				edgeList = new OwlEdgeList();
				_edgeMap[edgeID] = edgeList;
			}
			edgeList.Add(edge);
			_edges.Add(edge);
		}

		/// <summary>
		/// Removes the specified edge object if it exists.
		/// </summary>
		/// <param name="edge">An object that implements the IOwlEdge interface</param>
		/// <remarks>This method uses object.Equals to determine whether the specified edge exists and then removes it if it is present in the collection</remarks>
		public void Remove(IOwlEdge edge)
		{
			if(edge == null)
				throw(new ArgumentNullException());
			IOwlEdgeList edgeList = (IOwlEdgeList)_edgeMap[edge.ID];
			if(edgeList == null)
				return;
			edgeList.Remove(edge);
			_edges.Remove(edge);
		}

		/// <summary>
		/// Determines whether this collection contains any edges with the specified edge ID
		/// </summary>
		/// <param name="edgeID">A string containing the edge ID</param>
		/// <returns>True if there are any edges in this collection with the specified ID</returns>
		public bool Contains(string edgeID)
		{
			IOwlEdgeList edges = (IOwlEdgeList)_edgeMap[edgeID];
			if((edges == null) || (edges.Count == 0))
				return false;
			return true;
		}
		
		/// <summary>
		/// Removes all the edges from this collection
		/// </summary>
		public void RemoveAll()
		{
			_edges.RemoveAll();
			_edgeMap.Clear();
			_edgeIndexMap.Clear();
		}

		/// <summary>
		/// Determines whether the specified edge object is present in this collection
		/// </summary>
		/// <param name="edge">An object that implements the IOwlEdge interface</param>
		/// <returns>True if this collection contains the specified edge object</returns>
		/// <remarks>This method uses object.Equals to determine whether the specified edge object exists in the collection</remarks>
		public bool Contains(IOwlEdge edge)
		{
			if(!Contains(edge.ID))
				return false;
			return ((IOwlEdgeList)_edgeMap[edge.ID]).Contains(edge);
		}

		#endregion
	}
}
