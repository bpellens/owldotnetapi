/*****************************************************************************
 * OwlCollection.cs
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
	/// Represents an OWL resource of type rdf:List.
	/// </summary>
	public class OwlCollection : OwlResource, IOwlCollection
	{
		#region Variables
		/// <summary>
		/// The list of items in the collection.
		/// </summary>
		private ArrayList _items;

		#endregion
		
		#region Creators
		/// <summary>
		/// Initializes a new instance of the OwlCollection class
		/// </summary>
		/// <remarks>This constructor creates a new OwlNode with Uri rdf:List and sets it as the child node of an edge with URI rdf:type</remarks>	
		public OwlCollection()
		{
			_items = new ArrayList();
//			_typeEdge = new OwlEdge(OwlNamespaceCollection.RdfNamespace+"type");
//			_typeEdge.AttachChildNode(new OwlNode(OwlNamespaceCollection.RdfNamespace+"List"));
//			AttachChildEdge(_typeEdge);
		}

		/// <summary>
		/// Initializes a new instance of the OwlCollection class with the given Uri and TypeNode
		/// </summary>
		/// <param name="nodeUri">A string representing the Uri of this Resource</param>
		/// <param name="typeNode">The OwlNode object to attach to the edge specifying the type. This is usually a node with ID rdf:List.</param>
		/// <exception cref="ArgumentNullException">typeNode is a null reference</exception>
		public OwlCollection(string nodeUri, OwlNode typeNode)
		{
			if(typeNode == null)
				throw(new ArgumentNullException());
			_items = new ArrayList();
			ID = nodeUri;
			_typeEdge = new OwlEdge(OwlNamespaceCollection.RdfNamespace+"type");
			_typeEdge.AttachChildNode(typeNode);
			AttachChildEdge(_typeEdge);
		}

		/// <summary>
		/// Initializes a new instance of the OwlCollection class with the given Uri
		/// </summary>
		/// <param name="nodeUri">A string representing the URI of this Resource</param>
		/// <remarks>This constructor creates a new OwlNode with URI rdf:List and sets it as the child node of an edge with URI rdf:type</remarks>	
		public OwlCollection(string nodeUri)
		{
			ID = nodeUri;
			_items = new ArrayList();
//			_typeEdge = new OwlEdge(OwlNamespaceCollection.RdfNamespace+"type");
//			_typeEdge.AttachChildNode(new OwlNode(OwlNamespaceCollection.RdfNamespace+"List"));
//			AttachChildEdge(_typeEdge);
		}

		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the node at the specified index
		/// </summary>
		public IOwlNode this[int index]
		{
			get
			{
				return (OwlNode)_items[index];
			}
			set
			{
				_items[index] = value;
			}
		}

		/// <summary>
		/// Gets the number of nodes stored in this collection
		/// </summary>
		public int Count
		{
			get
			{
				return _items.Count;
			}
		}

		#endregion

		#region Accessors
		/// <summary>
		/// Gets an enumerator that can iterate through this collection
		/// </summary>
		/// <returns>An object that implements the IEnumerator interface</returns>
		public virtual IEnumerator GetEnumerator()
		{
			return _items.GetEnumerator();
		}

		#endregion

		#region Manipulators
		/// <summary>
		/// Adds an OwlNode to this collection
		/// </summary>
		/// <param name="newNode">The Node to add.</param>
		/// <exception cref="ArgumentNullException">newNode is a null reference.</exception>
		public void Add(IOwlNode newNode)
		{
			if(newNode == null)
				throw (new ArgumentNullException());
			_items.Add(newNode);
		}

		/// <summary>
		/// Removes a node object from the collection
		/// </summary>
		/// <param name="node">The node to remove</param>
		/// <returns>True if the node was successfully removed</returns>
		///	<remarks>If the node exists then it is removed by calling the ArrayList.Remove method which is an O(n) operation.</remarks>
		public void Remove(IOwlNode node)
		{
			if(node == null)
				throw (new ArgumentNullException());
			_items.Remove(node);
		}

		/// <summary>
		/// Removes all the nodes from this collection
		/// </summary>
		public void RemoveAll()
		{
			_items.Clear();
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
