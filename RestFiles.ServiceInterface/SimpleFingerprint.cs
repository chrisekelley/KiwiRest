using System;
using SourceAFIS.Simple;

namespace RestFiles.ServiceInterface
{
	public class SimpleFingerprint:	Fingerprint
	{
		public String Filename { get; set; }
//		public byte[] Template { get; set; }
		public String Base64Template { get; set; }
		public String Uuid { get; set; }
		public String _id { get; set; }
		public String _rev { get; set; }
		public String doctype { get; set; }
		public String DateUploaded { get; set; }

		public SimpleFingerprint ()
		{
		}
	}
}

