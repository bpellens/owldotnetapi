/*****************************************************************************
 * OwlResource.cs
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
using System.Text;
using System.Runtime.InteropServices;

namespace OwlDotNetApi
{
	/// <summary>
	/// Represents a resource in the OWL Graph
	/// </summary>
	public abstract class OwlResource : OwlNode, IOwlResource
	{
		#region Variables
		/// <summary>
		/// The child edge that connects to a node specifying the type of the resource
		/// </summary>
		protected OwlEdge _typeEdge;

		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the node that specifies the type of this resource
		/// </summary>
		/// <exception cref="ArgumentNullException">The specified value id null.</exception>
		public IOwlEdge Type
		{
			get
			{
				return _typeEdge;
			}
		}

		#endregion

		#region Manipulators
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
