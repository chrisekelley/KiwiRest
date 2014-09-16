using System;
using System.IO;
using Funq;
using RestFiles.ServiceInterface;
using ServiceStack;
using ServiceStack.Configuration;
using System.Collections.Generic;
using SourceAFIS.Simple;
using System.Threading.Tasks;
using MyCouch.Requests;
using MyCouch.Responses;
using ServiceStack.Text;

namespace RestFiles
{
    /// <summary>
    /// Create your ServiceStack web service application with a singleton AppHost.
    /// </summary> 
    public class AppHost : AppHostBase  
    {
        /// <summary>
        /// Initializes a new instance of your ServiceStack application, with the specified name and assembly containing the services.
        /// </summary>
        public AppHost() : base("REST Files", typeof(FilesService).Assembly) {}

        /// <summary>
        /// Configure the container with the necessary routes for your ServiceStack application.
        /// </summary>
        /// <param name="container">The built-in IoC used with ServiceStack.</param>
        public override void Configure(Container container)
        {
            //Permit modern browsers (e.g. Firefox) to allow sending of any REST HTTP Method
            Plugins.Add(new CorsFeature());

            SetConfig(new HostConfig {
                DebugMode = true,
//				DefaultContentType = "text/json", //Change default content type
            });

            var config = new AppConfig(new AppSettings());
            container.Register(config);
//			List<MyPerson> database = PersonList.GetPersonListAsync();
			Task database = GetPersonListAsync(container);
//			container.Register(database);

            if (!Directory.Exists(config.RootDirectory))
            {
                Directory.CreateDirectory(config.RootDirectory);
            }

        }

//		async  Task<ViewQueryResponse<string>> GetPersonListAsync ()
		async  Task GetPersonListAsync (Container container)
		{
			List<MyPerson> database = new List<MyPerson>();
			string sourceDirectory = @"/Users/chrisk/source/KiwiRest/RestFiles/App_Data/files";
			AfisEngine Afis = new AfisEngine();
			DateTime date1 = DateTime.Now;
			Console.WriteLine("Starting PersonList:  " + date1);
//			WebClient client = new WebClient ();
			string uri = "http://localhost:5984/prints/_all_docs&include_docs=true";
			//			string data = client.DownloadString (uri);
			//			var fromJson = JsonSerializer.DeserializeFromString<AllDocs>(data);
			var myCouchClient = new MyCouch.MyCouchClient("http://localhost:5984/prints");
			ViewQueryResponse<string> result = null;
			try 
			{
				var queryView = new QueryViewRequest("_all_docs");
				queryView.Configure(query => query
					.IncludeDocs(true));
				result = await myCouchClient.Views.QueryAsync<string>(queryView);
				var rows = result.Rows;

//				foreach (ViewQueryResponse<SimpleFingerprint> row in rows)
				foreach (ViewQueryResponse<string,string>.Row row in rows)
				{
					Console.WriteLine("Lookin' at " + row);
					string doc = row.IncludedDoc;
					var person = new MyPerson ();
//					SimpleFingerprint print = TypeSerializer.DeserializeFromString<SimpleFingerprint>(doc);
					var jsonObj = JsonSerializer.DeserializeFromString<JsonObject>(doc);
					person._id = jsonObj["_id"];
					var jsonFingerprints = jsonObj["FingerprintTemplates"];
					var Filename = jsonObj["Filename"];
//					var serialFingerprints = JsonSerializer.DeserializeFromString<List<JsonObject>>(jsonFingerprints);
					var serialFingerprints = JsonArrayObjects.Parse(jsonFingerprints);
					List<Fingerprint> fingerprints = new List<Fingerprint> ();
					foreach (KeyValuePair<string,string> pair in serialFingerprints[0]) {
						Fingerprint simpleFingerprint = new Fingerprint ();
						String auPair = pair.Key;
						char[] delimiterChars = { ':'};
						string[] printPair = auPair.Split(delimiterChars);
						var print = printPair[1];
						byte[] printArray = System.Convert.FromBase64String(print);
						simpleFingerprint.Template = printArray;
//						simpleFingerprint.Filename = Filename;
						fingerprints.Add(simpleFingerprint);
//						foreach (KeyValuePair<string,string> pair in fprint) {
//							SimpleFingerprint simpleFingerprint = new SimpleFingerprint ();
//							var strBase64Template = JsonSerializer.DeserializeFromString<string>(pair.Key);
////							simpleFingerprint.Base64Template = strBase64Template["Base64Template"];
//							fingerprints.Add(simpleFingerprint);
//						}

					}
//					foreach (KeyValuePair<string,string> pair in serialFingerprints)
//					{
//						var prints = JsonSerializer.DeserializeFromString<JsonObject>(pair.Key);
//						foreach (KeyValuePair<string,string> print in prints)
//						{
//							SimpleFingerprint simpleFingerprint = new SimpleFingerprint ();
//							var strBase64Template = JsonSerializer.DeserializeFromString<string>(print.Key);
////							simpleFingerprint.Base64Template = print["Base64Template"];
//							fingerprints.Add(simpleFingerprint);
//						}
//					}
					person.Fingerprints = fingerprints;
					person.Filename = jsonObj["Filename"];
					person.Uuid = jsonObj["Uuid"];
					database.Add(person);
					Console.WriteLine("Work?");
				}
				container.Register(database);
			}
			catch (Exception e) {
				Console.WriteLine("Error: " + e);
			}

//			return result;
		}

    }
    
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            //Initialize your application
            (new AppHost()).Init();
        }
    }
}