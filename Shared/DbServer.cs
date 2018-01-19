using System;
using System.Data.SqlClient;
using System.IO;

namespace Shared
{
	public class DbServer : IDisposable
	{
		private readonly SqlConnection _connection;
		private readonly DirectoryInfo outputFolder;

		public DbServer(string connection, string outputFolderPath)
		{
			_connection = new SqlConnection(connection);
			_connection.Open();

			if (Directory.Exists(outputFolderPath))
				Directory.Delete(outputFolderPath, true);

			outputFolder = Directory.CreateDirectory(outputFolderPath);
		}

		public void Dispose()
		{
			_connection.Close();
		}

		public SqlCommand GetSqlCommand(string filePath, int? id = null)
		{
			var script = File.ReadAllText(filePath);
			var command = new SqlCommand(script, _connection);

			if (id.HasValue)
				command.Parameters.AddWithValue("@industryGroupCode", id);

			return command;
		}

		public void CreateMissingSqlFeatures()
		{
			GetSqlCommand("../../SQL/Check.sql").ExecuteNonQuery();
			GetSqlCommand("../../SQL/Create.sql").ExecuteNonQuery();
		}

		public string GenerateOutputFile(string name)
		{
			var path = Path.Combine(outputFolder.FullName, name + ".txt");

			if (File.Exists(path))
				File.Delete(path);

			return path;
		}
	}
}