/*****************************************************************************
 * IOwlEdge.cs
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
	/// Defines a generalized mechanism for processing edges in the OWL Graph
	/// </summary>
	public interface IOwlEdge
	{
		/// <summary>
		/// When implemented by a class, gets or sets the Child Node of this IOwlEdge
		/// </summary>
		IOwlNode ChildNode
		{
			get;
			set;
		}
		/// <summary>
		/// When implemented by a class, gets or sets the Parent Node of this IOwlEdge
		/// </summary>
		IOwlNode ParentNode
		{
			get;
			set;
		}
		/// <summary>
		/// When implemented by a class, gets or sets the ID of this IOwlEdge
		/// </summary>
		string ID
		{
			get;
			set;
		}

		/// <summary>
		/// When implemented by a class, gets or sets the Language ID of this IOwlEdge
		/// </summary>
		string LangID
		{
			get;
			set;
		}

		/// <summary>
		/// When implemented by a class, attaches a Child Node to this IOwlEdge
		/// </summary>
		/// <param name="node">The IOwlNode to attach</param>
		void AttachChildNode(IOwlNode node);
		/// <summary>
		/// When implemented by a class, attaches a Parent Node to this IOwlEdge
		/// </summary>
		/// <param name="node">The IOwlNode to attach</param>
		void AttachParentNode(IOwlNode node);

		/// <summary>
		/// When implemented by a class, detachs the Child Node of this IOwlEdge
		/// </summary>
		/// <returns>The removed IOwlNode</returns>
		IOwlNode DetachChildNode();

		/// <summary>
		/// When implemented by a class, detaches the Parent Node of this IOwlEdge
		/// </summary>
		/// <returns>The removed IOwlNode</returns>
		IOwlNode DetachParentNode();

		/// <summary>
		/// When implemented by a class, it accepts a message and retransfers it back to the visitor
		/// </summary>
		/// <param name="visitor">The actual visitor in the pattern</param>
		/// <param name="parent">The parent object of the edge to be generated</param>
		void Accept(IOwlVisitor visitor, Object parent);
	}
}
