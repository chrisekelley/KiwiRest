namespace RestFiles.ServiceInterface
{
	public class Response
	{

		public int StatusCode { get; set; }
		public string Error { get; set; }
		public string UID { get; set; }
		public string Threshold { get; set; }

		public Response()
		{
		}
	}
}

