using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Shared;

namespace PopularityQuery
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			using (var server = new DbServer(ConfigurationManager.AppSettings["connection"], ConfigurationManager.AppSettings["outputFolder"]))
			{
				Dictionary<int, string> dictionary = new Dictionary<int, string>();

				var selectCommand = server.GetSqlCommand("../../SQL/Select.sql");
				using (var reader = selectCommand.ExecuteReader())
				{
					while (reader.Read())
					{
						var key = Convert.ToInt32(reader["GicsIndustryGroupCode"]);
						var value = Convert.ToString(reader["GicsIndustryGroupName"]);

						if (!dictionary.ContainsKey(key))
						{
							dictionary.Add(key, value);
						}
					}
				}
				
				foreach (var id in dictionary.Keys)
				{
					var command = server.GetSqlCommand("../../SQL/Get.sql", id);
					var path = server.GenerateOutputFile(dictionary[id]);

					using (var sw = File.CreateText(path))
					{
						using (var reader = command.ExecuteReader())
						{
							while (reader.Read())
							{
								var text = $"{reader["GicsIndustryGroupName"]}, " +
										   $"{reader["FolderName"]}, " +
										   $"{reader["DataRoom Appear"]}";
								sw.WriteLine(text);
							}
						}
					}
				}
			}
		}
	}
}