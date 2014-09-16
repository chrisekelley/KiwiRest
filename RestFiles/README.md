# Kiwi REST

SourceAFIS fingerprint .NET server using ServiceStack REST example

This project is based on the REST ServiceStack Example project. More information on that follows.

## How it works

When the server starts, Global.ASAX runs GetPersonListAsync to fetch all_docs from the CouchDB prints database and populates the database List.

When the FileService accepts an image, it uses this database to Identify the print.

## Status

09-16-2014

Accepts an image sent to it:

	curl -F "uuid=12345" -F "filecomment=This is an image file" -F "image=@/Users/chrisk/source/KiwiRest/RestFiles/App_Data/files/register-1402505605894.png" http://0.0.0.0:8080/files

CouchDB integration work is not complete. There are currently issues de-serializing the CouchDB response. Currently using the ServiceStack JsonSerializer:

	JsonSerializer.DeserializeFromString<JsonObject>(doc);

Consider using MyCouch to deserialise the graph. See: http://danielwertheim.se/2013/05/26/get-up-and-running-with-couchdb-and-c-using-mycouch-on-windows/

  var response = db.Views.RunQuery<Album[]>(query);
  
See also the wiki: https://github.com/danielwertheim/mycouch/wiki/documentation#documents-vs-entities

  var query = new QueryViewRequest("_all_docs");
  var response = await client.Views.QueryAsync<dynamic>(query);
    
Another example using an Entity:

  var query = new QueryViewRequest("artists", "albums").Configure(query => query
      .Limit(5)
      .Reduce(false));

    ViewQueryResponse<Album[]> result = await client.Views.QueryAsync<Album[]>(query);

#The REST Files

The Rest files is a ServiceStack Example project providing complete management of your remote filesystem,
over a REST-ful web services API, in a GitHub browser-like widget.

## Client Info

The client is written in [1 static default.html page, and uses only jQuery](https://github.com/ServiceStack/ServiceStack.Examples/blob/master/src/RestFiles/RestFiles/default.htm)
Because of the advanced HTML5 features used its best viewed in a modern browser (i.e. anything recent that's not IE)

  * CSS3 is used for folder browser animations
  * HTML5 History State API is used for ajax state and page navigation

## Server Info

The /files service exposes a complete strong-typed REST-ful API, the entire implementation of which fits in only   
[1 C# class](https://github.com/ServiceStack/ServiceStack.Examples/blob/master/src/RestFiles/RestFiles.ServiceInterface/FilesService.cs).

As it was developed using the http://servicestack.net Open Source .NET/Mono Web Services Framework
it also able to expose this REST-ful API over a myraid of formats (with no extra code/config):

  * [json](http://servicestack.net/RestFiles/files/dtos/Types?format=json)
  * [xml](http://servicestack.net/RestFiles/files/dtos/Types?format=xml)
  * [jsv](http://servicestack.net/RestFiles/files/dtos/Types?format=jsv&debug=true)
  * [csv](http://servicestack.net/RestFiles/files/dtos/Types?format=csv)

*Note: The speed of web services are faster than what they appear, as the delay + animations are for
 gratuitous purposes only :) All but the xml format uses the high-performance cross-platform,
 serializers in ServiceStack.Text. e.g the JsonSerializer serializer used is over 3.6x faster
 that the fastest JSON Serialzer shipped with .NET, see:
 [the Northwind Benchmarks](http://www.servicestack.net/benchmarks/NorthwindDatabaseRowsSerialization.100000-times.2010-08-17.html)

SOAP 1.1/1.2 endpoints are also available at the following url:

  * [soap11](http://servicestack.net/RestFiles/servicestack/soap11)
  * [soap12](http://servicestack.net/RestFiles/servicestack/soap12)

As a result of the strong-typed DTO pattern used to define the the webservice, ServiceStack is able to
generate the xsds, wsdls, metadata documentation on the fly at:

  * [docs](http://servicestack.net/RestFiles/servicestack/metadata)
  * [xsd](http://servicestack.net/RestFiles/servicestack/metadata?xsd=1)
  * [wsdl](http://servicestack.net/RestFiles/servicestack/soap12) (HTTP GET)

Caveat: XmlSchema is not fully implemented as of MONO <= 2.8, so in many cases you will need to
host your service on a Windows/.NET server to view your web services XSD and WSDLS.

## Live Demo

The live demo is hosted on Linux (Cent OS) / Nginx using [MONO](http://www.mono-project.com)

*Not affiliated with GitHub, which is a trademark of GitHub Inc.
