namespace AnalyticByPopularity
{
	public class FolderPair : LocalFile
	{
		public string Parent { get; set; }
		public string Child { get; set; }
		public string RegexDirectChild { get; set; }
		public string RegexNonDirectChild { get; set; }
		public int Popularity { get; set; }

		public override bool Equals(object obj)
		{
			var pair = obj as FolderPair;
			return Parent.Equals(pair.Parent) &&
			       Child.Equals(pair.Child) &&
			       RegexNonDirectChild.Equals(pair.RegexNonDirectChild) &&
			       RegexNonDirectChild.Equals(pair.RegexNonDirectChild) &&
			       Popularity == pair.Popularity &&
			       FileName == pair.FileName;
		}

		public override int GetHashCode()
		{
			return Parent.GetHashCode() + Child.GetHashCode() + Popularity.GetHashCode() + FileName.GetHashCode();
		}

		public override string ToString()
		{
			return $"[{FileName}] {Parent} / {Child} : {Popularity}";
		}
	}
}