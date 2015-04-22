/*****************************************************************************
 * IOwlXmlGenerator.cs
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
using System.IO;
using System.Xml;

namespace OwlDotNetApi
{
	/// <summary>
	/// Represents an OWL Xml Generator
	/// </summary>
	public interface IOwlXmlGenerator : IOwlGenerator
	{
		/// <summary>
		/// When implemented by a class, it generates the OWL graph into an existing Xml Document
		/// </summary>
		/// <param name="graph">An object that implements the IOwlGraph interface which needs to be generated</param>
		/// <param name="doc">The XmlDocument to use as a destination document</param>
		void GenerateOwl(IOwlGraph graph, XmlDocument doc);
	}
}
