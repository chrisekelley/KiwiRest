namespace RestFiles.ServiceInterface
{
	public class Response
	{

		public int StatusCode { get; set; }
		public string Error { get; set; }
		public string UID { get; set; }
		public float Threshold { get; set; }

		public Response()
		{
		}
	}
}

