/*****************************************************************************
 * OwlEdge.cs
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
	/// Represents an Edge in the OWL Graph
	/// </summary>
	public class OwlEdge : IOwlEdge
	{
		#region Variables
		/// <summary>
		/// Static value representing the value for a directed edge.
		/// </summary>
		public static bool DIRECTED = true;

		/// <summary>
		/// Static value representing the value for an undirected edge.
		/// </summary>
		public static bool UNDIRECTED = false;

		/// <summary>
		/// The status of the edge. It can be either directed or undirected.
		/// </summary>
		private bool _directed;

		/// <summary>
		/// The parent node of this edge
		/// </summary>
		private OwlNode _parentNode;

		/// <summary>
		/// The child node of this edge
		/// </summary>
		private OwlNode _childNode;

		/// <summary>
		/// The URI of this edge
		/// </summary>
		private string _edgeID;

		/// <summary>
		/// The language ID of this edge
		/// </summary>
		private string _langID;

		#endregion

		#region Creators
		/// <summary>
		/// Initializes a new instance of the OwlEdge class. Sets the ID, ChildNode and ParentNode properties to null
		/// </summary>
		public OwlEdge()
		{
			_edgeID = "";
			_childNode = null;
			_parentNode = null;
		}

		/// <summary>
		/// Initializes a new instance of the OwlEdge class with the given URI
		/// </summary>
		/// <param name="edgeUri">A string representing the Uri of this edge. The ChildNode and ParentNode properties are set to null</param>
		/// <exception cref="UriFormatException">The specified edgeUri was not a well formed URI</exception>
		public OwlEdge(string edgeUri)
		{
			Uri u = new Uri(edgeUri);
			_edgeID = edgeUri;
			_childNode = null;
			_parentNode = null;
		}

		#endregion

		#region Properties		
		/// <summary>
		/// Gets or sets the status of the edge
		/// </summary>
		public bool Directed
		{
			get
			{
				return _directed;
			}
			set
			{
				_directed = (bool)value;
			}
		}
		
		/// <summary>
		/// Gets or sets the parent node of this edge
		/// </summary>
		public IOwlNode ParentNode
		{
			get
			{
				return _parentNode;
			}
			set
			{
				_parentNode = (OwlNode)value;
			}
		}

		/// <summary>
		/// Gets or sets the child node of this edge
		/// </summary>
		public IOwlNode ChildNode
		{
			get
			{
				return _childNode;
			}
			set
			{
				_childNode = (OwlNode)value;
			}
		}

		/// <summary>
		/// Gets or sets the URI of this edge
		/// </summary>
		/// <exception cref="UriFormatException">The specified value is a null reference</exception>
		public string ID
		{
			get
			{
				return _edgeID;
			}
			set
			{
				Uri u = new Uri(value);
				_edgeID = value;
			}
		}

		/// <summary>
		/// Gets or sets the Language ID of this edge
		/// </summary>
		/// <remarks>This language ID is inherited by all child nodes and edges unless overridden</remarks>
		public string LangID
		{
			get
			{
				return _langID;
			}
			set
			{
				_langID = value;
			}
		}
		
		#endregion

		#region Manipulators
		/// <summary>
		/// Attaches a Child node to this edge
		/// </summary>
		/// <param name="node">The node to attach</param>
		/// <exception cref="ArgumentNullException">The specified node is a null reference</exception>
		public void AttachChildNode(IOwlNode node)
		{
			if(node == null)
				throw (new ArgumentNullException());
			DetachChildNode();
			ChildNode = node;
			node.ParentEdges.Add(this);
		}

		/// <summary>
		/// Detaches the child node
		/// </summary>
		/// <returns>The newly detached child node. Returns null if no child node was present</returns>
		public IOwlNode DetachChildNode()
		{
			IOwlNode node = ChildNode;
			if(ChildNode != null)
				ChildNode.DetachParentEdge(this);
			return node;
		}

		/// <summary>
		/// Attaches a parent node to this edge
		/// </summary>
		/// <param name="node">The node to attach</param>
		/// <exception cref="ArgumentNullException">The specified node is a null reference</exception>
		public void AttachParentNode(IOwlNode node)
		{
			if(node == null)
				throw (new ArgumentNullException());
			DetachParentNode();
			ParentNode = node;
			node.ChildEdges.Add(this);
		}

		/// <summary>
		/// Detaches the parent node from this edge
		/// </summary>
		/// <returns>The newly detached parent node. Returns null if no parent node was present</returns>
		public IOwlNode DetachParentNode()
		{
			IOwlNode node = ParentNode;
			if(ParentNode != null)
				ParentNode.DetachChildEdge(this);
			return node;
		}

		/// <summary>
		/// The virtual accept method which needs to be overridden by the subclasses in order to have a functional visitor
		/// </summary>
		/// <param name="visitor">The visitor used in the generator</param>
		/// <param name="parent">The parent object of the edge to be generated</param>
		public virtual void Accept(IOwlVisitor visitor, Object parent) 
		{
			visitor.Visit(this, parent);
		}

		#endregion
	}
}
