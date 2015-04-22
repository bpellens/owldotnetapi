/*****************************************************************************
 * OwlNodeCollection.cs
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
	/// Represents a collection of OWL Nodes
	/// </summary>
	public class OwlNodeCollection : IOwlNodeCollection
	{
		#region Variables
		/// <summary>
		/// The collection of OwlNode objects
		/// </summary>
		private Hashtable _nodes;

		#endregion

		#region Creators
		/// <summary>
		/// Initializes a new instance of the OwlNodeCollection class.
		/// </summary>
		public OwlNodeCollection()
		{
			_nodes = new Hashtable();
		}

		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the Node with the specified ID
		/// </summary>
		/// <exception cref="ArgumentNullException">nodeID is a null reference.</exception>
		public IOwlNode this[string nodeID]
		{
			get
			{
				return (OwlNode)_nodes[nodeID];
			}
			set
			{
				_nodes[nodeID] = value;
			}
		}

		/// <summary>
		/// Gets the total number of nodes in this collection.
		/// </summary>
		public int Count
		{
			get
			{
				return _nodes.Count;
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
			return _nodes.GetEnumerator();
		}

		#endregion

		#region Manipulators
		/// <summary>
		/// Adds a node to the collection.
		/// </summary>
		/// <param name="nodeID">The ID of the node to add.</param>
		/// <param name="newNode">An object that implements the IOwlNode interface. This is a reference to the node to add.</param>
		/// <exception cref="ArgumentException">A node with the specified ID already exists in the collection.</exception>
		/// <exception cref="ArgumentNullException">The specified ID is a null reference.</exception>
		public void Add(string nodeID, IOwlNode newNode)
		{
			if(newNode == null)
				throw (new ArgumentNullException());
			// Added:
			// Check on duplicate keys, if key is already in the dictionary,
			// then we don't to anything.
			// Why: Error coming from Hano Soma
			// Todo: Recheck this if it is correct
			OwlNode rNode = (OwlNode)_nodes[nodeID];
			if(rNode != null)
				return;
			// End
			_nodes.Add(nodeID,newNode);
		}

		/// <summary>
		/// Adds a node to the collection.
		/// </summary>
		/// <param name="newNode">An object that implements the IOwlNode interface. This is a reference to the node to add.</param>
		/// <exception cref="ArgumentException">A node with the same ID already exists in the collection.</exception>
		/// <exception cref="ArgumentNullException">The ID of the specified node is a null Reference.</exception>
		public void Add(IOwlNode newNode)
		{
			Add(newNode.ID,newNode);
		}

		/// <summary>
		/// Removes a node from this collection.
		/// </summary>
		/// <param name="node">An object that implements the IOwlNode interface. This is the node to remove.</param>
		/// <returns>True if a node with the same ID was found and removed.</returns>
		/// <exception cref="ArgumentException">node is a null reference.</exception>
		/// <remarks>This method removes the node with the same ID as the specified node.</remarks>
		public bool Remove(IOwlNode node)
		{
			if(node == null)
				throw (new ArgumentNullException());
			if(Contains(node))
			{
				_nodes.Remove(node.ID);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Removes all the nodes from this collection
		/// </summary>
		public void RemoveAll()
		{
			_nodes.Clear();
		}

		/// <summary>
		/// Determines whether the specified node is a member of this collection.
		/// </summary>
		/// <param name="node">An object that implements the IOwlNode interface.</param>
		/// <returns>True if a node with the same ID was found in the collection.</returns>
		public bool Contains(IOwlNode node)
		{
			if(node == null)
				return false;
			OwlNode rNode = (OwlNode)_nodes[node.ID];
			if(rNode != null)
				return (rNode == node);
			return false;
		}

		#endregion

	}
}
