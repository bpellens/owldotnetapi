/*****************************************************************************
 * OwlGraph.cs
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
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace OwlDotNetApi
{
	/// <summary>
	/// Represents an OWL Graph. 
	/// </summary>
	/// <remarks>This is an implementation of the IOwlGraph interface. This class maintain a collection of Nodes and a separate collection of Litrerals
	/// in order to allow distinguishing between Nodes and Literals with the same value.</remarks>
	public class OwlGraph : IOwlGraph
	{
		#region Variables
		/// <summary>
		/// The namespaces associated with this OWL Graph
		/// </summary>
		private OwlNamespaceCollection _nameSpaces;

		/// <summary>
		/// The collection of OwlEdges in this Graph
		/// </summary>
		private OwlEdgeCollection _edges;

		/// <summary>
		/// The collection of OwlNodes in this Graph
		/// </summary>
		private OwlNodeCollection _nodes;

		/// <summary>
		/// The collection of literals in this OWL Graph
		/// </summary>
		private OwlNodeCollection _literals;

		#endregion
		
		#region Creators
		/// <summary>
		/// Initializes a new instance of the OwlGraph class.
		/// </summary>
		public OwlGraph()
		{
			_nodes = new OwlNodeCollection();
			_literals = new OwlNodeCollection();
			_nameSpaces = new OwlNamespaceCollection();
			_edges = new OwlEdgeCollection();
		}

		#endregion

		#region Properties
		/// <summary>
		/// Gets the namespaces associated with this OWL Graph
		/// </summary>
		public IOwlNamespaceCollection NameSpaces
		{
			get
			{
				return _nameSpaces;
			}
		}
		
		/// <summary>
		/// Gets the collection of OwlEdges in this Graph
		/// </summary>
		public IOwlEdgeCollection Edges
		{
			get
			{
				return (IOwlEdgeCollection)_edges;
			}
		}

		/// <summary>
		/// Gets the collection of OwlNodes in this Graph
		/// </summary>
		public IOwlNodeCollection Nodes
		{
			get
			{
				return (IOwlNodeCollection)_nodes;
			}
		}

		/// <summary>
		/// Gets the collection of literals in this OWL Graph
		/// </summary>
		public IOwlNodeCollection  Literals
		{
			get
			{
				return (IOwlNodeCollection)_literals;
			}
		}

		/// <summary>
		/// Gets the total number of nodes and literals in this OWL Graph
		/// </summary>
		public long Count
		{
			get
			{
				return _nodes.Count+_literals.Count;
			}
		}

		/// <summary>
		/// Gets the node (or literal) with the specified URI
		/// </summary>
		/// <remarks>This method looks for a node that matches the specified URI and returns it. 
		/// If the node is not found then the first literal matching this URI (value+langiuageID+datatype URI) is returned.
		/// If neither a node or a literal matching this ID is found then null is returned.</remarks>
		public IOwlNode this[string nodeID]
		{
			get
			{
				IOwlNode node = _nodes[nodeID];
				if(node == null)
					node = _literals[nodeID];
				return node;
			}
		}

		#endregion

		#region Manipulators
		/// <summary>
		/// Adds a node to the Graph
		/// </summary>
		/// <param name="nodeUri">A string representing the URI of the new node.</param>
		/// <exception cref="UriFormatException">The specified nodeUri is not a well formed URI.</exception>
		/// <returns>An object that implements the IOwlNode interface. This is a reference to the new node added.
		/// This method checks the graph to determine whether the node with the specified URI exists. 
		/// If it does then a reference to the existing node is returned. If it does not exist then a new node is created, added 
		/// to the graph and returned.</returns>
		public IOwlNode AddNode(string nodeUri)
		{
			IOwlNode node = _nodes[nodeUri];
			if(node == null)
			{
				node = new OwlNode(nodeUri);
				_nodes[nodeUri] = node;
			}
			return node;
		}

		/// <summary>
		/// Adds a new node to the graph.
		/// </summary>
		/// <param name="node">An object that implements the IOwlNode interface. This is the new node to add.</param>
		/// <exception cref="ArgumentException">A node with the same ID already exists in the Graph.</exception>
		public void AddNode(IOwlNode node)
		{
			_nodes.Add((OwlNode)node);
		}

		/// <summary>
		/// Adds an edge to the graph
		/// </summary>
		/// <param name="edge">An object that implements the IOwlEdge interface</param>
		/// <exception cref="ArgumentNullException">The specified edge object is a null reference</exception>
		public void AddEdge(IOwlEdge edge)
		{
			_edges.Add((OwlEdge)edge);
		}

		/// <summary>
		/// Adds a literal to the Graph
		/// </summary>
		/// <param name="literalValue">A string representing the value of the literal.</param>
		/// <returns>An object that implements the IOwlLiteral interface. This is a reference to the newly added literal.</returns>
		/// <remarks>This method looks in the graph to determine whether a literal with the specified value (and a null datatype and langID) exists
		/// in the Graph. If the literal exists a reference to the existing literal is returned. If it does not exist then a new literal (with the specified value, and null datatype and LangID)
		/// is created, added to the graph and returned.</remarks>
		public IOwlLiteral AddLiteral(string literalValue)
		{
			OwlLiteral literal = (OwlLiteral)_literals[literalValue];
			if(literal == null)
			{
				literal = new OwlLiteral(literalValue);
				_literals[literalValue] = literal;
			}
			return literal;
		}

		/// <summary>
		/// Adds a literal to the Graph
		/// </summary>
		/// <param name="literalValue">A string representing the value of the literal.</param>
		/// <param name="langID">A string representing the Language ID of the literal.</param>
		/// <param name="datatypeUri">A string representing the datatype URI of the literal.</param>
		/// <returns>An object that implements the IOwlLiteral interface. This is a reference to the newly added literal.</returns>
		/// <exception cref="UriFormatException">The specified datatype URI is not null and is not a well formed URI.</exception>
		/// <remarks>This method looks in the graph to determine whether a literal with the specified value, datatype and langID exists
		/// in the Graph. If the literal exists a reference to the existing literal is returned. If it does not exist then a new literal with the specified value, datatype and LangID
		/// is created, added to the graph and returned. Any parameter supplied to this method, except literalValue, can be null or empty and it will be ignored.</remarks>
		public IOwlLiteral AddLiteral(string literalValue, string langID, string datatypeUri)
		{
			string literalID = literalValue;
			if((langID != null) && (langID.Length != 0))
				literalID +="@"+langID;
			if((datatypeUri != null) && (datatypeUri.Length != 0))
				literalID +="^^"+datatypeUri;
			OwlLiteral literal = (OwlLiteral)_literals[literalID];
			if(literal == null)
			{
				literal = new OwlLiteral(literalValue,langID,datatypeUri);
				_literals[literalID] = literal;
			}
			return literal;
		}

		/// <summary>
		/// Adds a literal to the graph
		/// </summary>
		/// <param name="literal">The new literal to add.</param>
		/// <exception cref="ArgumentException">A literal with the same value, datatype URI and language ID alreday exists in the graph.</exception>
		public void AddLiteral(IOwlLiteral literal)
		{
			_literals.Add((OwlLiteral)literal);
		}

		/// <summary>
		/// Merges the srcGraph into this graph object
		/// </summary>
		/// <param name="srcGraph">An object that implements the IOwlGraph interace</param>
		/// <param name="skipDuplicateEdges">A flag that indicates whether duplicate edges present in both graphs should be skipped during the merge process.</param>
		public void Merge(IOwlGraph srcGraph, bool skipDuplicateEdges)
		{
			if(srcGraph == null)
				return;
			Hashtable literalsAdded = new Hashtable();
			//go through all the nodes in the source graph
			IDictionaryEnumerator enumerator = (IDictionaryEnumerator)srcGraph.Nodes.GetEnumerator();
			while(enumerator.MoveNext())
			{
				//Console.WriteLine(((IOwlNode)(enumerator.Value)).ID);
				//add this node to the graph
				IOwlNode srcParentNode = (IOwlNode)enumerator.Value;
				IOwlNode destParentNode = AddNode(srcParentNode.ID);
				//go through all of the src node's child edges
				foreach(IOwlEdge srcChildEdge in srcParentNode.ChildEdges)
				{
					//for each of the src node's child edges do...
					IOwlNode destChildNode;
					if(srcChildEdge.ChildNode is IOwlLiteral)
					{	
						IOwlLiteral srcChildLiteral = srcChildEdge.ChildNode as IOwlLiteral;
						literalsAdded[srcChildLiteral] = srcChildLiteral;
						destChildNode = AddLiteral(srcChildLiteral.Value, srcChildLiteral.LangID, srcChildLiteral.Datatype);
					}
					else
					{
						destChildNode = AddNode(srcChildEdge.ChildNode.ID);
					}

					//Now we have the parent and the child nodes added to the graph..
					
					bool edgeExists = false;
					if(skipDuplicateEdges)
					{
						//does the new parent node and the new child node have an edge with the same ID as srcChildEdge?
						//go through all the child edges of destParentNode
						foreach(OwlEdge tempEdge in destParentNode.ChildEdges)
						{
							if((tempEdge.ChildNode == destChildNode) && (tempEdge.ID == srcChildEdge.ID))
							{
								edgeExists = true;
								break;
							}
						}
					}
					if(!skipDuplicateEdges || (skipDuplicateEdges && !edgeExists))
					{
						OwlEdge destChildEdge = new OwlEdge(srcChildEdge.ID);
						destParentNode.AttachChildEdge(destChildEdge);
						destChildEdge.AttachChildNode(destChildNode);
						//add the edge to the graph
						AddEdge(destChildEdge);
					}
				}
			}
		}
		#endregion
	}
}

