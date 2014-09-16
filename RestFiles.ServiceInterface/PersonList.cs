using System;
using System.IO;
using System.Collections.Generic;
using SourceAFIS.Simple;
using System.Net;
using ServiceStack.Text;
using MyCouch.Requests;
using MyCouch.Responses;
using System.Threading.Tasks;



namespace RestFiles.ServiceInterface
{
	public class PersonList
	{
		public PersonList ()
		{
		}

		async  Task<ViewQueryResponse<string>> GetPersonListAsync ()
		{
			List<MyPerson> database = new List<MyPerson>();
			string sourceDirectory = @"/Users/chrisk/source/KiwiRest/RestFiles/App_Data/files";
			AfisEngine Afis = new AfisEngine();
			DateTime date1 = DateTime.Now;
			Console.WriteLine("Starting PersonList:  " + date1);
			WebClient client = new WebClient ();
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
			}
			catch (Exception e) {
			}

			return result;
		}

	     public static List<MyPerson> GetPersonList ()
		{
			List<MyPerson> database = new List<MyPerson>();
			string sourceDirectory = @"/Users/chrisk/source/KiwiRest/RestFiles/App_Data/files";
			AfisEngine Afis = new AfisEngine();
			DateTime date1 = DateTime.Now;
			Console.WriteLine("Starting PersonList:  " + date1);
			WebClient client = new WebClient ();
			string uri = "http://localhost:5984/prints/_all_docs&include_docs=true";
//			string data = client.DownloadString (uri);
//			var fromJson = JsonSerializer.DeserializeFromString<AllDocs>(data);
			var myCouchClient = new MyCouch.MyCouchClient("http://localhost:5984/prints");
			try 
			{
				var queryView = new QueryViewRequest("_all_docs");
				queryView.Configure(query => query
					.IncludeDocs(true));
			}
			catch (Exception e) {
			}

			try
			{
				var files = Directory.EnumerateFiles(sourceDirectory, "*.png");

				foreach (string currentFile in files)
				{
					DateTime date3 = DateTime.Now;
					Console.WriteLine("Processing " + currentFile + " at " + date3);
					string fileName = currentFile.Substring(sourceDirectory.Length + 1);
					//					Directory.Move(currentFile, Path.Combine(archiveDirectory, fileName));
					Guid g = Guid.NewGuid ();
					var guidStr = g.ToString ();
					MyPerson probe = SourceAfisIdentify.Enroll (currentFile, guidStr, Afis);
					Console.WriteLine("Adding " + guidStr);
					DateTime date4 = DateTime.Now;
					Console.WriteLine("Processed  " + currentFile + " at " + date4 + " uuid: " + guidStr);
					var diffInSeconds = (date4 - date3).TotalSeconds;
					Console.WriteLine("Finished " + guidStr + " at " + date4 + " Total time: " + diffInSeconds + " seconds");
					database.Add (probe);
				}
				DateTime date2 = DateTime.Now;
				var diffInSeconds2 = (date2 - date1).TotalSeconds;
				Console.WriteLine("Finished PersonList at " + date2 + " Total time: " + diffInSeconds2 + " seconds");
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}

			return database;
		}
	}
}

