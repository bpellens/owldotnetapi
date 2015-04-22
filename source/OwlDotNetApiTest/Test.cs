using System;
using System.IO;
using System.Collections;
using OwlDotNetApi;

namespace OwlDotNetApiTest
{
	/// <summary>
	/// Summary description for Test.
	/// </summary>
	class Test
	{

		public Test() 
		{

		}

		public void test1(string file) 
		{
			IOwlParser parser = new OwlXmlParser();

			IOwlGraph graph = parser.ParseOwl(file);
			ArrayList errors = ((OwlParser)parser).Errors;
			ArrayList warnings = ((OwlParser)parser).Warnings;
			ArrayList messages = ((OwlParser)parser).Messages;

			//FileStream info = new FileStream("c:/info.txt", FileMode.OpenOrCreate);
			//StreamWriter sw = new StreamWriter(info);
			//sw.AutoFlush = true;

			//IOwlGenerator generator = new OwlXmlGenerator();
			//generator.GenerateOwl(graph, @"c:\example1.owl");

			//info = new FileStream("C:/generated.txt", FileMode.OpenOrCreate);
			//sw = new StreamWriter(info);
			//sw.AutoFlush = true;

//			foreach(string msg in messages)
//			{
//				sw.WriteLine(msg);
//			}
//
//			Console.WriteLine("Graph parsed successfully with {0} errors and {1} warnings\n\n",errors.Count,warnings.Count);
//
//			foreach(string err in errors)
//			{
//				Console.WriteLine("Error: "+err);
//			}
//
//			foreach(string war in warnings)
//			{
//				Console.WriteLine("Warning: "+war);
//			}
//
//			Console.WriteLine("The graph contains {0} node(s) and {1} edge(s).", graph.Nodes.Count, graph.Edges.Count);

			string baseuri = graph.NameSpaces["xmlns"];
			Console.WriteLine("The basuri is " + baseuri);

			OwlClass owlAnnotation = new OwlClass(baseuri + "Annotation"); //Create a parent (Annotation) node to relate to
			OwlGraph newGraph = new OwlGraph();
			newGraph.NameSpaces["xmlns:" + OwlNamespaceCollection.OwlNamespacePrefix] = OwlNamespaceCollection.OwlNamespace;
			newGraph.NameSpaces["xmlns:" + OwlNamespaceCollection.RdfSchemaNamespacePrefix] = OwlNamespaceCollection.RdfSchemaNamespace;
			newGraph.NameSpaces["xmlns:daml"] = "http://www.daml.org/2001/03/daml+oil#";
			newGraph.NameSpaces["xmlns:domain"] = "http://www.owl-ontologies.com/domain.owl#";
			newGraph.NameSpaces["xmlns:dc"] = "http://purl.org/dc/elements/1.1/";
			newGraph.NameSpaces["xmlns"] = "http://www.owl-ontologies.com/test.owl#";
			newGraph.NameSpaces["xml:base"] = "http://www.owl-ontologies.com/test.owl";
			newGraph.NameSpaces["xmlns:meta"] = baseuri;
		string newUri = "http://www.owl-ontologies.com/test.owl#";
			OwlIndividual  pp = new OwlIndividual(newUri + "Test", owlAnnotation); //Create the annotation
			newGraph.Nodes.Add(pp); 

			IOwlGenerator generator = new OwlXmlGenerator();
			generator.GenerateOwl(newGraph, @"c:\example2.owl");
		}

		public void test2() 
		{
			OwlGraph ontology = new OwlGraph();
			ontology.NameSpaces["xmlns:" + OwlNamespaceCollection.OwlNamespacePrefix] = OwlNamespaceCollection.OwlNamespace;
			ontology.NameSpaces["xmlns:" + OwlNamespaceCollection.RdfSchemaNamespacePrefix] = OwlNamespaceCollection.RdfSchemaNamespace;
			ontology.NameSpaces["xmlns:daml"] = "http://www.daml.org/2001/03/daml+oil#";
			ontology.NameSpaces["xmlns:domain"] = "http://www.owl-ontologies.com/domain.owl#";
			ontology.NameSpaces["xmlns:dc"] = "http://purl.org/dc/elements/1.1/";
			ontology.NameSpaces["xmlns"] = "http://www.owl-ontologies.com/test.owl#";
			ontology.NameSpaces["xml:base"] = "http://www.owl-ontologies.com/test.owl";

			string baseUri = "http://www.owl-ontologies.com/test.owl#";
			string altUri = "http://www.owl-ontologies.com/domain.owl#";

			OwlOntology o = new OwlOntology(baseUri + "testOntology");
			ontology.Nodes.Add(o);

			OwlClass a = new OwlClass(altUri + "ClassA");
			//ontology.Nodes.Add(a);

			OwlIndividual inst = new OwlIndividual(baseUri + "Car", a);
			ontology.Nodes.Add(inst);

			//OwlClass b = new OwlClass(baseUri + "ClassB");
			//ontology.Nodes.Add(b);

//			 OwlClass concept = new OwlClass(altUri+node.Type); //Create a concept node
//           
//            OwlIndividual instance = new OwlIndividual(baseUri + node.ID, concept);    // create an instance node as individual of concept node                      
//            _description.Nodes.Add(instance);


			//OwlEdge relation = new OwlEdge(OwlNamespaceCollection.RdfSchemaNamespace + "subClassOf");
			//relation.AttachParentNode(a);
			//relation.AttachChildNode(b);
			//ontology.Edges.Add(relation);

			IOwlGenerator generator = new OwlXmlGenerator();
			generator.GenerateOwl(ontology, @"c:\example2.owl");
		}

		public void test3(string file) 
		{
			IOwlParser parser = new OwlXmlParser();
			IOwlGraph graph = parser.ParseOwl(file);

			Console.WriteLine("The nodes of the graph are:");
			IDictionaryEnumerator nEnumerator = (IDictionaryEnumerator)graph.Nodes.GetEnumerator();
			while(nEnumerator.MoveNext()) 
			{
				OwlNode node = (OwlNode)graph.Nodes[(nEnumerator.Key).ToString()];
				if(!node.IsAnonymous()) Console.WriteLine(node.ID);
			}

			Console.WriteLine(Environment.NewLine);

			Console.WriteLine("Retrieving some specific data:");
			IOwlNode hotelNode = (IOwlNode)graph.Nodes["http://www.owl-ontologies.com/travel.owl#Hotel"];
			
			Console.WriteLine(Environment.NewLine);
			Console.WriteLine("The edges are: ");
			OwlEdgeCollection edges = (OwlEdgeCollection)hotelNode.ChildEdges;
			foreach(OwlEdge e in edges) 
			{
				Console.WriteLine(e.ID);
			}

			Console.WriteLine("The subClassOf edges are:");
			IOwlEdgeList subclassEdges = (IOwlEdgeList)hotelNode.ChildEdges["http://www.w3.org/2000/01/rdf-schema#subClassOf"];
			foreach(OwlEdge s in subclassEdges) 
			{
				Console.WriteLine(s.ChildNode.ID);
			}
		}

		public void test4(string file) 
		{
			IOwlParser parser = new OwlXmlParser();
			IOwlGraph graph = parser.ParseOwl(file);

			Console.WriteLine("Retrieving some specific data:");
		
			// Here we will retrieve the enumerator in order to get all the nodes from the file
			IDictionaryEnumerator nEnumerator = (IDictionaryEnumerator)graph.Nodes.GetEnumerator();
			while(nEnumerator.MoveNext()) 
			{
				// Get the node from the graph
				OwlNode node = (OwlNode)graph.Nodes[(nEnumerator.Key).ToString()];
				// We will cast the node to a OwlClass because we are looking for classes
				OwlClass clsNode = node as OwlClass;
				// If clsNode is different from null, then we are dealing with an OwlClass -> OK
				// If the clsNode is not anonymous, means that we have a class with a proper name -> OK
				if((clsNode != null) && (!clsNode.IsAnonymous())) 
				{
					// So, now we have a good owl-class, we will look for any subClassOf relations (edges)
					IOwlEdgeList subclassEdges = (IOwlEdgeList)node.ChildEdges["http://www.w3.org/2000/01/rdf-schema#subClassOf"];
					if(subclassEdges != null) 
					{
						// We will list all the edges and check if the target of the edge is the class we want to
						// have as the superclass
						foreach(OwlEdge s in subclassEdges) 
						{
							if(s.ChildNode.ID == "http://www.owl-ontologies.com/travel.owl#Accommodation")
								Console.WriteLine(node.ID);
						}
					}
				}
			}
		}

		public void test5(string file) 
		{
			IOwlParser parser = new OwlXmlParser();
			IOwlGraph graph = parser.ParseOwl(file);

			string baseUri = "http://www.owl-ontologies.com/travel.owl#";
			OwlClass hotelNode = (OwlClass)graph.Nodes["http://www.owl-ontologies.com/travel.owl#LuxuryHotel"];

			OwlIndividual newHotel = new OwlIndividual(baseUri + "PellensPalace", hotelNode);
			graph.Nodes.Add(newHotel);

			IOwlGenerator generator = new OwlXmlGenerator();
			generator.GenerateOwl(graph, @"c:\travelnew.owl");
		}

		public void test6(string file) 
		{
			IOwlParser parser = new OwlXmlParser();
			IOwlGraph graph = parser.ParseOwl(file);

			IOwlGenerator generator = new OwlXmlGenerator();
			generator.GenerateOwl(graph, @"c:\travelnew.owl");
		}

		public void test7(string file) 
		{
			// First of all, we will create the parser object and parse the 
			// file that we want
			IOwlParser parser = new OwlXmlParser();
			IOwlGraph graph = parser.ParseOwl(file);

			// Lookup the instance of course for which we want to search for
			// the prerequisites, in our ontology, this is CourseA
			OwlIndividual instanceCourseA = (OwlIndividual)graph.Nodes["http://www.owl-ontologies.com/test.owl#CourseA"];

			// With this loop, we will go through all the nodes in the graph
			IDictionaryEnumerator nEnumerator = (IDictionaryEnumerator)graph.Nodes.GetEnumerator();
			while(nEnumerator.MoveNext()) 
			{
				OwlNode node = (OwlNode)graph.Nodes[(nEnumerator.Key).ToString()];
				// If the node we are encountering is an individual (so an 
				// instance, then we will continue
				OwlIndividual i = node as OwlIndividual;
				if(i != null) 
				{
					Console.WriteLine("Course: " + i.ID);
					// For this node, we will now look for all the edges that
					// we want, in our case the isPrerequisite edges
					IOwlEdgeList prerequisiteEdges = (IOwlEdgeList)node.ChildEdges["http://www.owl-ontologies.com/test.owl#isPrerequisite"];
					if(prerequisiteEdges != null) 
					{
						// Finally, a loop over all the edges and if we
						// encounter one which has an equal id to our
						// instance, then print the OK.
						foreach(OwlEdge s in prerequisiteEdges) 
						{
							if(s.ChildNode.ID == instanceCourseA.ID) 
							{
								Console.WriteLine("-- Ok");	
							}
						}
					}
				}	
			}
		}

		public void test8() 
		{
			OwlGraph ontology = new OwlGraph();
			ontology.NameSpaces["xmlns:" + OwlNamespaceCollection.OwlNamespacePrefix] = OwlNamespaceCollection.OwlNamespace;
			ontology.NameSpaces["xmlns:" + OwlNamespaceCollection.RdfSchemaNamespacePrefix] = OwlNamespaceCollection.RdfSchemaNamespace;
			ontology.NameSpaces["xmlns:daml"] = "http://www.daml.org/2001/03/daml+oil#";
			ontology.NameSpaces["xmlns:dc"] = "http://purl.org/dc/elements/1.1/";
			ontology.NameSpaces["xmlns"] = "http://www.owl-ontologies.com/test.owl#";
			ontology.NameSpaces["xml:base"] = "http://www.owl-ontologies.com/test.owl";

			string baseUri = "http://www.owl-ontologies.com/test.owl#";
			
			OwlClass testCls = new OwlClass(baseUri + "TestClass");
			ontology.Nodes.Add(testCls);
			OwlRestriction testRestriction = new OwlRestriction("blankID:1");
			ontology.Nodes.Add(testRestriction);
			OwlObjectProperty testProp = new OwlObjectProperty(baseUri + "testProp");
			ontology.Nodes.Add(testProp);

			OwlEdge subClassOfRelation = new OwlEdge(OwlNamespaceCollection.RdfSchemaNamespace + "subClassOf");
			subClassOfRelation.AttachParentNode(testCls);
			subClassOfRelation.AttachChildNode(testRestriction);
			ontology.Edges.Add(subClassOfRelation);

			OwlEdge onPropertyRelation = new OwlEdge(OwlNamespaceCollection.OwlNamespace + "onProperty");
			onPropertyRelation.AttachParentNode(testRestriction);
			onPropertyRelation.AttachChildNode(testProp);
			ontology.Edges.Add(onPropertyRelation);

			OwlLiteral val = new OwlLiteral("1", null, "http://www.w3.org/2001/XMLSchema#int");

			OwlEdge cardinalityRelation = new OwlEdge(OwlNamespaceCollection.OwlNamespace + "cardinality");
			cardinalityRelation.AttachParentNode(testRestriction);
			cardinalityRelation.AttachChildNode(val);
			ontology.Edges.Add(cardinalityRelation);

			IOwlGenerator generator = new OwlXmlGenerator();
			generator.GenerateOwl(ontology, @"c:\test.owl");
		}

        public void test9(string file)
        {
            IOwlParser parser = new OwlXmlParser();
            IOwlGraph graph = parser.ParseOwl(file);

            Console.WriteLine("Retrieving some specific data:");

            // Here we will retrieve the enumerator in order to get all the nodes from the file
            IDictionaryEnumerator nEnumerator = (IDictionaryEnumerator)graph.Nodes.GetEnumerator();
            while (nEnumerator.MoveNext())
            {
                // Get the node from the graph
                OwlNode node = (OwlNode)graph.Nodes[(nEnumerator.Key).ToString()];
                // We will cast the node to a OwlIndividual because we are looking for individuals
                OwlIndividual indNode = node as OwlIndividual;
                // If indNode is different from null, then we are dealing with an OwlIndividual -> OK
                // If the indNode is not anonymous, means that we have an individual with a proper name -> OK
                if ((indNode != null) && (!indNode.IsAnonymous()))
                {
                    // So, now we have a good owl-individual
                    Console.WriteLine(indNode.ID);
                }
            }
        }

		public void test10(string file) 
		{
			IOwlParser parser = new OwlXmlParser();
			IOwlGraph graph = parser.ParseOwl(file);

			IOwlGenerator generator = new OwlXmlGenerator();
			ArrayList errors = ((OwlGenerator)generator).Errors;
			ArrayList warnings = ((OwlGenerator)generator).Warnings;
			ArrayList messages = ((OwlGenerator)generator).Messages;

			FileStream info = new FileStream("c:/info.txt", FileMode.OpenOrCreate);
			StreamWriter sw = new StreamWriter(info);
			sw.AutoFlush = true;

			generator.GenerateOwl(graph, @"c:\test.owl");

			foreach(string msg in messages)
			{
				sw.WriteLine(msg);
			}
		}

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			Test t = new Test();

			// First Example:
			// Reading an ontology from a file and afterwards writing it again to another file
			//t.test1(args[0]); 

			// Second Example:
			// Creating a new ontology with a two classes and making one class a subclass of
			// another class and finally writing it to file
			//t.test2();

			// Third Example:
			// Reading an ontology from a file and retrieving some information from it
			//t.test3("C:\\travel.owl");

			//t.test2("C:\\test.owl");
			//t.test9("C:\\travel.owl");
			t.test10("C:\\domain_reduced.owl");
		}
	}
}
