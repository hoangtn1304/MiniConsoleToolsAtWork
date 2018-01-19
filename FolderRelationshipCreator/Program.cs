using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using FolderRelationshipCreator.CsvHelper;

namespace FolderRelationshipCreator
{
	public class Program
	{
		private const int Popularity = 5;

		private static void Main(string[] args)
		{
			var dataFolder = Directory.CreateDirectory("../../data");
			//var dataFolder = Directory.CreateDirectory("../../test");
			if (!dataFolder.Exists)
				throw new Exception("Data folder should exist.");

			var outputFolder = ConfigurationManager.AppSettings["outputFolder"];
			if (!Directory.Exists(outputFolder))
			{
				Directory.CreateDirectory(outputFolder);
			}
			
			int i = 0;
			foreach (var file in dataFolder.EnumerateFiles("*.txt"))
			{
				Console.WriteLine("PROCESSING for {0}/24 files ...", i++);

				var content = ProduceDirectChild(file);

				var path = Path.Combine(outputFolder, Path.GetFileName(file.Name));

				if (File.Exists(path))
				{
					File.Delete(path);
				}

				using (var sw = File.CreateText(path))
				{
					sw.WriteLine(content);
				}

				var template = @"../../template/index.html";
				var text = File.ReadAllText(template);
				var newContent = text.Replace("{SOURCE-TO-UPDATE}", content);
				var newFile = Path.Combine(outputFolder, Path.GetFileNameWithoutExtension(file.Name) + ".html");

				if (File.Exists(newFile))
				{
					File.Delete(newFile);
				}

				using (var writer = File.CreateText(newFile))
				{
					writer.WriteLine(newContent);
				}
			}

			Console.WriteLine("FINISH for 24 files with popularity {0}", Popularity);
			//Console.Read();
		}

		private static string ProduceDirectChild(FileInfo file)
		{
			var source = file.FullName;

			var folderParentChildPairs = new List<FolderParentChildPair>();
			using (var reader = new CsvFileReader(source))
			{
				var row = new CsvRow();
				while (reader.ReadRow(row))
				{
					var dataRoomId = int.Parse(row[0]);
					var folderId = int.Parse(row[1]);
					if (row.Count > 2)
					{
						var folderFullPath = row[2];
						if (!string.IsNullOrEmpty(folderFullPath))
						{
							var folders = folderFullPath.Split(new[] { " \\ " }, StringSplitOptions.RemoveEmptyEntries);
							folders.Aggregate((current, next) =>
											  {
												  folderParentChildPairs.Add(new FolderParentChildPair
												  {
													  Parent = current,
													  Child = next,
													  DataRoomId = dataRoomId,
													  FullRelationshipName = $"{current} -> {next}"
												  });
												  return next;
											  });
						}
					}
				}
			}

			var whereClause = folderParentChildPairs
				.Where(f => !f.Child.StartsWith("Level") && !f.Parent.StartsWith("Level")).ToList();

			var groupClause = whereClause.GroupBy(f => f.FullRelationshipName).ToList();

			var result = groupClause
				.Select(f => new
				{
					Relationship = f.Key,
					Popular = f.Select(item => item.DataRoomId).Distinct().Count()
				})
				.ToList();

			var top50 = result.Where(r => r.Popular >= Popularity).ToList();
			top50 = top50.OrderByDescending(r => r.Popular).ToList();

			var stringBuilder = new StringBuilder();

			stringBuilder = new StringBuilder();
			for (var i = 0; i < top50.Count; i++)
			{
				var node = folderParentChildPairs.Where(f => f.FullRelationshipName == top50[i].Relationship).FirstOrDefault();
				stringBuilder.AppendLine(string.Format("{{source: \"{0}\", target: \"{1}\", value: \"{2}\",  type: \"suit\"}},", node.Child, node.Parent, top50[i].Popular));
			}

			return stringBuilder.ToString();
		}

		private static string ProduceAllChild(FileInfo file)
		{
			var source = file.FullName;

			var folderParentChildPairs = new List<FolderParentChildPair>();
			using (var reader = new CsvFileReader(source))
			{
				var row = new CsvRow();
				while (reader.ReadRow(row))
				{
					var dataRoomId = int.Parse(row[0]);
					var folderId = int.Parse(row[1]);
					if (row.Count > 2)
					{
						var folderFullPath = row[2];
						if (!string.IsNullOrEmpty(folderFullPath))
						{
							var folders = folderFullPath.Split(new[] { " \\ " }, StringSplitOptions.RemoveEmptyEntries);
							folders.Aggregate((current, next) =>
							                  {
								                  folderParentChildPairs.Add(new FolderParentChildPair
								                                             {
									                                             Parent = current,
									                                             Child = next,
									                                             DataRoomId = dataRoomId,
									                                             FullRelationshipName = $"{current} -> {next}"
								                                             });
								                  return next;
							                  });
						}
					}
				}
			}

			var whereClause = folderParentChildPairs
				.Where(f => !f.Child.StartsWith("Level") && !f.Parent.StartsWith("Level")).ToList();

			var groupClause = whereClause.GroupBy(f => f.FullRelationshipName).ToList();

			var result = groupClause
				.Select(f => new
				             {
					             Relationship = f.Key,
					             Popular = f.Select(item => item.DataRoomId).Distinct().Count()
				             })
				.ToList();

			var top50 = result.Where(r => r.Popular >= Popularity).ToList();
			top50 = top50.OrderByDescending(r => r.Popular).ToList();

			var stringBuilder = new StringBuilder();

			stringBuilder = new StringBuilder();
			for (var i = 0; i < top50.Count; i++)
			{
				var node = folderParentChildPairs.Where(f => f.FullRelationshipName == top50[i].Relationship).FirstOrDefault();
				stringBuilder.AppendLine(string.Format("{{source: \"{0}\", target: \"{1}\", value: \"{2}\",  type: \"suit\"}},", node.Child, node.Parent, top50[i].Popular));
			}

			return stringBuilder.ToString();
		}
	}
}