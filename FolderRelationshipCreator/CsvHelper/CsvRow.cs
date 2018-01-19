using System.Collections.Generic;

namespace FolderRelationshipCreator.CsvHelper
{
	/// <summary>
	/// Class to store one CSV row
	/// </summary>
	public class CsvRow : List<string>
	{
		public string LineText { get; set; }
	}
}