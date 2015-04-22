/*****************************************************************************
 * IOwlGraph.cs
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

namespace OwlDotNetApi
{
	/// <summary>
	/// Reprsents an OWL Graph comprising Nodes and Literals connected by Edges
	/// </summary>
	public interface IOwlGraph
	{
		/// <summary>
		/// When implemented by a class, gets the namespaces associated with this OWL Graph
		/// </summary>
		IOwlNamespaceCollection NameSpaces
		{
			get;
		}

		/// <summary>
		/// When implemented by a class, gets the collection of Nodes in this Graph
		/// </summary>
		IOwlNodeCollection Nodes
		{
			get;
		}

		/// <summary>
		/// When implemented by a class, gets the collection of Literals in this Graph
		/// </summary>
		IOwlNodeCollection Literals
		{
			get;
		}

		/// <summary>
		/// When implemented by a class, gets the number of nodes in this OWL Graph
		/// </summary>
		long Count
		{
			get;
		}

		/// <summary>
		/// When implementsd by a class, gets a collection of edges in this OwlGraph
		/// </summary>
		IOwlEdgeCollection Edges
		{
			get;
		}

		/// <summary>
		/// When implemented by a class, gets the node with the specified ID from this graph
		/// </summary>
		IOwlNode this[string nodeID]
		{
			get;
		}

		/// <summary>
		/// When implemented by a class, adds an edge to the OwlGraph
		/// </summary>
		/// <param name="edge">An object that implements the IOwlEdge interface</param>
		void AddEdge(IOwlEdge edge);

		/// <summary>
		/// When implemented by a class, adds a new node to the OwlGraph
		/// </summary>
		/// <param name="nodeUri">A string representing the Uri of the new node</param>
		/// <returns>The newly added node</returns>
		IOwlNode AddNode(string nodeUri);

		/// <summary>
		/// When implemented by a class, adds a new node to the OwlGraph 
		/// </summary>
		/// <param name="node">The IOwlNode to add</param>
		void AddNode(IOwlNode node);

		/// <summary>
		/// When implemented by a class, adds a new literal to the OwlGraph
		/// </summary>
		/// <param name="literalValue">A string representing the value of the new literal</param>
		/// <returns>The newly added IOwlLiteral</returns>
		IOwlLiteral AddLiteral(string literalValue);

		/// <summary>
		/// When implemented by a class, adds a new literal to the OwlGraph
		/// </summary>
		/// <param name="datatypeUri">A string representing the URI that specifies the datatype of the new literal</param>
		/// <param name="langID">A string representing the Language ID of the new Literal</param>
		/// <param name="literalValue">A string representing the value of the new Literal</param>
		/// <returns>The newly added IOwlLiteral</returns>
		IOwlLiteral AddLiteral(string literalValue, string langID, string datatypeUri);

		/// <summary>
		/// When implemented by a class, adds a new literal to the OwlGraph
		/// </summary>
		/// <param name="literal">The IOwlLiteral to add</param>
		void AddLiteral(IOwlLiteral literal);
	}
}
