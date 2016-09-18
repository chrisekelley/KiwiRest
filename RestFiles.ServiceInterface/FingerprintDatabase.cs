using System;
using SourceAFIS.Simple;
using System.Collections.Generic;

namespace RestFiles.ServiceInterface
{
	public class FingerprintDatabase
	{
		public FingerprintDatabase()
		{
		}

		public List<MyPerson> people = new List<MyPerson>();
		public void AddData(MyPerson person)
		{
			lock (people) people.Add(person);
		}
	}
}

