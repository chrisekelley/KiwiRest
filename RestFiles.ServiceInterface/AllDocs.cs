using System;
using System.Collections.Generic;

namespace RestFiles.ServiceInterface
{
	public class AllDocs
	{
		protected int total_rows { get; set; }
		protected int offset { get; set; }
		List<SimpleFingerprint> rows { get; set; }

		public AllDocs ()
		{
		}
	}
}

