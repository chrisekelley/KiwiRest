using System;
using System.IO;
using System.Net;
using RestFiles.ServiceInterface.Support;
using RestFiles.ServiceModel;
using RestFiles.ServiceModel.Types;
using NServiceKit;
using NServiceKit.Text;
using NServiceKit.ServiceInterface;
using NServiceKit.Common.Web;
using File = System.IO.File;
using SourceAFIS.Simple;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NServiceKit.ServiceHost;

namespace RestFiles.ServiceInterface
{
	/// <summary>
    /// Define your ServiceStack web service request (i.e. Request DTO).
    /// </summary>
    public class FilesService : Service
    {
        /// <summary>
        /// Gets or sets the AppConfig. The built-in IoC used with ServiceStack autowires this property.
        /// </summary>
        public AppConfig Config { get; set; }
		//public static List<MyPerson> Database { get; set; }
		public FingerprintDatabase FingerprintDatabase { get; set; }

		static AfisEngine Afis = new AfisEngine();

        public object Get(Files request)
        {
            var targetFile = GetAndValidateExistingPath(request);

            var isDirectory = Directory.Exists(targetFile.FullName);

            if (!isDirectory && request.ForDownload)
                return new HttpResult(targetFile, asAttachment: true);

            var response = isDirectory
                ? new FilesResponse { Directory = GetFolderResult(targetFile.FullName) }
                : new FilesResponse { File = GetFileResult(targetFile) };

            return response;
        }

//		[AddHeader(ContentType = "text/json")]
		public object Post (Files request)
		{
			//var container = ServiceStackHost.Instance.Container;

			//			var response = new MyPerson { };
			var response = "";
			var message = "";
			var uuid = "";
			var targetDir = GetPath (request);

			var isExistingFile = targetDir.Exists
			                              && (targetDir.Attributes & FileAttributes.Directory) != FileAttributes.Directory;

			if (isExistingFile)
				throw new NotSupportedException (
					"POST only supports uploading new files. Use PUT to replace contents of an existing file");

			if (!Directory.Exists (targetDir.FullName))
				Directory.CreateDirectory (targetDir.FullName);

			if (base.Request.Files.Length == 0) {
//				message = "No files uploaded.";
			}

			foreach(var key in base.Request.FormData.AllKeys) {
				foreach(var val in base.Request.FormData.GetValues(key)) {
					if (key == "uuid") {
						uuid = val;
						Console.WriteLine (string.Format("{0}: {1}", key, val) + " uuid set to: " + uuid);
					}
				}
			}
				
			foreach (var uploadedFile in base.Request.Files) {
				var newFilePath = Path.Combine (targetDir.FullName, uploadedFile.FileName);
//					if (uploadedFile.StartsWith("verify") {
//					}
				uploadedFile.SaveTo (newFilePath);

				// Enroll visitor with unknown identity
				Guid g;
				// Create and display the value of two GUIDs.
				g = Guid.NewGuid ();
				DateTime date1 = DateTime.Now;
				Console.WriteLine("Starting Enroll:  " + date1);
				MyPerson probe = SourceAfisIdentify.Enroll (newFilePath, g.ToString (), Afis);
				// Look up the probe using Threshold = 10
				Afis.Threshold = 10;
				DateTime date2 = DateTime.Now;
				var list = FingerprintDatabase.people;
				//Console.WriteLine("Identifying {0} in Database of {1} persons...", probe.Name, list.Count + " at " + date2);
				Console.WriteLine ("{0} : Identifying {1} in Database of {2} persons...", date2, probe.Name, list.Count);

				MyPerson match = Afis.Identify (probe, list).FirstOrDefault () as MyPerson;

				DateTime date3 = DateTime.Now;
				var diffInSeconds = (date3 - date2).TotalSeconds;
				Console.WriteLine("Enroll time:  " + diffInSeconds + " seconds");
				diffInSeconds = (date2 - date1).TotalSeconds;
				Console.WriteLine("Total enroll + match time:  " + diffInSeconds + " seconds");

				Response jsonResponse = new Response();

				// Null result means that there is no candidate with similarity score above threshold
				if (match == null) {
					message = "NoMatch";
					jsonResponse.StatusCode = 3;
					jsonResponse.Error = message;
					jsonResponse.UID = g.ToString();
					Console.WriteLine (message);
					//Database.Add (probe);
					FingerprintDatabase.AddData(probe);
					String url = "http://localhost:5984/prints/" + g.ToString ();
//					String data = "{_id: " + "\"" + g.ToString ()  + "\"" + ", fileName:" + "\""  + uploadedFile.FileName + "\""  + "}";
					var person = new MyPerson ();
//					person.Filename = uploadedFile.FileName;
					person.Uuid = g.ToString ();
					List<Fingerprint> probleFingerprints = probe.Fingerprints;
					List<SimpleFingerprint> fingerprints = new List<SimpleFingerprint> ();

					foreach(var print in probleFingerprints) {
						byte[] template = print.Template;
						// Convert the binary input into Base64 UUEncoded output. 
						string base64String = null;
						try {
							base64String = System.Convert.ToBase64String(template, 0,template.Length);
						}
						catch (System.ArgumentNullException) {
							System.Console.WriteLine("Binary data array is null.");
						}

						SimpleFingerprint fprint = new SimpleFingerprint ();
						fprint.Base64Template = base64String;
						fprint.Filename = uploadedFile.FileName;
						DateTime dateUploaded = DateTime.Now;
						string isoJson = JsonConvert.SerializeObject(dateUploaded, Formatting.None, new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd hh:mm:ss" });
						fprint.DateUploaded = isoJson;
						//fingerprints.Add (fprint);
						person.simpleFingerprint = fprint;
					}

					person.SimpleFingerprints = fingerprints;

					var json = NServiceKit.Text.JsonSerializer.SerializeToString(person);
					Console.WriteLine (json);
					WebClient client = new WebClient ();
					try {
						client.UploadString (url, "PUT", json);
					} catch (Exception ex) {
						Console.WriteLine (ex);
					}
//					response = new FilesResponse { File = new FileResult
//						{
//							Name = uploadedFile.FileName,
//							IsTextFile = false,
//							Contents = message,
//						}
//					};

//					response = probe;
					string jsonString = JsonConvert.SerializeObject(jsonResponse);
					response = jsonString;
				} else {
					// Print out any non-null result
					Console.WriteLine ("Probe {0} matches registered person {1}", probe.Name, match.Name);

					// Compute similarity score
					DateTime date4 = DateTime.Now;
					float score = Afis.Verify (probe, match);
					Console.WriteLine ("Similarity score between {0} and {1} = {2:F3}", probe.Name, match.Name, score);
					DateTime date5 = DateTime.Now;
					diffInSeconds = (date5 - date4).TotalSeconds;
					Console.WriteLine("Verify time:  " + diffInSeconds + " seconds");
					message = "Match: " + probe.Name + " matches " + match.Name + " Score: " + score;
//					response = new FilesResponse { File = new FileResult
//						{
//							Name = uploadedFile.FileName,
//							IsTextFile = true,
//							Contents = message,
//						}
//					};
//					response = match;
					response = message;
				}
			}
			return response;
		}

        public void Put(Files request)
        {
            var targetFile = GetAndValidateExistingPath(request);

            if (!this.Config.TextFileExtensions.Contains(targetFile.Extension))
                throw new NotSupportedException("PUT Can only update text files, not: " + targetFile.Extension);

            if (request.TextContents == null)
                throw new ArgumentNullException("TextContents");

            File.WriteAllText(targetFile.FullName, request.TextContents);
        }

        public void Delete(Files request)
        {
            var targetFile = GetAndValidateExistingPath(request);
            File.Delete(targetFile.FullName);
        }

        private FolderResult GetFolderResult(string targetPath)
        {
            var result = new FolderResult();

            foreach (var dirPath in Directory.GetDirectories(targetPath))
            {
                var dirInfo = new DirectoryInfo(dirPath);

                if (this.Config.ExcludeDirectories.Contains(dirInfo.Name)) continue;

                result.Folders.Add(new Folder
                {
                    Name = dirInfo.Name,
                    ModifiedDate = dirInfo.LastWriteTimeUtc,
                    FileCount = dirInfo.GetFiles().Length
                });
            }

            foreach (var filePath in Directory.GetFiles(targetPath))
            {
                var fileInfo = new FileInfo(filePath);

                result.Files.Add(new ServiceModel.Types.File
                {
                    Name = fileInfo.Name,
                    Extension = fileInfo.Extension,
                    FileSizeBytes = fileInfo.Length,
                    ModifiedDate = fileInfo.LastWriteTimeUtc,
                    IsTextFile = Config.TextFileExtensions.Contains(fileInfo.Extension),
                });
            }

            return result;
        }

        private FileInfo GetPath(Files request)
        {
            return new FileInfo(Path.Combine(this.Config.RootDirectory, request.Path.GetSafePath()));
        }

        private FileInfo GetAndValidateExistingPath(Files request)
        {
            var targetFile = GetPath(request);
            if (!targetFile.Exists && !Directory.Exists(targetFile.FullName))
                throw new HttpError(HttpStatusCode.NotFound, new FileNotFoundException("Could not find: " + request.Path));

            return targetFile;
        }

        private FileResult GetFileResult(FileInfo fileInfo)
        {
            var isTextFile = this.Config.TextFileExtensions.Contains(fileInfo.Extension);

            return new FileResult
            {
                Name = fileInfo.Name,
                Extension = fileInfo.Extension,
                FileSizeBytes = fileInfo.Length,
                IsTextFile = isTextFile,
                Contents = isTextFile ? File.ReadAllText(fileInfo.FullName) : null,
                ModifiedDate = fileInfo.LastWriteTimeUtc,
            };
        }
    }
}