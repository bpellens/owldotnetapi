/*****************************************************************************
 * IOwlVisitor.cs
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
	/// This interface defines the type of object that the nodes and edges will 
	/// accept. The Node hierarchy classes call back a Visiting 
	/// object's Visit() methods; In so doing they identify their 
	/// type. Implementors of this interface can create algorithms 
	/// that operate differently on different type of Nodes. 
	/// </summary>
	public interface IOwlVisitor
	{	
		/// <summary>
		/// Visit function to generate a regular node, used in the visitor patterns
		/// </summary>
		/// <param name="node">The actual node which needs to be generated</param>
		/// <param name="parent">The parent object of the node</param>
		void Visit(OwlNode node, Object parent);

		/// <summary>
		/// Visit function to generate the code for an edge.
		/// </summary>
		/// <param name="edge">The actual edge which needs to be generated</param>
		/// <param name="parent">The parent object of the edge</param>
		void Visit(OwlEdge edge, Object parent);

		/// <summary>
		/// Visit function to generate some output, used in the visitor pattern
		/// </summary>
		/// <param name="node">The actual node which needs to be generated</param>
		/// <param name="parent">The parent object of the node</param>
		void Visit(OwlLiteral node, Object parent);

		/// <summary>
		/// Visit function to generate some output, used in the visitor pattern
		/// </summary>
		/// <param name="node">The actual node which needs to be generated</param>
		/// <param name="parent">The parent object of the node</param>
		void Visit(OwlClass node, Object parent);

		/// <summary>
		/// Visit function to generate some output, used in the visitor pattern
		/// </summary>
		/// <param name="node">The actual node which needs to be generated</param>
		/// <param name="parent">The parent object of the node</param>
		void Visit(OwlDataRange node, Object parent);

		/// <summary>
		/// Visit function to generate some output, used in the visitor pattern
		/// </summary>
		/// <param name="node">The actual node which needs to be generated</param>
		/// <param name="parent">The parent object of the node</param>
		void Visit(OwlAnnotationProperty node, Object parent);

		/// <summary>
		/// Visit function to generate some output, used in the visitor pattern
		/// </summary>
		/// <param name="node">The actual node which needs to be generated</param>
		/// <param name="parent">The parent object of the node</param>
		void Visit(OwlDatatype node, Object parent);

		/// <summary>
		/// Visit function to generate some output, used in the visitor pattern
		/// </summary>
		/// <param name="node">The actual node which needs to be generated</param>
		/// <param name="parent">The parent object of the node</param>
		void Visit(OwlDatatypeProperty node, Object parent);

		/// <summary>
		/// Visit function to generate some output, used in the visitor pattern
		/// </summary>
		/// <param name="node">The actual node which needs to be generated</param>
		/// <param name="parent">The parent object of the node</param>
		void Visit(OwlIndividual node, Object parent);

		/// <summary>
		/// Visit function to generate some output, used in the visitor pattern
		/// </summary>
		/// <param name="node">The actual node which needs to be generated</param>
		/// <param name="parent">The parent object of the node</param>
		void Visit(OwlObjectProperty node, Object parent);

		/// <summary>
		/// Visit function to generate some output, used in the visitor pattern
		/// </summary>
		/// <param name="node">The actual node which needs to be generated</param>
		/// <param name="parent">The parent object of the node</param>
		void Visit(OwlOntology node, Object parent);

		/// <summary>
		/// Visit function to generate some output, used in the visitor pattern
		/// </summary>
		/// <param name="node">The actual node which needs to be generated</param>
		/// <param name="parent">The parent object of the node</param>
		void Visit(OwlOntologyProperty node, Object parent);

		/// <summary>
		/// Visit function to generate some output, used in the visitor pattern
		/// </summary>
		/// <param name="node">The actual node which needs to be generated</param>
		/// <param name="parent">The parent object of the node</param>
		void Visit(OwlProperty node, Object parent);

		/// <summary>
		/// Visit function to generate some output, used in the visitor pattern
		/// </summary>
		/// <param name="node">The actual node which needs to be generated</param>
		/// <param name="parent">The parent object of the node</param>
		void Visit(OwlRestriction node, Object parent);

		/// <summary>
		/// Visit function to generate some output, used in the visitor pattern
		/// </summary>
		/// <param name="node">The actual node which needs to be generated</param>
		/// <param name="parent">The parent object of the node</param>
		void Visit(OwlResource node, Object parent);

		/// <summary>
		/// Visit function to generate some output, used in the visitor pattern
		/// </summary>
		/// <param name="node">The actual node which needs to be generated</param>
		/// <param name="parent">The parent object of the node</param>
		void Visit(OwlCollection node, Object parent);
		
	}
}
