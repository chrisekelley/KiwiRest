using System.Collections.Generic;
using System.IO;
using NServiceKit;
using NServiceKit.Configuration;
using NServiceKit.Common;
using NServiceKit.Common.Utils;

namespace RestFiles.ServiceInterface
{
	public class AppConfig
	{
		public AppConfig()
		{
			this.TextFileExtensions = new List<string>();
			this.ExcludeDirectories = new List<string>();
		}

		//public AppConfig(IAppSettings resources)
		public AppConfig(IResourceManager resources)
		{
			this.RootDirectory = resources.GetString("RootDirectory").MapHostAbsolutePath()
				.Replace('\\', Path.DirectorySeparatorChar);

			this.TextFileExtensions = resources.GetList("TextFileExtensions");
			this.ExcludeDirectories = resources.GetList("ExcludeDirectories");
		}

		public string RootDirectory { get; set; }

		public IList<string> TextFileExtensions { get; set; }

		public IList<string> ExcludeDirectories { get; set; }

		public interface IAppSettings
		{
			Dictionary<string, string> GetAll();

			List<string> GetAllKeys();

			bool Exists(string key);

			void Set<T>(string key, T value);

			string GetString(string name);

			IList<string> GetList(string key);

			IDictionary<string, string> GetDictionary(string key);

			T Get<T>(string name);

			T Get<T>(string name, T defaultValue);
		}
	}
}