# owldotnetapi
The OwlDotNetApi is an OWL (Web Ontology Language) API and parser written in C# for the .NET platform based on the Drive RDF parser. Its fully compliant with the W3C OWL syntax specification and can be used within any .NET language.

When looking for a toolkit or an API to manipulate ontologies, the Jena Semantic Web Toolkit is practically the only decent you will find. However, when your application is written in .NET you will need to connect to Java which will seriously reduce the speed of your application. Therefore, I have developed this OWL API in C# which is based on the Drive RDF parser for the .NET platform.

The API uses the underlying data model from Drive to build a directed linked graph from the OWL ontology. The RDF parser itself has been modified to parse OWL ontologies instead. Furthermore, the application has been extended in order to allow the generation of an OWL ontology from the internal model as well. 
