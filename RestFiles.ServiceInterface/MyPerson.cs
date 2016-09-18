using System;
using SourceAFIS.Simple;
using System.Collections.Generic;

namespace RestFiles.ServiceInterface
{
	public class MyPerson: Person
	{
		public string Name { get; set; }
		public string Filename { get; set; }
		public string Uuid { get; set; }
		public string _id { get; set; }
		public string _rev { get; set; }
		public string doctype { get; set; }
		public SimpleFingerprint simpleFingerprint { get; set; }
		public List<SimpleFingerprint> SimpleFingerprints { get; set; }

		public MyPerson ()
		{

		}
	}
}

