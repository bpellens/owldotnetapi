/*****************************************************************************
 * OwlGenerator.cs
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
	/// Summary description for OwlGenerator.
	/// </summary>
	public abstract class OwlGenerator : IOwlVisitor
	{
		#region Variables
		/// <summary>
		/// List of warning messages generated while generating the OWL/XML
		/// </summary>
		protected ArrayList _warnings;

		/// <summary>
		/// List of error messages generated while generating the OWL/XML
		/// </summary>
		protected ArrayList _errors;

		/// <summary>
		/// List of information messages generated while generating the OWL/XML
		/// (used for testing purposes
		/// </summary>
		protected ArrayList _messages;

		/// <summary>
		/// Indicates whether the parser should throw an exception and stop when it encounters an error
		/// </summary>
		protected bool _stopOnErrors;

		/// <summary>
		/// Indicates whether the parser should throw an exception and stop when it generates a warnung
		/// </summary>
		protected bool _stopOnWarnings;

		#endregion

		#region Properties
		/// <summary>
		/// List of warning messages generated while generating the OWL/XML
		/// </summary>
		public ArrayList Warnings
		{
			get
			{
				return _warnings;
			}
		}

		/// <summary>
		/// List of error messages generated while generating the OWL/XML
		/// </summary>
		public ArrayList Errors
		{
			get
			{
				return _errors;
			}
		}

		/// <summary>
		/// List of information messages generated while generating the OWL/XML
		/// </summary>
		public ArrayList Messages
		{
			get
			{
				return _messages;
			}
		}

		/// <summary>
		/// Gets or sets a value that indicates whether the generator should throw an exception and stop when it encounters an error
		/// </summary>
		public bool StopOnErrors
		{
			get
			{
				return _stopOnErrors;
			}
			set
			{
				_stopOnErrors = value;
			}
		}

		/// <summary>
		/// Gets or sets a value that indicates whether the generator should throw an exception and stop when it generates a warning
		/// </summary>
		public bool StopOnWarnings
		{
			get
			{
				return _stopOnWarnings;
			}
			set
			{
				_stopOnWarnings = value;
			}
		}

		#endregion

		#region Manipulators
		/// <summary>
		/// Called by the generator when an error is encountered.
		/// </summary>
		/// <param name="msg">The error message associated with the error</param>
		/// <exception cref="InvalidOwlException">ExceptionsOnError is set to true</exception>
		protected void OnError(string msg)
		{
			Errors.Add(msg);
			if(StopOnErrors)
				OnError(new InvalidOwlException(msg));
		}

		/// <summary>
		/// Called by the generator when an error is encountered.
		/// </summary>
		/// <param name="e">The exception to throw.</param>
		/// <remarks>If ExceptionsOnError is set to true then the specified Exception is thrown. 
		/// If ExceptionsOnError is set to false then the error message from the exception is output to standard output</remarks>
		protected void OnError(Exception e)
		{
			throw(e);
		}

		/// <summary>
		/// Called by the generator when a warning is generated.
		/// </summary>
		/// <param name="msg">The message associated with the warning</param>
		/// <exception cref="InvalidOwlException">ExceptionsOnWarnings is set to true</exception>
		protected void OnWarning(string msg)
		{
			Warnings.Add(msg);
			if(StopOnWarnings)
				OnWarning(new InvalidOwlException(msg));
		}

		/// <summary>
		/// Called by the generator when a warning is generated. 
		/// </summary>
		/// <param name="e">The exception to throw if ExceptionsOnWarnings is true</param>
		/// <remarks>If ExceptionsOnWarnings is set to true then the specified exception is thrown.
		/// If ExceptionsOnWarnings is set to false then the error message from the exception is output to standard output</remarks>
		protected void OnWarning(Exception e)
		{
			throw(e);
		}

		/// <summary>
		/// To be called when particular testing information is to be written
		/// to a log file
		/// </summary>
		/// <param name="msg">The log item</param>
		protected void OnMessage(string msg)
		{
			Messages.Add(msg);
		}

		/// <summary>
		/// Determines whether the specified URI string is a well formed URI
		/// </summary>
		/// <param name="uriString">A string</param>
		/// <returns>True if the specified string is a well formed URI.</returns>
		protected bool IsValidUri(string uriString)
		{
			if((uriString == null) || (uriString.Length == 0))
				return false;
			try
			{
				Uri u = new Uri(uriString);
				return true;
			}
			catch(UriFormatException)
			{
				return false;
			}
		}

		/// <summary>
		/// Implementation of the visit function to generate some output, used in the visitor pattern
		/// </summary>
		/// <param name="node">The actual node which needs to be generated</param>
		/// <param name="parent">The parent object of the node</param>
		public abstract void Visit(OwlNode node, Object parent);

		/// <summary>
		/// Implementation of the visit function to generate some output, used in the visitor pattern
		/// </summary>
		/// <param name="edge">The actual edge which needs to be generated</param>
		/// <param name="parent">The parent object of the edge</param>
		public abstract void Visit(OwlEdge edge, Object parent);

		/// <summary>
		/// Implementation of the visit function to generate some output, used in the visitor pattern
		/// </summary>
		/// <param name="node">The actual node which needs to be generated</param>
		/// <param name="parent">The parent object of the node</param>
		public abstract void Visit(OwlClass node, Object parent);

		/// <summary>
		/// Implementation of the visit function to generate some output, used in the visitor pattern
		/// </summary>
		/// <param name="node">The actual node which needs to be generated</param>
		/// <param name="parent">The parent object of the node</param>
		public abstract void Visit(OwlDataRange node, Object parent);

		/// <summary>
		/// Implementation of the visit function to generate some output, used in the visitor pattern
		/// </summary>
		/// <param name="node">The actual node which needs to be generated</param>
		/// <param name="parent">The parent object of the node</param>
		public abstract void Visit(OwlLiteral node, Object parent);

		/// <summary>
		/// Implementation of the visit function to generate some output, used in the visitor pattern
		/// </summary>
		/// <param name="node">The actual node which needs to be generated</param>
		/// <param name="parent">The parent object of the node</param>
		public abstract void Visit(OwlAnnotationProperty node, Object parent);

		/// <summary>
		/// Implementation of the visit function to generate some output, used in the visitor pattern
		/// </summary>
		/// <param name="node">The actual node which needs to be generated</param>
		/// <param name="parent">The parent object of the node</param>
		public abstract void Visit(OwlDatatype node, Object parent);

		/// <summary>
		/// Implementation of the visit function to generate some output, used in the visitor pattern
		/// </summary>
		/// <param name="node">The actual node which needs to be generated</param>
		/// <param name="parent">The parent object of the node</param>
		public abstract void Visit(OwlDatatypeProperty node, Object parent);

		/// <summary>
		/// Implementation of the visit function to generate some output, used in the visitor pattern
		/// </summary>
		/// <param name="node">The actual node which needs to be generated</param>
		/// <param name="parent">The parent object of the node</param>
		public abstract void Visit(OwlIndividual node, Object parent);

		/// <summary>
		/// Implementation of the visit function to generate some output, used in the visitor pattern
		/// </summary>
		/// <param name="node">The actual node which needs to be generated</param>
		/// <param name="parent">The parent object of the node</param>
		public abstract void Visit(OwlObjectProperty node, Object parent);

		/// <summary>
		/// Implementation of the visit function to generate some output, used in the visitor pattern
		/// </summary>
		/// <param name="node">The actual node which needs to be generated</param>
		/// <param name="parent">The parent object of the node</param>
		public abstract void Visit(OwlOntology node, Object parent);

		/// <summary>
		/// Implementation of the visit function to generate some output, used in the visitor pattern
		/// </summary>
		/// <param name="node">The actual node which needs to be generated</param>
		/// <param name="parent">The parent object of the node</param>
		public abstract void Visit(OwlOntologyProperty node, Object parent);

		/// <summary>
		/// Implementation of the visit function to generate some output, used in the visitor pattern
		/// </summary>
		/// <param name="node">The actual node which needs to be generated</param>
		/// <param name="parent">The parent object of the node</param>
		public abstract void Visit(OwlProperty node, Object parent);

		/// <summary>
		/// Implementation of the visit function to generate some output, used in the visitor pattern
		/// </summary>
		/// <param name="node">The actual node which needs to be generated</param>
		/// <param name="parent">The parent object of the node</param>
		public abstract void Visit(OwlRestriction node, Object parent);

		/// <summary>
		/// Implementation of the visit function to generate some output, used in the visitor pattern
		/// </summary>
		/// <param name="node">The actual node which needs to be generated</param>
		/// <param name="parent">The parent object of the node</param>
		public abstract void Visit(OwlResource node, Object parent);

		/// <summary>
		/// Implementation of the visit function to generate some output, used in the visitor pattern
		/// </summary>
		/// <param name="node">The actual node which needs to be generated</param>
		/// <param name="parent">The parent object of the node</param>
		public abstract void Visit(OwlCollection node, Object parent);

		#endregion
	}
}
