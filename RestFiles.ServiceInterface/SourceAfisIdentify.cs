using System;
using System.Drawing;
using SourceAFIS.Simple;


namespace RestFiles.ServiceInterface
{
	public class SourceAfisIdentify
	{
		public SourceAfisIdentify ()
		{
		}

//		static AfisEngine Afis = new AfisEngine();

//		// Inherit from Fingerprint in order to add Filename field
//		[Serializable]
//		class MyFingerprint : Fingerprint
//		{
//			public string Filename;
//		}
//
//		// Inherit from Person in order to add Name field
//		[Serializable]
//		class MyPerson : Person
//		{
//			public string Name;
//		}

		// Take fingerprint image file and create Person object from the image
		public static MyPerson Enroll(string filename, string name, AfisEngine Afis)
		{
			Console.WriteLine("Enrolling {0}...", name);

			// Initialize empty fingerprint object and set properties
			MyFingerprint fp = new MyFingerprint();
			fp.Filename = filename;
			// Load image from the file
			Console.WriteLine(" Loading image from {0}...", filename);
			//            BitmapImage image = new BitmapImage(new Uri(filename, UriKind.RelativeOrAbsolute));
			Bitmap image = (Bitmap) Image.FromFile(@filename, true);
			//			Bitmap image = Bitmap.FromFile(@filename, true);

			fp.AsBitmap = image;
			// Above update of fp.AsBitmapSource initialized also raw image in fp.Image
			// Check raw image dimensions, Y axis is first, X axis is second
			Console.WriteLine(" Image size = {0} x {1} (width x height)", fp.Image.GetLength(1), fp.Image.GetLength(0));

			// Initialize empty person object and set its properties
			MyPerson person = new MyPerson();
			person.Name = name;
			person.Uuid = name;
			// Add fingerprint to the person
			person.Fingerprints.Add(fp);

			// Execute extraction in order to initialize fp.Template
			Console.WriteLine(" Extracting template...");
			Afis.Extract(person);
			// Check template size
			Console.WriteLine(" Template size = {0} bytes", fp.Template.Length);

			return person;
		}
	}
}

