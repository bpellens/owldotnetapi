/*****************************************************************************
 * OwlXmlGenerator.cs
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
using System.Xml;
using System.IO;
using System.Collections.Generic;

namespace OwlDotNetApi
{
    /// <summary>
    /// Summary description for OwlXmlGenerator.
    /// </summary>
    public class OwlXmlGenerator : OwlGenerator, IOwlXmlGenerator
    {
        #region Variables
        /// <summary>
        /// The OWL Xml Document object
        /// </summary>
        protected XmlDocument _owlDocument;

        /// <summary>
        /// The base URI of the ontology the be generated
        /// </summary>
        private string _baseUri;

        /// <summary>
        /// This is the reversed collection of namespaces with as key the URI
        /// of the namespace and as value the prefix;
        /// </summary>
        private OwlNamespaceCollection _namespaces;

        private List<OwlNode> _visited;

        #endregion

        #region Creators

        /// <summary>
        /// Initializes an instance of the generator.
        /// </summary>
        public OwlXmlGenerator()
        {
            _owlDocument = null;
            _warnings = new ArrayList();
            _errors = new ArrayList();
            _messages = new ArrayList();

            _baseUri = "";
            _namespaces = new OwlNamespaceCollection();
            StopOnErrors = false;
            StopOnWarnings = false;
        }

        #endregion

        #region Manipulators
        /// <summary>
        /// Generate the OWL graph into a file represented by the uri
        /// </summary>
        /// <param name="graph">The graph which needs to be generated</param>
        /// <param name="filename">The name of the file to which the graph needs to be generated</param>
        public void GenerateOwl(IOwlGraph graph, string filename)
        {
            if (filename == null)
                throw (new ArgumentNullException("The specified filename is a null reference"));

            _owlDocument = new XmlDocument();

            GenerateOwl(graph, _owlDocument);
            try
            {
                _owlDocument.Save(filename);
            }
            catch (XmlException xe)
            {
                OnError(xe);
                return;
            }
            catch (Exception e)
            {
                OnError(e);
                return;
            }
        }

        /// <summary>
        /// Generate the OWL graph into a file represented by the object of type Uri
        /// </summary>
        /// <param name="graph">The graph which needs to be generated</param>
        /// <param name="uri">The Uri object representing the file to which the graph needs to be generated</param>
        public void GenerateOwl(IOwlGraph graph, Uri uri)
        {
            if (uri == null)
                throw (new ArgumentNullException("The specified URI is a null reference"));

            _owlDocument = new XmlDocument();

            GenerateOwl(graph, _owlDocument);

            // Save the file to the correct uri
            // This is not supported at the moment
        }

        /// <summary>
        /// Generates the OWL graph into an XmlDocument object
        /// </summary>
        /// <param name="graph">The graph which needs to be generated</param>
        /// <param name="doc">The XmlDocument object used as a destination for the graph</param>
        public void GenerateOwl(IOwlGraph graph, XmlDocument doc)
        {
            if (doc == null)
                throw (new ArgumentNullException("The specified XmlDocument object is a null reference"));

            Warnings.Clear();
            Errors.Clear();

            _visited.Clear();

            XmlElement root = _owlDocument.CreateElement("rdf:RDF", OwlNamespaceCollection.RdfNamespace);
            _owlDocument.AppendChild(root);

            //Added by HM
            // Create an XML declaration. 
            XmlDeclaration xmldecl = _owlDocument.CreateXmlDeclaration("1.0", null, null);
            xmldecl.Encoding = "UTF-8";
            xmldecl.Standalone = "yes";

            // Add the new node to the document.			
            _owlDocument.InsertBefore(xmldecl, root);

            //End of HM addition

            _baseUri = graph.NameSpaces["xml:base"];

            IDictionaryEnumerator nsEnumerator = (IDictionaryEnumerator)graph.NameSpaces.GetEnumerator();
            while (nsEnumerator.MoveNext())
            {
                // Write all the namespaces to the document
                XmlAttribute nsAttribute = _owlDocument.CreateAttribute((nsEnumerator.Key).ToString());
                nsAttribute.Value = (nsEnumerator.Value).ToString();
                root.Attributes.Append(nsAttribute);
                // Also insert the reversed namespaces into the a local variable
                _namespaces[nsEnumerator.Value.ToString()] = nsEnumerator.Key.ToString();
            }

            IDictionaryEnumerator nEnumerator = (IDictionaryEnumerator)graph.Nodes.GetEnumerator();
            while (nEnumerator.MoveNext())
            {
                OwlNode node = (OwlNode)graph.Nodes[(nEnumerator.Key).ToString()];
                if (!node.IsAnonymous())
                    node.Accept(this, root);
            }

            XmlComment comment = _owlDocument.CreateComment("This file has been generated by the OwlDotNetApi.");
            _owlDocument.AppendChild(comment);
            doc = _owlDocument;
        }

        /// <summary>
        /// The general implementation of a regular OwlNode. A node is only of type OwlNode in some special cases.
        /// </summary>
        /// <param name="node">The actual node to visit</param>
        /// <param name="parent">The parent object of the node</param>
        public override void Visit(OwlNode node, Object parent)
        {
            XmlElement parentElement = parent as XmlElement;
            if (parentElement != null)
            {
                // Here we will only visit the blank node, because these are 
                // the only ones we need. The others are the standard nodes
                // representing datatypes or owl, rdf, rdfs predefined nodes.
                if (node.IsAnonymous())
                {
                    XmlComment comment = _owlDocument.CreateComment("Visiting a regular node: " + node.GetType());
                    parentElement.AppendChild(comment);

                    _visited.Add(node);
                }
            }
        }

        /// <summary>
        /// Implementation of an OwlEdge.
        /// </summary>
        /// <param name="edge">The actual edge to be visited</param>
        /// <param name="parent">The parent object (node) of the edge</param>
        public override void Visit(OwlEdge edge, Object parent)
        {
            XmlElement parentElement = parent as XmlElement;
            if (parentElement != null)
            {
                Uri uri = new Uri(edge.ID);
                // Retrieve the local name from the uri
                string localName = uri.Fragment.Substring(1, uri.Fragment.Length - 1);
                // Get the path, up to the local name, from the uri
                string path = uri.GetLeftPart(UriPartial.Path) + "#";
                string prefix;
                // Check for the namespace of the edge in order to get the correct prefix
                if (path == OwlNamespaceCollection.OwlNamespace)
                    prefix = OwlNamespaceCollection.OwlNamespacePrefix;
                else if (path == OwlNamespaceCollection.RdfNamespace)
                    prefix = OwlNamespaceCollection.RdfNamespacePrefix;
                else if (path == OwlNamespaceCollection.RdfSchemaNamespace)
                    prefix = OwlNamespaceCollection.RdfSchemaNamespacePrefix;
                else
                {
                    // If the namespace of the edge is something else, then look for it
                    prefix = "";
                    if (path != _baseUri)
                    {
                        // Search for the prefix of this individual
                        int pos = _namespaces[path].IndexOf(':');
                        if (pos != -1)
                        {
                            prefix = _namespaces[path].Substring(++pos);
                        }
                    }
                }

                string qualifiedName = (prefix.Length != 0) ? (prefix + ":" + localName) : localName;
                // At this point we will not generate the rdf:type edges.
                XmlElement edgeElement = _owlDocument.CreateElement(qualifiedName, path);

                // Check to see if the child node is an anonymous one or not. If it is, then further
                // process that node, otherwise just reference it by name.
                OwlNode node = (OwlNode)edge.ChildNode;
                if (node.IsAnonymous() || node.IsLiteral())
                    node.Accept(this, edgeElement);
                else
                {
                    // Add the name as one of its attributes
                    XmlAttribute nameAttribute = _owlDocument.CreateAttribute("rdf", "resource", OwlNamespaceCollection.RdfNamespace);
                    nameAttribute.Value = GetNodeReference(node);
                    edgeElement.Attributes.Append(nameAttribute);
                }
                parentElement.AppendChild(edgeElement);
            }
        }

        /// <summary>
        /// Implementation of the visit function to generate some output, used in the visitor pattern
        /// </summary>
        /// <param name="node">The actual node which needs to be generated</param>
        /// <param name="parent">The parent object of the node</param>
        public override void Visit(OwlClass node, Object parent)
        {
            XmlElement parentElement = parent as XmlElement;
            if (parentElement != null)
            {
                XmlElement classElement = _owlDocument.CreateElement("owl:Class", OwlNamespaceCollection.OwlNamespace);
                // If the node is not a blank one
                if (!node.IsAnonymous())
                {
                    // Add the name as one of its attributes
                    AddNameAttribute(classElement, node);
                }

                // Generate all the edges going out of this node
                if (!_visited.Contains(node))
                    VisitEdges(node, classElement);

                // Attach the node eventually to its parent
                parentElement.AppendChild(classElement);
                _visited.Add(node);
            }
        }

        /// <summary>
        /// Implementation of the visit function to generate some output, used in the visitor pattern
        /// </summary>
        /// <param name="node">The actual node which needs to be generated</param>
        /// <param name="parent">The parent object of the node</param>
        public override void Visit(OwlLiteral node, Object parent)
        {
            XmlElement parentElement = parent as XmlElement;
            _messages.Add("Writing " + node.ID);
            if (parentElement != null)
            {
                // Set the datatype property on its parent
                XmlAttribute nameAttribute = _owlDocument.CreateAttribute("rdf", "datatype", OwlNamespaceCollection.RdfNamespace);
                nameAttribute.Value = node.Datatype;
                parentElement.Attributes.Append(nameAttribute);

                // Add the value of the literal
                parentElement.InnerXml = node.Value;
                _visited.Add(node);
                return;
            }
        }

        /// <summary>
        /// Implementation of the visit function to generate some output, used in the visitor pattern
        /// </summary>
        /// <param name="node">The actual node which needs to be generated</param>
        /// <param name="parent">The parent object of the node</param>
        public override void Visit(OwlAnnotationProperty node, Object parent)
        {
            XmlElement parentElement = parent as XmlElement;
            if (parentElement != null)
            {
                XmlElement propertyElement = _owlDocument.CreateElement("owl:AnnotationProperty", OwlNamespaceCollection.OwlNamespace);
                // Add the name attribute to the node
                AddNameAttribute(propertyElement, node);

                // Generate all the edges going out of this node
                if (!_visited.Contains(node))
                    VisitEdges(node, propertyElement);

                // Attach the node eventually to its parent
                parentElement.AppendChild(propertyElement);
                _visited.Add(node);
            }
        }

        /// <summary>
        /// Implementation of the visit function to generate some output, used in the visitor pattern
        /// </summary>
        /// <param name="node">The actual node which needs to be generated</param>
        /// <param name="parent">The parent object of the node</param>
        public override void Visit(OwlDatatype node, Object parent)
        {
            // Todo
        }

        /// <summary>
        /// Implementation of the visit function to generate some output, used in the visitor pattern
        /// </summary>
        /// <param name="node">The actual node which needs to be generated</param>
        /// <param name="parent">The parent object of the node</param>
        public override void Visit(OwlDataRange node, Object parent)
        {
            // Todo
        }

        /// <summary>
        /// Implementation of the visit function to generate some output, used in the visitor pattern
        /// </summary>
        /// <param name="node">The actual node which needs to be generated</param>
        /// <param name="parent">The parent object of the node</param>
        public override void Visit(OwlDatatypeProperty node, Object parent)
        {
            XmlElement parentElement = parent as XmlElement;
            if (parentElement != null)
            {
                XmlElement propertyElement = _owlDocument.CreateElement("owl:DatatypeProperty", OwlNamespaceCollection.OwlNamespace);
                // Add the name attribute to the node
                AddNameAttribute(propertyElement, node);

                // Generate all the edges going out of this node
                if (!_visited.Contains(node))
                {
                    VisitEdges(node, propertyElement);
                }

                // Attach the node eventually to its parent
                parentElement.AppendChild(propertyElement);
                _visited.Add(node);
            }
        }

        /// <summary>
        /// Implementation of the visit function to generate some output, used in the visitor pattern
        /// </summary>
        /// <param name="node">The actual node which needs to be generated</param>
        /// <param name="parent">The parent object of the node</param>
        public override void Visit(OwlIndividual node, Object parent)
        {
            XmlElement parentElement = parent as XmlElement;
            if (parentElement != null)
            {
                //string qualifiedName = prefix + ":" + localName;
                OwlNode typeNode = (OwlNode)node.Type.ChildNode;
                Uri uri = new Uri(typeNode.ID);
                // Retrieve the local name from the uri
                string localName = uri.Fragment.Substring(1, uri.Fragment.Length - 1);
                // Get the path, up to the local name, from the uri
                string path = uri.GetLeftPart(UriPartial.Path) + "#";
                string prefix = "";
                if (path != _baseUri)
                {
                    // Search for the prefix of this individual
                    int pos = _namespaces[path].IndexOf(':');
                    if (pos != -1)
                    {
                        prefix = _namespaces[path].Substring(++pos);
                    }
                }
                string qualifiedName = (prefix.Length != 0) ? (prefix + ":" + localName) : localName;

                XmlElement individualElement = _owlDocument.CreateElement(qualifiedName, path);

                // Add the name attribute to the node
                AddNameAttribute(individualElement, node);

                // Generate all the edges going out of this node
                if (!_visited.Contains(node))
                    VisitEdges(node, individualElement);

                // Attach the node eventually to its parent
                parentElement.AppendChild(individualElement);
                _visited.Add(node);
            }
        }

        /// <summary>
        /// Implementation of the visit function to generate some output, used in the visitor pattern
        /// </summary>
        /// <param name="node">The actual node which needs to be generated</param>
        /// <param name="parent">The parent object of the node</param>
        public override void Visit(OwlObjectProperty node, Object parent)
        {
            XmlElement parentElement = parent as XmlElement;
            if (parentElement != null)
            {
                XmlElement propertyElement = _owlDocument.CreateElement("owl:ObjectProperty", OwlNamespaceCollection.OwlNamespace);
                // Add the name attribute to the node
                AddNameAttribute(propertyElement, node);

                // Generate all the edges going out of this node
                if (!_visited.Contains(node))
                {
                    // Visit all the other edges
                    VisitEdges(node, propertyElement);
                }

                // Attach the node eventually to its parent
                parentElement.AppendChild(propertyElement);
                _visited.Add(node);
            }
        }

        /// <summary>
        /// Implementation of the visit function to generate some output, used in the visitor pattern
        /// </summary>
        /// <param name="node">The actual node which needs to be generated</param>
        /// <param name="parent">The parent object of the node</param>
        public override void Visit(OwlOntology node, Object parent)
        {
            XmlElement parentElement = parent as XmlElement;
            if (parentElement != null)
            {
                XmlElement ontologyElement = _owlDocument.CreateElement("owl:Ontology", OwlNamespaceCollection.OwlNamespace);
                // Add the name attribute to the node
                XmlAttribute nameAttribute = _owlDocument.CreateAttribute("rdf", "about", OwlNamespaceCollection.RdfNamespace);
                nameAttribute.Value = GetNodeID(node);
                ontologyElement.Attributes.Append(nameAttribute);

                // Generate all the edges going out of this node
                if (!_visited.Contains(node))
                    VisitEdges(node, ontologyElement);

                // Attach the node eventually to its parent
                parentElement.AppendChild(ontologyElement);
                _visited.Add(node);
            }
        }

        /// <summary>
        /// Implementation of the visit function to generate some output, used in the visitor pattern
        /// </summary>
        /// <param name="node">The actual node which needs to be generated</param>
        /// <param name="parent">The parent object of the node</param>
        public override void Visit(OwlOntologyProperty node, Object parent)
        {
            XmlElement parentElement = parent as XmlElement;
            if (parentElement != null)
            {
                XmlElement propertyElement = _owlDocument.CreateElement("owl:OntologyProperty", OwlNamespaceCollection.OwlNamespace);
                // Add the name attribute to the node
                AddNameAttribute(propertyElement, node);

                // Generate all the edges going out of this node
                if (!_visited.Contains(node))
                    VisitEdges(node, propertyElement);

                // Attach the node eventually to its parent
                parentElement.AppendChild(propertyElement);
                _visited.Add(node);
            }
        }

        /// <summary>
        /// Implementation of the visit function to generate some output, used in the visitor pattern
        /// </summary>
        /// <param name="node">The actual node which needs to be generated</param>
        /// <param name="parent">The parent object of the node</param>
        public override void Visit(OwlProperty node, Object parent)
        {
            XmlElement parentElement = parent as XmlElement;
            if (parentElement != null)
            {
                XmlComment comment = _owlDocument.CreateComment("Visiting a property: " + node.ID);
                parentElement.AppendChild(comment);
                // Nothing is done when a regular property has been reached
            }
        }

        /// <summary>
        /// Implementation of the visit function to generate some output, used in the visitor pattern
        /// </summary>
        /// <param name="node">The actual node which needs to be generated</param>
        /// <param name="parent">The parent object of the node</param>
        public override void Visit(OwlRestriction node, Object parent)
        {
            XmlElement parentElement = parent as XmlElement;
            if (parentElement != null)
            {
                XmlElement restrictionElement = _owlDocument.CreateElement("owl:Restriction", OwlNamespaceCollection.OwlNamespace);

                // If the node is not a blank one
                if (!node.IsAnonymous())
                {
                    // Add the name as one of its attributes
                    AddNameAttribute(restrictionElement, node);
                }

                // Generate all the edges going out of this node
                if (!_visited.Contains(node))
                    VisitEdges(node, restrictionElement);

                // Attach the node eventually to its parent
                parentElement.AppendChild(restrictionElement);
                _visited.Add(node);
            }
        }

        /// <summary>
        /// Implementation of the visit function to generate some output, used in the visitor pattern
        /// </summary>
        /// <param name="node">The actual node which needs to be generated</param>
        /// <param name="parent">The parent object of the node</param>
        public override void Visit(OwlResource node, Object parent)
        {
            XmlElement parentElement = parent as XmlElement;
            if (parentElement != null)
            {
                XmlComment comment = _owlDocument.CreateComment("Visiting a resource: " + node.ID);
                parentElement.AppendChild(comment);
                // Nothing is done when a resource has been reached
            }
        }

        /// <summary>
        /// Implementation of the visit function to generate some output, used in the visitor pattern
        /// </summary>
        /// <param name="node">The actual node which needs to be generated</param>
        /// <param name="parent">The parent object of the node</param>
        public override void Visit(OwlCollection node, Object parent)
        {
            XmlElement parentElement = parent as XmlElement;
            if (parentElement != null)
            {
                // Set the datatype property on its parent
                XmlAttribute nameAttribute = _owlDocument.CreateAttribute("rdf", "parseType", OwlNamespaceCollection.RdfNamespace);
                nameAttribute.Value = "Collection";
                parentElement.Attributes.Append(nameAttribute);

                IEnumerator e = node.GetEnumerator();
                while (e.MoveNext())
                {
                    OwlNode n = (OwlNode)e.Current;
                    n.Accept(this, parent);
                }
            }
        }

        /// <summary>
        /// Visit all the outgoing edges of the node and add them to the parent
        /// </summary>
        /// <param name="node">The actual node of which we need to visit the edges</param>
        /// <param name="parent">The xml representative of the node</param>
        public void VisitEdges(OwlNode node, Object parent)
        {
            OwlEdgeCollection edges = (OwlEdgeCollection)node.ChildEdges;
            foreach (OwlEdge e in edges)
            {
                e.Accept(this, parent);
            }
        }

        private void AddNameAttribute(XmlElement element, OwlNode node)
        {
            XmlAttribute nameAttribute;
            if (_visited.Contains(node))
            {
                nameAttribute = _owlDocument.CreateAttribute("rdf", "about", OwlNamespaceCollection.RdfNamespace);
                nameAttribute.Value = GetNodeReference(node);
            }
            else
            {
                nameAttribute = _owlDocument.CreateAttribute("rdf", "ID", OwlNamespaceCollection.RdfNamespace);
                nameAttribute.Value = GetNodeID(node);
            }
            element.Attributes.Append(nameAttribute);
        }

        /// <summary>
        /// This method extracts the localName of the node
        /// </summary>
        /// <param name="node">The node from which we want to retrieve the local ID</param>
        /// <returns>The local name of the node if it has one, or the full uri otherwise</returns>
        private string GetNodeID(OwlNode node)
        {
            Uri uri = new Uri(node.ID);
            string name = uri.ToString();
            if (name.StartsWith(_baseUri))
            {
                name = uri.Fragment;
                if (name.Length > 0)
                    name = uri.Fragment.Substring(1, uri.Fragment.Length - 1);
            }
            return name;
        }

        /// <summary>
        /// This method extracts the reference of the node.
        /// </summary>
        /// <param name="node">The node from which we want to retrieve the reference</param>
        /// <returns>The reference of the node if it has a short reference, or the full uri otherwise</returns>
        private string GetNodeReference(OwlNode node)
        {
            Uri uri = new Uri(node.ID);
            string name = uri.ToString();
            if (name.StartsWith(_baseUri))
            {
                name = uri.Fragment;
            }
            return name;
        }

        #endregion
    }
}
