/*****************************************************************************
 * OwlXmlParser.cs
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
using System.Xml;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace OwlDotNetApi
{
	/// <summary>
	/// The primary OWL Parser.
	/// </summary>
	public class OwlXmlParser : OwlParser , IOwlXmlParser
	{
		#region Variables
		/// <summary>
		/// A Hashtable that stores declared rdf:ID values for quick lookup of duplicates
		/// </summary>
		private Hashtable _declID;

		/// <summary>
		/// Variable used to generate new IDs for OWL Nodes
		/// </summary>
		private long _newID;
		private long _dummyID;

		/// <summary>
		/// A Collection of non syntactic elements in the OWL syntax
		/// </summary>
		private Hashtable _nonSyntacticElements;

		/// <summary>
		/// A Collection of syntactic elements in the OWL syntax
		/// </summary>
		private Hashtable _syntacticElements;

		/// <summary>
		/// A Collection of all RDF and XML properties
		/// </summary>
		/// <remarks>The members of this collection are rdf:about, rdf:resource, rdf:parseType, rdf:ID, rdf:nodeID, rdf:datatype, rdf:value, xml:lang, xml:base</remarks>
		private Hashtable _rdfXmlProperties;

		#endregion

		#region Creators
		/// <summary>
		/// Initializes an instance of the RdfParser class
		/// </summary>
		public OwlXmlParser()
		{
			_owlGraph = null;
			_warnings = new ArrayList();
			_errors = new ArrayList();
			_messages = new ArrayList();
			_newID = 10000;
			_dummyID = 10000;

			string[] rdfXmlAttrs = {"rdf:about", "rdf:resource", "rdf:parseType", "rdf:ID", "rdf:nodeID", "rdf:type", "rdf:datatype", "rdf:value", "xml:lang", "xml:base"};
			_rdfXmlProperties = new Hashtable();
			int len = rdfXmlAttrs.Length;
			for(int i=0;i<len;i++)
				_rdfXmlProperties.Add(rdfXmlAttrs[i],rdfXmlAttrs[i]);

			string[] syntacticElements = {"rdf:RDF", "rdf:ID", "rdf:about", "rdf:resource", "rdf:parseType", "rdf:nodeID"};
			_syntacticElements = new Hashtable();
			len = syntacticElements.Length;
			for(int i=0;i<len;i++)
				_syntacticElements.Add(syntacticElements[i], syntacticElements[i]);

			string[] nonSyntacticElements = {"rdf:type", "rdf:value", "rdf:datatype", "rdf:List", "rdf:first", "rdf:rest", "rdf:nil", "rdfs:comment", "rdfs:subPropertyOf", "rdfs:domain", "rdfs:range", "rdfs:subClassOf", "owl:allValuesFrom", "owl:backwardCompatibleWith", "owl:cardinality", "owl:complementOf", "owl:differentFrom", "owl:disjointWith", "owl:distinctMembers", "owl:equivalentClass", "owl:equivalentProperty", "owl:hasValue", "owl:imports", "owl:incompatibleWith", "owl:intersectionOf", "owl:inverseOf", "owl:maxCardinality", "owl:minCardinality", "owl:oneOf", "owl:onProperty", "owl:priorVersion", "owl:sameAs", "owl:someValuesFrom", "owl:unionOf", "owl:versionInfo"};
			_nonSyntacticElements = new Hashtable();
			len = nonSyntacticElements.Length;
			for(int i=0;i<len;i++)
				_nonSyntacticElements.Add(nonSyntacticElements[i],nonSyntacticElements[i]);

			_declID = new Hashtable();

			StopOnErrors = false;
			StopOnWarnings = false;

		}

		#endregion

		#region Manipulators
		/// <summary>
		/// Parses the OWL from the given XmlDocument, into an existing graph using the given xml:base uri
		/// </summary>
		/// <param name="doc">The XmlDocument to use as the source of the XML data</param>
		/// <param name="graph">An object that implements the IOwlGraph interface</param>
		/// <param name="xmlbaseUri">The xml:base Uri to use incase one is not found in the XML data or the graph</param>
		/// <returns>An object that implements the IOwlGraph interface</returns>
		public IOwlGraph ParseOwl(XmlDocument doc, IOwlGraph graph, string xmlbaseUri)
		{
			//parses from the xml document
			//if doc is null throws an ArgumentNullException
			//looks for xml:base in doc
			//if xml:base is not found in doc then uses the xmlbaseUri 
			//if xmlbaseUri is not a valid Uri it defaults to http://unknown.org/

			if(doc == null)
				throw(new ArgumentNullException("The specified XmlDocument object is a null reference"));

			Warnings.Clear();
			Errors.Clear();
			//Start with the root
			XmlElement root = doc.DocumentElement;
			
			if(root.Name != "rdf:RDF")
			{
				if(root.Name.ToLower() == "rdf")
					OnWarning("Unqualified use of rdf as the root element name.");
				else
					OnWarning("Root element of an OWL document must be rdf:RDF");
			}
			string oldXmlbaseUri = null;
			if(graph == null)
			{
				//Now create the OwlGraph
				_owlGraph = new OwlGraph();
			}
			else
			{
				oldXmlbaseUri = graph.NameSpaces["xml:base"];
				graph.NameSpaces.Remove("xml:base");
				_owlGraph = graph;
			}

			//its an OWL Document so now get the namespace info
			int count = root.Attributes.Count;
			for(int i=0;i<count;i++)
			{
				try
				{
					string nsName = root.Attributes[i].Name;
					if(_owlGraph.NameSpaces[nsName] != null)
						OnWarning("Redefinition of namespace "+nsName);
					_owlGraph.NameSpaces[nsName] = root.Attributes[i].Value;
				}
				catch(ArgumentException ine)
				{
					OnWarning(ine.Message);
				}
			}
			
			string xbUri = _owlGraph.NameSpaces["xml:base"];
			if(xbUri == null)
			{
				xbUri = doc.BaseURI;
				if(!IsValidUri(xbUri))
				{
					xbUri = xmlbaseUri;
					if(!IsValidUri(xbUri))
					{
						if(oldXmlbaseUri != null)
							xbUri = oldXmlbaseUri;
						else
						{
							OnWarning("Valid xml:base URI not found. Using http://unknown.org/");
							xbUri = "http://unknown.org/";
						}
					}
				}
			}
			
			//ignore and discard everything after the first # character
			int pos = xbUri.IndexOf('#');
			if(pos != -1)
			{
				xbUri = xbUri.Remove(pos,xbUri.Length-pos);
			}
			//Now finally set the value of the xml:base Uri
			_owlGraph.NameSpaces["xml:base"] = xbUri;

			if(root.HasChildNodes)
			{
				count = root.ChildNodes.Count;
				for(int i=0;i<count;i++) 
				{
					ProcessNode(root.ChildNodes[i], null);
				}
			}

			return _owlGraph;
		}

		/// <summary>
		/// Parses the OWL from the given XmlDocument, into an existing graph
		/// </summary>
		/// <param name="doc">The XmlDocument to use as the source of the XML data</param>
		/// <param name="graph">An object that implements the IOwlGraph interface</param>
		/// <returns>An object that implements the IOwlGraph interface</returns>
		public IOwlGraph ParseOwl(XmlDocument doc, IOwlGraph graph)
		{
			return ParseOwl(doc,graph,null);
		}

		/// <summary>
		/// Parses the OWL from the given XmlDocument, using the given xml:base uri
		/// </summary>
		/// <param name="doc">The XmlDocument to use as the source of the XML data</param>
		/// <param name="xmlbaseUri">The xml:base Uri to use incase one is not found in the XML data</param>
		/// <returns>An object that implements the IOwlGraph interface</returns>
		public IOwlGraph ParseOwl(XmlDocument doc, string xmlbaseUri)
		{
			return ParseOwl(doc,null,xmlbaseUri);
		}

		/// <summary>
		/// Parses the OWL from the given XmlDocument
		/// </summary>
		/// <param name="doc">The XmlDocument to use as the source of the XML data</param>
		/// <returns>An object that implements the IOwlGraph interface</returns>
		public IOwlGraph ParseOwl(XmlDocument doc)
		{
			//parses from the xml document
			//if doc is null throws an ArgumentNullException
			//looks for xml:base in doc
			//if xml:base is not found then defaults to http://unknown.org/
			if(doc == null)
				throw(new ArgumentNullException("The specified XmlDocument is a null reference"));
			return ParseOwl(doc,null, null);
		}

		/// <summary>
		/// Parses the OWL from the given stream, into an existing graph using the given xml:base uri
		/// </summary>
		/// <param name="inStream">The Stream to use as the source of the XML data</param>
		/// <param name="graph">An object that implements the IOwlGraph interface</param>
		/// <param name="xmlbaseUri">The xml:base Uri to use incase one is not found in the XML data or the graph</param>
		/// <returns>An object that implements the IOwlGraph interface</returns>
		public IOwlGraph ParseOwl(Stream inStream, IOwlGraph graph, string xmlbaseUri)
		{
			//looks for xml:base in owl doc.
			//if not found, then uses xmlbaseUri
			//if xmlbaseUri is not a valid Uri it defaults to http://unknown.org/
			if(inStream == null)
				throw(new ArgumentNullException("The specified input stream is a null reference"));
			
			XmlDocument doc = new XmlDocument();
			try
			{
				doc.Load(inStream);
			}
			catch(XmlException xe)
			{
				OnError(xe);
				return null;
			}
			catch(Exception e)
			{
				OnError(e);
				return null;
			}
			return ParseOwl(doc, graph, xmlbaseUri);
		}

		/// <summary>
		/// Parses the OWL from the given stream, using the given xml:base uri
		/// </summary>
		/// <param name="inStream">The Stream to use as the source of the XML data</param>
		/// <param name="xmlbaseUri">The xml:base Uri to use incase one is not found in the XML data</param>
		/// <returns>An object that implements the IOwlGraph interface</returns>
		public IOwlGraph ParseOwl(Stream inStream, string xmlbaseUri)
		{
			return ParseOwl(inStream,null,xmlbaseUri);
		}
	
		/// <summary>
		/// Parses the OWL from a stream into an existing Graph
		/// </summary>
		/// <param name="inStream">The input stream for data</param>
		/// <param name="graph">An object that implements the IOwlGraph interface that will be used as the destination graph</param>
		/// <returns>An object that implements the IOwlGraph interface</returns>
		public IOwlGraph ParseOwl(Stream inStream, IOwlGraph graph)
		{
			return ParseOwl(inStream,graph, null);
		}

		/// <summary>
		/// Parses the OWL from a stream
		/// </summary>
		/// <param name="inStream">The input stream for data</param>
		/// <returns>An object that implements the IOwlGraph interface</returns>
		public IOwlGraph ParseOwl(Stream inStream)
		{
			//looks for xml:base in Owl doc.
			//if not found, then defaults to http://unknown.org/
			return ParseOwl(inStream, null, null);
		}

		/// <summary>
		/// Parses the OWL at the given URI, into an existing graph
		/// </summary>
		/// <param name="uri">The Uri of the document to parse</param>
		/// <param name="graph">An object that implements the IOwlGraph interface that will be used as the destination graph</param>
		/// <returns>An object that implements the IOwlGraph interface</returns>
		public IOwlGraph ParseOwl(Uri uri, IOwlGraph graph)
		{
			if(uri == null)
				throw(new ArgumentNullException("The specified URI is a null reference"));
			//parses from a Uri.
			XmlDocument doc = new XmlDocument();
			try
			{
				doc.Load(uri.ToString());
			}
			catch(XmlException xe)
			{
				OnError(xe);
				return null;
			}
			catch(Exception e)
			{
				OnError(e);
				return null;
			}
			return ParseOwl(doc, graph);
		}

		/// <summary>
		/// Parses the OWL at the given URI
		/// </summary>
		/// <param name="uri">The Uri of the document to parse</param>
		/// <returns>An object that implements the IOwlGraph interface</returns>
		public IOwlGraph ParseOwl(Uri uri)
		{
			return ParseOwl(uri,null);
		}

		/// <summary>
		/// Parses the OWL at the given URI, into an existing graph
		/// </summary>
		/// <param name="uri">The Uri of the document to parse</param>
		/// <param name="graph">An object that implements the IOwlGraph interface that will be used as the destination graph</param>
		/// <returns>An object that implements the IOwlGraph interface</returns>
		public IOwlGraph ParseOwl(string uri, IOwlGraph graph)
		{
			Uri srcUri = null;
			try
			{
				srcUri = new Uri(uri);
			}
			catch(UriFormatException)
			{
				srcUri = new Uri(Path.GetFullPath(uri));
			}
			return ParseOwl(srcUri,graph);
		}

		/// <summary>
		/// Builds an OwlGraph from an RDF/XML serialization
		/// </summary>
		/// <param name="uri">The URI of the OWL document to parse</param>
		/// <returns>An object that implements the IOwlGraph interface</returns>
		public IOwlGraph ParseOwl(string uri)
		{
			return ParseOwl(uri,null);
		}

		/// <summary>
		/// Processes a node in the XML document. 
		/// </summary>
		/// <param name="node">The XmlNode to process.</param>
		/// <param name="parent">The parent of the current node.</param>
		/// <returns>Returns a reference to the new node created</returns>
		private Object ProcessNode(XmlNode node, Object parent)
		{
			//if the node is a comment or anything else then totally ignore
			if((node.NodeType == XmlNodeType.Comment) || (node.NodeType == XmlNodeType.None) || (node.NodeType == XmlNodeType.Whitespace) || (node.NodeType == XmlNodeType.SignificantWhitespace))
				return true;
			OwlNode rNode = null;
	
			if(node.NodeType == XmlNodeType.Element)
			{
				//get the xml:base attribute...
				XmlAttribute xmlBaseAttr = node.Attributes["xml:base"];
				if((xmlBaseAttr == null) && (parent != null))
				{
					//ok the child does not have an xml:base... and the parent is not null i.e. this node is not a child of the rdf:RDF root
					xmlBaseAttr = node.ParentNode.Attributes["xml:base"];
					if(xmlBaseAttr != null)
						node.Attributes.Append(xmlBaseAttr);
				}
			}

			OwlEdge parentOwlEdge = null;
			string langID = null;
			if(parent != null)
			{
				parentOwlEdge = (OwlEdge)parent;
				langID = parentOwlEdge.LangID;
			}

			if(node.NodeType == XmlNodeType.Element)
			{
				//first parse the Rdf attributes rdf:about, rdf:ID, rdf:nodeID
				string nodeID = ParseNodeRdfAttributes(node);
				rNode = ParseNodeSyntax(node,nodeID);
				OwlResource res = rNode as OwlResource;
				if(res != null) 
				{
					//OnMessage("Processing node: "+res.ID);
				}
				/*
				//try and get the xml:lang attribute or inherit the xml:lang attribute from the parent if applicable
				XmlNode langAttr = node.Attributes["xml:lang"];
				if(langAttr != null)
					rNode.LangID = langAttr.Value;
				else
					rNode.LangID = langID;
				*/
				//process the regular attributes of this node
				ParseNodeAttributes(node, rNode);
			}
			else if((node.NodeType == XmlNodeType.Text) || (node.NodeType == XmlNodeType.CDATA))
			{
				//its a literal
				//so get the lang ID and the datatype from the parent node
				string datatypeUri = GetDatatypeUri(node.ParentNode);
				rNode = (OwlLiteral)_owlGraph.AddLiteral(node.Value,langID,datatypeUri);
			}
		
			if(parentOwlEdge != null)
				parentOwlEdge.AttachChildNode(rNode);
	
			if(node.HasChildNodes)
			{
				int count = node.ChildNodes.Count;
				for(int i=0;i<count;i++)
					ProcessEdge(node.ChildNodes[i], rNode);
			}

			return rNode;
		}

		/// <summary>
		/// Parses the rdf:value, rdf:type attributes as well as any attributes not part of the OWL, RDF or XML namespace
		/// </summary>
		/// <param name="node">The XmlNode on which the attributes appear</param>
		/// <param name="rNode">The OwlNode to which the attributes must be applied</param>
		private void ParseNodeAttributes(XmlNode node, OwlNode rNode)
		{
			int count = node.Attributes.Count;
			for(int i=0;i<count;i++)
			{
				XmlAttribute attr = node.Attributes[i];
				if(!IsOwlRdfXmlProperty(attr) || (attr.Name=="rdf:type") || (attr.Name == "rdf:value"))
				{
					if(IsUnqualifiedRdfProperty(node.Attributes[i]))
						OnWarning("Unqualified use of rdf:"+node.Attributes[i].LocalName);

					//create a new edge
					string edgeUri = PrependXmlBase(attr.NamespaceURI+attr.LocalName, GetXmlBase(node));
					OwlEdge rEdge = new OwlEdge(edgeUri);
					OwlNode childNode = null;
					if(attr.Name == "rdf:type")
					{
						childNode = (OwlNode)_owlGraph.AddNode(PrependXmlBase(attr.Value,GetXmlBase(node)));
					}
					else
					{
						childNode = (OwlLiteral)_owlGraph.AddLiteral(attr.Value,rNode.LangID,GetDatatypeUri(node));
					}
					
					rNode.AttachChildEdge(rEdge);
					rEdge.AttachChildNode(childNode);
					//add the new edge to the graph
					_owlGraph.AddEdge(rEdge);
				}
			}
		}

		/// <summary>
		/// Parses the OWL attributes rdf:about and rdf:ID
		/// </summary>
		/// <param name="node">The XmlNode on which the attributes appear</param>
		/// <returns>A Uri string with the ID specified by the attributes. Null if none of the three attributes are found</returns>
		private string ParseNodeRdfAttributes(XmlNode node)
		{
			int attrFound = 0;
			XmlNode attr = node.Attributes["rdf:about"];
			string retVal = null;
			if(attr != null)
			{
				//found an rdf:about attribute
				retVal = QualifyResource(attr.Value, GetXmlBase(node));
				attrFound = 1;
			}
			attr = node.Attributes["rdf:ID"];
			if(attr != null)
			{
				//found an rdf:ID attribute
				//check that its a valid xml name
				if(!IsXmlName(attr.Value))
					OnError(attr.Value+" is not an XML Name");
				//now check if its already been declared
				if(_declID[attr.Value] != null)
					OnError("Redefinition of rdf:ID "+attr.Value);
				else
					_declID[attr.Value] = attr.Value;
				retVal = PrependXmlBase(attr.Value, GetXmlBase(node));
				attrFound += 2;
			}
			attr = node.Attributes["rdf:nodeID"];
			if(attr != null)
			{
				//found an rdf:nodeID attribute
				//check if its an xml name
				if(!IsXmlName(attr.Value))
					OnError(attr.Value+" is not an XML Name");
				retVal = GetBlankNodeUri(attr.Value);
				attrFound += 4;
			}
			switch(attrFound)
			{
				case 3:
					OnError("Cannot use rdf:about and rdf:ID together");
					break;
				case 5:
					OnError("Cannot use rdf:about and rdf:nodeID together");
					break;
				case 6:
					OnError("Cannot use rdf:ID and rdf:nodeID together");
					break;
				case 7:
					OnError("Cannot use rdf:about, rdf:ID and rdf:nodeID together");
					break;
			}
			return retVal;
		}

		/// <summary>
		/// Process the OWL Syntax on a node
		/// </summary>
		/// <param name="node">The XmlNode representing the OWL Node</param>
		/// <param name="nodeID">The ID to be assigned to the OWL node</param>
		/// <returns>The newly created OWL node or null if there are no OWL elements on this XmlNode</returns>
		private OwlNode ParseNodeSyntax(XmlNode node, string nodeID)
		{
			//first get the NameSpace URI, NameSpace prefix and the LocalName for the node
			String nameSpaceURI = node.NamespaceURI;
			String nameSpacePrefix = node.Prefix;
			String localName = node.LocalName;
			OwlNode rNode = null;
			
			if(nameSpaceURI==OwlNamespaceCollection.OwlNamespace)
			{
				if(localName=="Class")
				{
					//its an owl:Class
					rNode = AddClassToGraph(nodeID);
					return rNode;
				}
				else if(localName=="Restriction")
				{
					//its an owl:Restriction
					rNode = AddRestrictionToGraph(nodeID);
					return rNode;
				}
				else if(localName=="DataRange")
				{
					//its an owl:DataRange
					rNode = AddDataRangeToGraph(nodeID);
					return rNode;
				}
				else if(localName=="ObjectProperty")
				{
					//its an owl:ObjectProperty
					rNode = AddObjectPropertyToGraph(nodeID);
					return rNode;
				}
				else if(localName=="TransitiveProperty")
				{
					//its an owl:TransitiveProperty
					rNode = AddTransitivePropertyToGraph(nodeID);
					return rNode;
				}
				else if(localName=="SymmetricProperty")
				{
					//its an owl:SymmentricProperty
					rNode = AddSymmetricPropertyToGraph(nodeID);
					return rNode;
				}
				else if(localName=="InverseFunctionalProperty")
				{
					//its an owl:InverseFunctionalProperty
					rNode = AddInverseFunctionalPropertyToGraph(nodeID);
					return rNode;
				}
				else if(localName=="DatatypeProperty")
				{
					//its an owl:DatatypeProperty
					rNode = AddDatatypePropertyToGraph(nodeID);
					return rNode;
				}
				else if(localName=="FunctionalProperty")
				{
					//its an owl:FunctionalProperty
					rNode = AddFunctionalPropertyToGraph(nodeID);
					return rNode;
				}
				else if(localName=="Ontology") 
				{
					//its an owl:Ontology
					rNode = AddOntologyToGraph(nodeID);
					return rNode;
				}
				else if(localName=="OntologyProperty")
				{
					//its an owl:OntologyProperty
					rNode = AddOntologyPropertyToGraph(nodeID);
					return rNode;
				}
				else if(localName=="AnnotationProperty")
				{
					//it an owl:AnnotationProperty
					rNode = AddAnnotationPropertyToGraph(nodeID);
					return rNode;
				}
			}

			if(IsSyntacticElement(node))
				OnError("Cannot use " + node.Name + " as a node element name");

			string typeURI = nameSpaceURI + localName;
			rNode = AddIndividualToGraph(nodeID, typeURI);
			return rNode;

//			OnMessage("Strange node : " + nodeID);
//			OnMessage("   - " + nameSpaceURI);
//			OnMessage("   - " + nameSpacePrefix);
//			if(nameSpacePrefix.Length == 0) 
//			{
//				//then it must be a local instance of a class
//				string typeURI = nameSpaceURI + localName;
//				this.OnMessage("An instance (" + nodeID + ") has been encountered: " + typeURI);
//				rNode = AddIndividualToGraph(nodeID, typeURI);
//				return rNode;
//			}
//
//			
//			
//			//print a warning if its an unknown rdf or xml property
//			if(!IsNonSyntacticElement(node))
//				OnWarning("Unknown node element "+ node.Name);
//			
//			//IsOwlRdfXmlProperty(node);
//			OwlNode tempNode = (OwlNode)_owlGraph.AddNode(GetDummyNodeUri(null));
//			rNode = tempNode;
//
//			return rNode;
		}

		/// <summary>
		/// Adds an OWL Resource of type owl:Class to the graph</summary>
		/// <param name="nodeUri">The Uri of the resource.</param>
		/// <returns>Returns a reference to the newly added resource.</returns>
		/// <exception cref="UriFormatException">The specified nodeUri is not a well formed Uri.</exception>
		private OwlClass AddClassToGraph(string nodeUri)
		{
			//if the uri is null then create a blank node uri
			if(nodeUri == null)
				nodeUri = GetBlankNodeUri(null);
			OwlNode node = (OwlNode)_owlGraph[nodeUri];
			if((node != null) && (node is OwlClass))
				return (OwlClass)node;
			OwlNode typeNode = (OwlNode)_owlGraph.AddNode(OwlNamespaceCollection.OwlNamespace+"Class");
			if(node == null)
			{
				node = new OwlClass(nodeUri,typeNode);
				_owlGraph.AddEdge(((OwlClass)node).Type);
				_owlGraph.AddNode(node);
				return (OwlClass)node;
			}
			OwlClass newNode = new OwlClass(nodeUri,typeNode);
			_owlGraph.AddEdge(newNode.Type);
			MoveEdges(node, newNode);
			_owlGraph.Nodes.Remove(node);
			_owlGraph.AddNode(newNode);
			return newNode;
		}

		/// <summary>
		/// Adds an OWL Resource of type owl:Restriction to the graph</summary>
		/// <param name="nodeUri">The Uri of the resource.</param>
		/// <returns>Returns a reference to the newly added resource.</returns>
		/// <exception cref="UriFormatException">The specified nodeUri is not a well formed Uri.</exception>
		private OwlRestriction AddRestrictionToGraph(string nodeUri)
		{
			//if the uri is null then create a blank node uri
			if(nodeUri == null)
				nodeUri = GetBlankNodeUri(null);
			OwlNode node = (OwlNode)_owlGraph[nodeUri];
			if((node != null) && (node is OwlRestriction))
				return (OwlRestriction)node;
			OwlNode typeNode = (OwlNode)_owlGraph.AddNode(OwlNamespaceCollection.OwlNamespace+"Restriction");
			if(node == null)
			{
				node = new OwlRestriction(nodeUri,typeNode);
				_owlGraph.AddEdge(((OwlRestriction)node).Type);
				_owlGraph.AddNode(node);
				return (OwlRestriction)node;
			}
			OwlRestriction newNode = new OwlRestriction(nodeUri,typeNode);
			_owlGraph.AddEdge(newNode.Type);
			MoveEdges(node, newNode);
			_owlGraph.Nodes.Remove(node);
			_owlGraph.AddNode(newNode);
			return newNode;
		}

		private OwlNode AddDataRangeToGraph(string nodeUri)
		{
			//if the uri is null then create a blank node uri
			if(nodeUri == null)
				nodeUri = GetBlankNodeUri(null);

			OwlNode node = (OwlNode)_owlGraph[nodeUri];
			if((node != null) && (node is OwlDataRange))
				return (OwlDataRange)node;
			OwlNode typeNode = 
				(OwlNode)_owlGraph.AddNode(OwlNamespaceCollection.OwlNamespace+"DataRange");
			if(node == null)
			{
				node = new OwlDataRange(nodeUri,typeNode);
				_owlGraph.AddEdge(((OwlDataRange)node).Type);
				_owlGraph.AddNode(node);
				return (OwlDataRange)node;
			}
			OwlDataRange newNode = new OwlDataRange(nodeUri,typeNode);
			_owlGraph.AddEdge(newNode.Type);
			MoveEdges(node, newNode);
			_owlGraph.Nodes.Remove(node);
			_owlGraph.AddNode(newNode);
			return newNode;
		}

		/// <summary>
		/// Adds an OWL Resource of type owl:FunctionalProperty to the graph</summary>
		/// <param name="nodeUri">The Uri of the resource.</param>
		/// <returns>Returns a reference to the newly added resource.</returns>
		/// <exception cref="UriFormatException">The specified nodeUri is not a well formed Uri.</exception>
		private OwlProperty AddFunctionalPropertyToGraph(string nodeUri)
		{
			//if the uri is null then create a blank node uri
			if(nodeUri == null)
				nodeUri = GetBlankNodeUri(null);
			OwlNode node = (OwlNode)_owlGraph[nodeUri];
			if((node != null) && (node is OwlProperty))
				return (OwlProperty)node;
			OwlNode typeNode = (OwlNode)_owlGraph.AddNode(OwlNamespaceCollection.OwlNamespace+"FunctionalProperty");
			if(node == null)
			{
				node = new OwlProperty(nodeUri,typeNode);
				_owlGraph.AddEdge(((OwlProperty)node).Type);
				_owlGraph.AddNode(node);
				return (OwlProperty)node;
			}
			OwlProperty newNode = new OwlProperty(nodeUri,typeNode);
			_owlGraph.AddEdge(newNode.Type);
			MoveEdges(node, newNode);
			_owlGraph.Nodes.Remove(node);
			_owlGraph.AddNode(newNode);
			return newNode;
		}

		/// <summary>
		/// Adds an OWL Resource of type owl:ObjectProperty to the graph</summary>
		/// <param name="nodeUri">The Uri of the resource.</param>
		/// <returns>Returns a reference to the newly added resource.</returns>
		/// <exception cref="UriFormatException">The specified nodeUri is not a well formed Uri.</exception>
		private OwlObjectProperty AddObjectPropertyToGraph(string nodeUri)
		{
			//if the uri is null then create a blank node uri
			if(nodeUri == null)
				nodeUri = GetBlankNodeUri(null);
			OwlNode node = (OwlNode)_owlGraph[nodeUri];
			if((node != null) && (node is OwlObjectProperty))
				return (OwlObjectProperty)node;
			OwlNode typeNode = (OwlNode)_owlGraph.AddNode(OwlNamespaceCollection.OwlNamespace+"ObjectProperty");
			if(node == null)
			{
				node = new OwlObjectProperty(nodeUri,typeNode);
				_owlGraph.AddEdge(((OwlObjectProperty)node).Type);
				_owlGraph.AddNode(node);
				return (OwlObjectProperty)node;
			}
			OwlObjectProperty newNode = new OwlObjectProperty(nodeUri,typeNode);
			_owlGraph.AddEdge(newNode.Type);
			MoveEdges(node, newNode);
			_owlGraph.Nodes.Remove(node);
			_owlGraph.AddNode(newNode);
			return newNode;
		}

		/// <summary>
		/// Adds an OWL Resource of type owl:TransitiveProperty to the graph</summary>
		/// <param name="nodeUri">The Uri of the resource.</param>
		/// <returns>Returns a reference to the newly added resource.</returns>
		/// <exception cref="UriFormatException">The specified nodeUri is not a well formed Uri.</exception>
		private OwlNode AddTransitivePropertyToGraph(string nodeUri)
		{
			OwlNode node = AddObjectPropertyToGraph(nodeUri);
			OwlNode typeNode = (OwlNode)_owlGraph.AddNode(OwlNamespaceCollection.OwlNamespace+"TransitiveProperty");
			OwlEdge typeEdge = new OwlEdge(OwlNamespaceCollection.RdfNamespace+"type");
			_owlGraph.AddEdge(typeEdge);
			typeEdge.AttachChildNode(typeNode);
			node.AttachChildEdge(typeEdge);
			return node;
		}

		/// <summary>
		/// Adds an OWL Resource of type owl:SymmetricProperty to the graph</summary>
		/// <param name="nodeUri">The Uri of the resource.</param>
		/// <returns>Returns a reference to the newly added resource.</returns>
		/// <exception cref="UriFormatException">The specified nodeUri is not a well formed Uri.</exception>
		private OwlNode AddSymmetricPropertyToGraph(string nodeUri)
		{
			OwlNode node = AddObjectPropertyToGraph(nodeUri);
			OwlNode typeNode = (OwlNode)_owlGraph.AddNode(OwlNamespaceCollection.OwlNamespace+"SymmetricProperty");
			OwlEdge typeEdge = new OwlEdge(OwlNamespaceCollection.RdfNamespace+"type");
			_owlGraph.AddEdge(typeEdge);
			typeEdge.AttachChildNode(typeNode);
			node.AttachChildEdge(typeEdge);
			return node;
		}

		/// <summary>
		/// Adds an OWL Resource of type owl:InverseFunctionalProperty to the graph</summary>
		/// <param name="nodeUri">The Uri of the resource.</param>
		/// <returns>Returns a reference to the newly added resource.</returns>
		/// <exception cref="UriFormatException">The specified nodeUri is not a well formed Uri.</exception>
		private OwlNode AddInverseFunctionalPropertyToGraph(string nodeUri)
		{
			OwlNode node = AddObjectPropertyToGraph(nodeUri);
			OwlNode typeNode = (OwlNode)_owlGraph.AddNode(OwlNamespaceCollection.OwlNamespace+"InverseFunctionalProperty");
			OwlEdge typeEdge = new OwlEdge(OwlNamespaceCollection.RdfNamespace+"type");
			_owlGraph.AddEdge(typeEdge);
			typeEdge.AttachChildNode(typeNode);
			node.AttachChildEdge(typeEdge);
			return node;
		}

		/// <summary>
		/// Adds an OWL Resource of type owl:DatatypeProperty to the graph</summary>
		/// <param name="nodeUri">The Uri of the resource.</param>
		/// <returns>Returns a reference to the newly added resource.</returns>
		/// <exception cref="UriFormatException">The specified nodeUri is not a well formed Uri.</exception>
		private OwlDatatypeProperty AddDatatypePropertyToGraph(string nodeUri)
		{
			//if the uri is null then create a blank node uri
			if(nodeUri == null)
				nodeUri = GetBlankNodeUri(null);
			OwlNode node = (OwlNode)_owlGraph[nodeUri];
			if((node != null) && (node is OwlDatatypeProperty))
				return (OwlDatatypeProperty)node;
			OwlNode typeNode = (OwlNode)_owlGraph.AddNode(OwlNamespaceCollection.OwlNamespace+"DatatypeProperty");
			if(node == null)
			{
				node = new OwlDatatypeProperty(nodeUri,typeNode);
				_owlGraph.AddEdge(((OwlDatatypeProperty)node).Type);
				_owlGraph.AddNode(node);
				return (OwlDatatypeProperty)node;
			}
			OwlDatatypeProperty newNode = new OwlDatatypeProperty(nodeUri,typeNode);
			_owlGraph.AddEdge(newNode.Type);
			MoveEdges(node, newNode);
			_owlGraph.Nodes.Remove(node);
			_owlGraph.AddNode(newNode);
			return newNode;
		}

		/// <summary>
		/// Adds an OWL Resource of type owl:Ontology to the graph</summary>
		/// <param name="nodeUri">The Uri of the resource.</param>
		/// <returns>Returns a reference to the newly added resource.</returns>
		/// <exception cref="UriFormatException">The specified nodeUri is not a well formed Uri.</exception>
		private OwlOntology AddOntologyToGraph(string nodeUri)
		{
			//if the uri is null then create a blank node uri
			if(nodeUri == null)
				nodeUri = GetBlankNodeUri(null);
			OwlNode node = (OwlNode)_owlGraph[nodeUri];
			if((node != null) && (node is OwlOntology))
				return (OwlOntology)node;
			OwlNode typeNode = (OwlNode)_owlGraph.AddNode(OwlNamespaceCollection.OwlNamespace+"Ontology");
			if(node == null)
			{
				node = new OwlOntology(nodeUri,typeNode);
				_owlGraph.AddEdge(((OwlOntology)node).Type);
				_owlGraph.AddNode(node);
				return (OwlOntology)node;
			}
			OwlOntology newNode = new OwlOntology(nodeUri,typeNode);
			_owlGraph.AddEdge(newNode.Type);
			MoveEdges(node, newNode);
			_owlGraph.Nodes.Remove(node);
			_owlGraph.AddNode(newNode);
			return newNode;
		}

		/// <summary>
		/// Adds an OWL Resource of type owl:OntologyProperty to the graph</summary>
		/// <param name="nodeUri">The Uri of the resource.</param>
		/// <returns>Returns a reference to the newly added resource.</returns>
		/// <exception cref="UriFormatException">The specified nodeUri is not a well formed Uri.</exception>
		private OwlOntologyProperty AddOntologyPropertyToGraph(string nodeUri)
		{
			//if the uri is null then create a blank node uri
			if(nodeUri == null)
				nodeUri = GetBlankNodeUri(null);
			OwlNode node = (OwlNode)_owlGraph[nodeUri];
			if((node != null) && (node is OwlOntologyProperty))
				return (OwlOntologyProperty)node;
			OwlNode typeNode = (OwlNode)_owlGraph.AddNode(OwlNamespaceCollection.OwlNamespace+"OntologyProperty");
			if(node == null)
			{
				node = new OwlOntologyProperty(nodeUri,typeNode);
				_owlGraph.AddEdge(((OwlOntologyProperty)node).Type);
				_owlGraph.AddNode(node);
				return (OwlOntologyProperty)node;
			}
			OwlOntologyProperty newNode = new OwlOntologyProperty(nodeUri,typeNode);
			_owlGraph.AddEdge(newNode.Type);
			MoveEdges(node, newNode);
			_owlGraph.Nodes.Remove(node);
			_owlGraph.AddNode(newNode);
			return newNode;
		}

		/// <summary>
		/// Adds an OWL Resource of type owl:AnnotationProperty to the graph</summary>
		/// <param name="nodeUri">The Uri of the resource.</param>
		/// <returns>Returns a reference to the newly added resource.</returns>
		/// <exception cref="UriFormatException">The specified nodeUri is not a well formed Uri.</exception>
		private OwlAnnotationProperty AddAnnotationPropertyToGraph(string nodeUri)
		{
			//if the uri is null then create a blank node uri
			if(nodeUri == null)
				nodeUri = GetBlankNodeUri(null);
			OwlNode node = (OwlNode)_owlGraph[nodeUri];
			if((node != null) && (node is OwlAnnotationProperty))
				return (OwlAnnotationProperty)node;
			OwlNode typeNode = (OwlNode)_owlGraph.AddNode(OwlNamespaceCollection.OwlNamespace+"AnnotationProperty");
			if(node == null)
			{
				node = new OwlAnnotationProperty(nodeUri,typeNode);
				_owlGraph.AddEdge(((OwlAnnotationProperty)node).Type);
				_owlGraph.AddNode(node);
				return (OwlAnnotationProperty)node;
			}
			OwlAnnotationProperty newNode = new OwlAnnotationProperty(nodeUri,typeNode);
			_owlGraph.AddEdge(newNode.Type);
			MoveEdges(node, newNode);
			_owlGraph.Nodes.Remove(node);
			_owlGraph.AddNode(newNode);
			return newNode;
		}

		/// <summary>
		/// Adds an OWL Individual to the graph</summary>
		/// <param name="nodeUri">The Uri of the resource.</param>
		/// <param name="typeUri">The Uri of the type.</param>
		/// <returns>Returns a reference to the newly added resource.</returns>
		/// <exception cref="UriFormatException">The specified nodeUri is not a well formed Uri.</exception>
		private OwlIndividual AddIndividualToGraph(string nodeUri, string typeUri)
		{
			//if the uri is null then create a blank node uri
			if(nodeUri == null)
				nodeUri = GetBlankNodeUri(null);
			OwlNode node = (OwlNode)_owlGraph[nodeUri];
			if((node != null) && (node is OwlIndividual))
				return (OwlIndividual)node;
			OwlNode typeNode = (OwlNode)_owlGraph[typeUri];
			if(typeNode == null)
				typeNode = (OwlNode)_owlGraph.AddNode(typeUri);

			if(node == null)
			{
				node = new OwlIndividual(nodeUri,typeNode);
				_owlGraph.AddEdge(((OwlIndividual)node).Type);
				_owlGraph.AddNode(node);
				return (OwlIndividual)node;
			}
			OwlIndividual newNode = new OwlIndividual(nodeUri,typeNode);
			_owlGraph.AddEdge(newNode.Type);
			MoveEdges(node, newNode);
			_owlGraph.Nodes.Remove(node);
			_owlGraph.AddNode(newNode);
			return newNode;
		}

		/// <summary>
		/// Parses the OWL Attributes rdf:resource, rdf:nodeID, rdf:parseType, and xml:lang as they appear on an edge
		/// </summary>
		/// <param name="node">The XmlNode on which the attributes appear</param>
		/// <param name="rEdge">The OwlEdge to which the attributes must be applied</param>
		/// <returns>True if the children of the specified XML Node should be parsed</returns>
		private bool ParseEdgeRdfAttributes(XmlNode node, OwlEdge rEdge)
		{
			int attrFound = 0;
			bool parseChildren = true;
			XmlNode attr = node.Attributes["rdf:resource"];
			if(attr != null)
			{
				ProcessRdfResource(node, rEdge, attr.Value);
				attrFound = 1;
				parseChildren = false;
			}
			attr = node.Attributes["rdf:nodeID"];
			if(attr != null)
			{
				ProcessRdfNodeID(rEdge, attr.Value);
				attrFound += 2;
				parseChildren = false;
			}
			attr = node.Attributes["rdf:parseType"];
			if(attr != null)
			{
				ProcessRdfParseType(node,rEdge,attr.Value);
				attrFound += 4;
				parseChildren = false;
			}
			attr = node.Attributes["xml:lang"];
			if(attr != null)
			{
				rEdge.LangID = attr.Value;
			}
			switch(attrFound)
			{
				case 3:
					OnError("Cannot use rdf:resource and rdf:nodeID together");
					break;
				case 5:
					OnError("Cannot use rdf:resource and rdf:parseType together");
					break;
				case 6:
					OnError("Cannot use rdf:nodeID and rdf:parseType together");
					break;
				case 7:
					OnError("Cannot use rdf:resource, rdf:nodeID and rdf:parseType together");
					break;
			}
			return parseChildren;
		}

		/// <summary>
		/// Process the rdf:value and any attributes not in the rdf or xml namespace
		/// </summary>
		/// <param name="node">The XmlNode that attributes appear on</param>
		/// <param name="rEdge">The Owl edge that the attributes must be applied to.</param>
		private void ParseEdgeAttributes(XmlNode node, OwlEdge rEdge)
		{
			//go through all the attributes
			int count = node.Attributes.Count;
			for(int i=0;i<count;i++)
			{
				XmlAttribute attr = node.Attributes[i];
				
				if((!IsSyntacticElement(attr))  || (attr.Prefix == "rdf" && attr.LocalName == "value"))
				{
					if((attr.NamespaceURI == null) || (attr.NamespaceURI.Length == 0))
					{
						OnError("Unqualified attribute: "+attr.LocalName);
						continue;
					}
					if((attr.Name == "rdf:value")&& (rEdge.ChildNode is OwlLiteral))
					{
						OnError("Cannot use rdf:value ("+attr.Value+") as property for a literal ("+((OwlLiteral)rEdge.ChildNode).Value+").");
						continue;
					}
					//if the childnode of the edge is a literal then it cant have any arcs going out from it
					if((rEdge.ChildNode != null) && (rEdge.ChildNode is OwlLiteral) && (attr.Name != "rdf:datatype"))
					{
						OnError("Cannot have property "+attr.Name+" for an Owl Literal "+rEdge.ChildNode.ID);
						continue;
					}
					
					string edgeID = attr.NamespaceURI+attr.LocalName;
					string literalValue = attr.Value;
					string langID = rEdge.LangID;
					string datatypeUri = GetDatatypeUri(node);

					//if this edge does not have a child node then create a blank node and add it
					if(rEdge.ChildNode == null)
						rEdge.AttachChildNode((OwlNode)_owlGraph.AddNode(GetBlankNodeUri(null)));

					//make an edge from the child of rEdge
					OwlEdge childEdge = new OwlEdge();
					childEdge.ID = edgeID;
					OwlLiteral childLiteral = (OwlLiteral)_owlGraph.AddLiteral(literalValue,langID,datatypeUri);
		
					rEdge.ChildNode.AttachChildEdge(childEdge);
					//attach it to the edge
					childEdge.AttachChildNode(childLiteral);
					//add the edge to the graph
					_owlGraph.AddEdge(childEdge);
				}
			}
		}

		/// <summary>
		/// Create a node from the rdf:resource attribute and attach it to a given edge as a child
		/// </summary>
		/// <param name="node">The XmlNode that contains the rdf:resource attribute.</param>
		/// <param name="rEdge">The edge to which the new childnode must be added.</param>
		/// <param name="resourceUri">The URI specified by the rdf:resource attribute.</param>
		/// <remarks>If the specified Uri is null or empty a new blank node URI is created. 
		/// If it is a relative URI then it is converted to an absolute URI by prefixing it with the value given by xml:base</remarks>
		private void ProcessRdfResource(XmlNode node, OwlEdge rEdge, string resourceUri)
		{
			string nodeID = QualifyResource(resourceUri,GetXmlBase(node));
			OwlNode rNode = (OwlNode)_owlGraph.AddNode(nodeID);
			rEdge.AttachChildNode(rNode);
		}

		/// <summary>
		/// Process the rdf:nodeID attribute found on the OWL edge
		/// </summary>
		/// <param name="rEdge">The edge to which the rdf:nodeID attribute must be applied</param>
		/// <param name="baseNodeID">The ID specified by the rdf:nodeID attribute</param>
		private void ProcessRdfNodeID(OwlEdge rEdge, string baseNodeID)
		{
			if(!IsXmlName(baseNodeID))
				OnError(baseNodeID+" is not an XML name");
			OwlNode rNode = (OwlNode)_owlGraph.AddNode(GetBlankNodeUri(baseNodeID));
			rEdge.AttachChildNode(rNode);
		}

		/// <summary>
		/// Processes the rdf:parseType attribute
		/// </summary>
		/// <param name="node">The XML Node on which the attribute appears</param>
		/// <param name="rEdge">The edge to which the attribute must be applied</param>
		/// <param name="parseType">The parse type as specified by the value of the attribute</param>
		private void ProcessRdfParseType(XmlNode node, OwlEdge rEdge, string parseType)
		{
			this.OnMessage("Parsing a parseType");
			if(parseType == "Resource")
			{
				//its an rdf:parseType="Resource" so we must now parse its children
				//the children of this xmlNode are now EDGES in the RDF graph that 
				//extend out from the dest node of this edge i.e. the rdfNode created above
							
				//create a new node that will be the child of this edge
				//create a new node ID
				OwlNode rNode = (OwlNode)_owlGraph.AddNode(GetBlankNodeUri(null));
				//attach this node to the edge
				rEdge.AttachChildNode(rNode);
				this.OnMessage("Parsing a parseType: Resource");
				if(node.HasChildNodes)
				{
					int count = node.ChildNodes.Count;
					for(int i=0;i<count;i++) 
					{
						this.OnMessage("Going to parse the first child: " + node.ChildNodes[i].OuterXml.ToString());
						ProcessEdge(node.ChildNodes[i], rNode);
					}
				}
			}
			else if(parseType == "Literal")
			{
				//its an rdf:parseType="Literal" so all the xml below this node 
				//will be the content of the dest node i.e. rdfNode
				//create a new node that will be the child of this edge
				//create a new node ID
				string literalValue = node.InnerXml;
				string datatypeUri = GetDatatypeUri(node);
				if(datatypeUri == null)
					datatypeUri = "http://www.w3.org/1999/02/22-rdf-syntax-ns#XMLLiteral";
				OwlLiteral literal = (OwlLiteral)_owlGraph.AddLiteral(literalValue,rEdge.LangID,datatypeUri);
				//attach this node to the edge
				rEdge.AttachChildNode(literal);
			}
			else if(parseType == "Collection")
			{
				//its a Collection so make a cons list
				//get the children of this node
				OwlNode rNode = BuildCollection(node);
				//connect the collection to the edge
				rEdge.AttachChildNode(rNode);
			}
			else
			{
				OnError("Unknown parseType "+parseType);
			}
		}

		/// <summary>
		/// Builds an OWL Collection.
		/// </summary>
		/// <param name="propertyNode">The XML Node containing the children that will form the members of this list</param>
		/// <returns>The head of the collection. This is the first member of the list or a nil node if the list is empty</returns>
		private OwlCollection BuildCollection(XmlNode propertyNode)
		{
//			OwlNode rNode;
//			OwlNode collHead = null;
//			OwlEdge restEdge = null;
//			OwlEdge firstEdge;
			int count = propertyNode.ChildNodes.Count;

			OwlCollection collection = new OwlCollection(GetBlankNodeUri(null));
			
			for(int i=0;i<count;i++)
			{
//				//build the blank node that is a member of this list
//				rNode = (OwlNode)_owlGraph.AddNode(GetBlankNodeUri(null));
//				//if this is the first element of the cons list then set this as the head of the collection so we may return it later
//				if(i == 0)
//					collHead = rNode;
//
//				//add an outgoing edge with name rdf:type from rdfNode to rdf:List
//				OwlEdge typeEdge = new OwlEdge(OwlNamespaceCollection.RdfNamespace+"type");
//				_owlGraph.AddEdge(typeEdge);
//				OwlNode typeNode = (OwlNode)_owlGraph.AddNode(OwlNamespaceCollection.RdfNamespace+"List");
//				rNode.AttachChildEdge(typeEdge);
//				typeEdge.AttachChildNode(typeNode);
//										
//				//create a new first edge
//				firstEdge = new OwlEdge(OwlNamespaceCollection.RdfNamespace+"first");
//				rNode.AttachChildEdge(firstEdge);
//				//add the first edge to the graph
//				_owlGraph.AddEdge(firstEdge);
//
//				OwlNode firstNode =	(OwlNode)ProcessNode(propertyNode.ChildNodes[i],firstEdge);

				OwlNode newNode = (OwlNode)ProcessNode(propertyNode.ChildNodes[i],null);
				collection.Add(newNode);

//				//OwlNode firstNode =	tempNode;
//				firstEdge.AttachChildNode(firstNode);
//		
//				if(restEdge != null)
//					rNode.AttachParentEdge(restEdge);
//
//				//make the new rest Edge
//				restEdge = new OwlEdge(OwlNamespaceCollection.RdfNamespace+"rest");
//				restEdge.AttachParentNode(rNode);
//				//add the new RestEdge to the graph
//				_owlGraph.AddEdge(restEdge);
			}
//			//ok we have exited the loop or bypassed it because there are no children
//			
//			rNode = (OwlNode)_owlGraph.AddNode(OwlNamespaceCollection.RdfNamespace+"nil");
//			if(restEdge != null)
//				rNode.AttachParentEdge(restEdge);
//					
//			if(collHead == null)
//				collHead = rNode;
//
//			return collHead;
			return collection;
		}

		/// <summary>
		/// Parses the OWL syntax on an edge.
		/// </summary>
		/// <param name="node">The XML Node representing the OWL edge</param>
		/// <param name="rEdge">The edge</param>
		/// <param name="parentNode">The parent OWL node</param>
		/// <returns>True if the edge was given an ID and attached to the parent node</returns>
		private bool ParseEdgeSyntax(XmlNode node, OwlEdge rEdge, OwlNode parentNode)
		{
			//first get the NameSpace URI, NameSpace prefix and the LocalName for the node
			String nameSpaceURI = node.NamespaceURI;
			String nameSpacePrefix = node.Prefix;
			String localName = node.LocalName;
			if(IsNonSyntacticElement(node)) 
			{
				rEdge.ID = nameSpaceURI + localName;
				
				if(rEdge.ID == (OwlNamespaceCollection.RdfNamespace + "type"))
				{
					OwlNode typeNode;
					if(rEdge.ChildNode.ID == (OwlNamespaceCollection.OwlNamespace + "DatatypeProperty")) 
					{
						typeNode = (OwlNode)_owlGraph.AddNode(OwlNamespaceCollection.OwlNamespace+"DatatypeProperty");
						OwlDatatypeProperty newNode = new OwlDatatypeProperty(parentNode.ID,typeNode);
						_owlGraph.AddEdge(newNode.Type);
						MoveEdges(parentNode, newNode);
						_owlGraph.Nodes.Remove(parentNode);
						_owlGraph.AddNode(newNode);
						rEdge.AttachParentNode(newNode);
						return true;
					}
					if(rEdge.ChildNode.ID == (OwlNamespaceCollection.OwlNamespace + "ObjectProperty")) 
					{
						typeNode = (OwlNode)_owlGraph.AddNode(OwlNamespaceCollection.OwlNamespace+"ObjectProperty");
						OwlObjectProperty newNode = new OwlObjectProperty(parentNode.ID,typeNode);
						_owlGraph.AddEdge(newNode.Type);
						MoveEdges(parentNode, newNode);
						_owlGraph.Nodes.Remove(parentNode);
						_owlGraph.AddNode(newNode);
						rEdge.AttachParentNode(newNode);
						return true;
					}
					
				}

				rEdge.AttachParentNode(parentNode);
				return true;
			}

			if(IsSyntacticElement(node)) 
			{
				OnError("Cannot use " + node.Name + " as a property element");
				return false;
			}

			// It is an instance of a property
			rEdge.ID = nameSpaceURI + localName;
			rEdge.AttachParentNode(parentNode);
			return true;
			
//			OnWarning("Unknown property element "+ node.Name);
//			return false;
		}

		/// <summary>
		/// Gets the rdf:datatype Uri from the specified XML element
		/// </summary>
		/// <param name="node">The XmlNode from which to extract the rdf:datatype attribute</param>
		/// <returns>A string representing the datatype Uri. A null reference is returned if the rdf:datatype attribute is not found.</returns>
		/// <remarks>This method looks for the rdf:datatype attribute on the specified XML element and returns the value.</remarks>
		private string GetDatatypeUri(XmlNode node)
		{
			XmlNode datatypeAttr = node.Attributes["rdf:datatype"];
			if(datatypeAttr != null)
				return PrependXmlBase(datatypeAttr.Value, GetXmlBase(node));
			return null;
		}

		/// <summary>
		/// Processes an edge in the XML document. 
		/// </summary>
		/// <param name="node">The XmlNode to process.</param>
		/// <param name="parent">The parent of the current node.</param>
		/// <returns>Returns a reference to the new edge created</returns>
		private Object ProcessEdge(XmlNode node,  Object parent)
		{
			//if the node is a comment or anything else then totally ignore
			if((node.NodeType == XmlNodeType.Comment) || (node.NodeType == XmlNodeType.None) || (node.NodeType == XmlNodeType.Whitespace) || (node.NodeType == XmlNodeType.SignificantWhitespace))
				return true;
			OwlEdge rEdge = null;
			
			if(node.NodeType == XmlNodeType.Element)
			{
				//get the xml:base attribute...
				XmlAttribute xmlBaseAttr = node.Attributes["xml:base"];
				if((xmlBaseAttr == null) && (parent != null))
				{
					//ok the child does not have an xml:base... and the parent is not null i.e. this node is not a child of the rdf:RDF root
					xmlBaseAttr = node.ParentNode.Attributes["xml:base"];
					if(xmlBaseAttr != null)
						node.Attributes.Append(xmlBaseAttr);
				}
			}

			rEdge = new OwlEdge();

			OwlNode parentOwlNode = (OwlNode)parent;
			//process the xml:lang attribute if applicable
			//rEdge.LangID = parentOwlNode.LangID;
				

				
			if(ParseEdgeRdfAttributes(node,rEdge)) //if the process attributes method returns true then process the children of this node
			{
				if(node.HasChildNodes)
				{
					int count = node.ChildNodes.Count;
					for(int i=0;i<count;i++)
						ProcessNode(node.ChildNodes[i], rEdge);
				}
			}

			if(!ParseEdgeSyntax(node,rEdge, parentOwlNode))
			{
				rEdge.ID = node.NamespaceURI+node.LocalName;
				rEdge.AttachParentNode(parentOwlNode);
				
			}

			//coming back up the tree
			ParseEdgeAttributes(node,rEdge);

			//if the edge is dangling then put an empty Literal on it.
			if(rEdge.ChildNode == null)
				rEdge.AttachChildNode(_owlGraph.AddLiteral("",null,null));

			//add the edge to the Graph
			_owlGraph.AddEdge(rEdge); 

			return rEdge;
		}

		/// <summary>
		/// Moves all the edges associated with the source node to the destination node
		/// </summary>
		/// <param name="srcNode">The node from which the edges are to be moved.</param>
		/// <param name="destNode">The node to which the edges are to be moved</param>
		/// <remarks>This method moves all the edges from the src node to the dest node. 
		/// The src node is not removed from the graph. </remarks>
		private void MoveEdges(OwlNode srcNode, OwlNode destNode)
		{
			try 
			{
				foreach(IOwlEdge parentEdge in srcNode.ParentEdges)
				{
					parentEdge.AttachChildNode(destNode);
				}
				foreach(IOwlEdge childEdge in srcNode.ChildEdges)
				{
					childEdge.AttachParentNode(destNode);
				}
			}
			catch 
			{
				// sometimes an error is thrown saying that the collection is being modified
				// during the transfer. If this happens, just redo the transfer
				MoveEdges(srcNode, destNode);
			}
		}

		/// <summary>
		/// Converts the value given by rdf:about into a fully qualified Uri
		/// </summary>
		/// <param name="val">The value specified by the rdf:about attribute</param>
		/// <param name="xmlBaseUriString">An string specifying an xml:base Uri to use. This parameter can be null.</param>
		/// <returns>A well formed Uri string</returns>
		/// <remarks>This method should only be used to convert rdf:about and rdf:resource values into fully qualified URIs. 
		/// If the xmlBaseUriString is null or an empty string then the global value for xml:base will be used.</remarks>
		private string QualifyResource(string val, string xmlBaseUriString)
		{
			if((xmlBaseUriString == null) || (xmlBaseUriString.Length == 0))
				xmlBaseUriString = _owlGraph.NameSpaces["xml:base"];
			
			//if val is blank or null, val = xml:base
			if((val == null) || (val.Length == 0))
				return xmlBaseUriString;

			Uri xmlBaseUri = new Uri(xmlBaseUriString);

			// if val starts with // or val starts with \\
			// val = scheme + ":"+ val;
			if(val.StartsWith("\\\\") || val.StartsWith("//"))
				return xmlBaseUri.Scheme+":"+val;
			if(IsValidUri(val))
				return val;

			// if val starts with #
			// val = xml:base+val
			if(val.StartsWith("#"))
				return xmlBaseUriString+val;

			// if val starts with /
			// val = scheme + "://" + authority + val;
			if(val.StartsWith("/") || val.StartsWith("\\"))
				return xmlBaseUri.Scheme+"://"+xmlBaseUri.Authority+val;

			string folderPath = GetFolderPath(xmlBaseUri.AbsolutePath);
			// if val starts with ../
			// val = scheme+"://" + authority + modpath {modpath = GetAbsolutePath(folderpath,val)}
			if(val.StartsWith("../") || val.StartsWith("..\\"))
			{
				string absPath = GetAbsolutePath(folderPath, val);
				return xmlBaseUri.Scheme+"://"+xmlBaseUri.Authority+absPath;
			}
			// if val doesnt start with anything 
			// val = scheme+"://" + authority + folderPath + val
			return xmlBaseUri.Scheme+"://"+xmlBaseUri.Authority+folderPath+val;
		}


		private string GetFolderPath(string pathStr)
		{
			
			if(!(pathStr.EndsWith("/") || pathStr.EndsWith("\\")))
			{
				int index = pathStr.LastIndexOfAny(new Char[]{'\\','/'});
				pathStr = pathStr.Substring(0,index+1);
			}
			return pathStr;
		}

		/// <summary>
		/// Returns an absolute path by combining the initial path and a relative path
		/// </summary>
		/// <param name="initPath">A string representing the initial path</param>
		/// <param name="relPath">A string representing the relative path</param>
		/// <returns>A string representing the absolute path formed by the combination of the initial and relative paths.</returns>
		/// <remarks>The init path should be terminated at both ends by / or \ characters. This method concatenates the initial 
		/// and relative paths and returns an absolute path with proper handling of ../ prefixes on the relative path</remarks>
		private string GetAbsolutePath(string initPath, string relPath)
		{
			Regex regex = new Regex(@"[/\\](\w)*[/\\](\.\.)[/\\]",RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
			string path = initPath+relPath;
			string oldPath = "";
			while(oldPath!= path)
			{
				oldPath = path;
				path = regex.Replace(oldPath,"/");
			}
			return path;
		}

		/// <summary>
		/// Makes a URI string from the specified ID by prepending the uri specified by xml:base to it
		/// </summary>
		/// <param name="id">The ID to convert to a URI string</param>
		/// <param name="xmlBaseUri">A string containing an xml:base Uri to use rather than the global xml:base Uri.</param>
		/// <remarks>This method checks the given xmlBaseUri string for the value of the xml:base URI to prepend to the ID.
		/// If it is null or an empty string then the global xml:base is used.</remarks>
		/// <returns>A string containing a well formed URI.</returns>
		private string PrependXmlBase(string id, string xmlBaseUri)
		{
			if(IsValidUri(id))
				return id;
			if(id == null)
				id = "";
			if((xmlBaseUri == null) || (xmlBaseUri.Length == 0))
				xmlBaseUri = _owlGraph.NameSpaces["xml:base"];
			if(id[0] == '#')
				id = xmlBaseUri+id;
			else
				id = xmlBaseUri+"#"+id;
			return id;
		}

		/// <summary>
		/// Gets the value of the xml:base attribute from the XmlNode if one exists
		/// </summary>
		/// <param name="node">An Xml Node</param>
		/// <returns>A string containing the xml:base uri. Returns null if the xml:base attribute is not found</returns>
		private string GetXmlBase(XmlNode node)
		{
			XmlNode attr = node.Attributes["xml:base"];
			if(attr == null)
				return null;
			string xmlBaseUri =	attr.Value;
			if(!IsValidUri(xmlBaseUri))
				return null;

			//ignore and discard everything after the first # character
			int pos = xmlBaseUri.IndexOf('#');
			if(pos != -1)
			{
				xmlBaseUri = xmlBaseUri.Remove(pos,xmlBaseUri.Length-pos);
			}
			return xmlBaseUri;
		}

		/// <summary>
		/// Determines whether the XML Node is a property that is part of the RDF or XML syntax
		/// </summary>
		/// <param name="prop">An XmlNode</param>
		/// <returns>True if the property is part of the OWL, RDF or XML syntax or if the property is reserved for use by xml</returns>
		/// <remarks>This method returns true is the property localname or prefix begins with xml (regardless of whether xml is in uppercase, lowercase or
		/// any combination thereof. </remarks>
		private bool IsOwlRdfXmlProperty(XmlNode prop)
		{
			if(_rdfXmlProperties[prop.Name] != null)
				return true;
			if((prop.Prefix.ToLower().StartsWith("xml")) || (prop.LocalName.ToLower().StartsWith("xml")))
				return true;
			if(((prop.Prefix == "rdf") || (prop.Prefix == "xml") || (prop.Prefix == "owl")) && (!IsNonSyntacticElement(prop) && (!IsSyntacticElement(prop))))
				OnWarning("Unknown OWL, RDF or XML property: "+prop.Prefix+":"+prop.LocalName);
			return false;
		}

		/// <summary>
		/// Determines whether the XML Node is a syntactic OWL, RDF, or RDFS element on an edge
		/// </summary>
		/// <param name="prop">An XmlNode</param>
		/// <returns>True if the element is a syntactic OWL, RDF, or RDFS element.</returns>
		private bool IsSyntacticElement(XmlNode prop)
		{
			if(_syntacticElements[prop.Name] != null)
				return true;
			return false;
		}

		/// <summary>
		/// Determines whether the XML Node is a non-syntactic OWL, RDF, or RDFS element on an edge
		/// </summary>
		/// <param name="prop">An XmlNode</param>
		/// <returns>True if the element is a non-syntactic OWL, RDF, or RDFS element.</returns>
		private bool IsNonSyntacticElement(XmlNode prop)
		{
			if(_nonSyntacticElements[prop.Name] != null)
				return true;
			return false;
		}

		/// <summary>
		/// Determines whether the specified name is an XML name
		/// </summary>
		/// <param name="name">A Name</param>
		/// <returns>True if the name is an XML name</returns>
		private bool IsXmlName(string name)
		{
			try
			{
				XmlConvert.VerifyNCName(name);
				return true;
			}
			catch(XmlException)
			{
				return false;
			}
		}

		/// <summary>
		/// Determines whether the XML Node is an unqualified RDF property
		/// </summary>
		/// <param name="prop"></param>
		/// <returns></returns>
		private bool IsUnqualifiedRdfProperty(XmlNode prop)
		{
			string prefix = prop.Prefix;
			string localName = prop.LocalName;

			if(prefix.Length == 0)
			{
				if(_rdfXmlProperties["rdf:"+localName] != null)
					return true;
			}
			return false;
		}

		/// <summary>
		/// Gets a URI string for a new blank node
		/// </summary>
		/// <param name="baseID">The base ID from which the Uri must be created.</param>
		/// <returns>A string containing a well formed URI</returns>
		private string GetBlankNodeUri(string baseID)
		{
			if((baseID == null) || (baseID.Length == 0))
				return "blankID:"+_newID++;
			else
				return "blankID:"+baseID;
		}

		/// <summary>
		/// Gets a URI string for a new blank node
		/// </summary>
		/// <param name="baseID">The base ID from which the Uri must be created.</param>
		/// <returns>A string containing a well formed URI</returns>
		private string GetDummyNodeUri(string baseID)
		{
			if((baseID == null) || (baseID.Length == 0))
				return "dummyID:"+_dummyID++;
			else
				return "dummyID:"+baseID;
		}

		#endregion
	}
}
