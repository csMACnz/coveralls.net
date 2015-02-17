using System;

namespace csmacnz.Coveralls
{
	public class FileCoverageData
	{
		public FileCoverageData(string fullPath, int?[] coverage)
		{
			if (string.IsNullOrEmpty(fullPath)) throw new ArgumentException("fullPath");
			if (coverage == null) throw new ArgumentException("coverage");

			FullPath = fullPath;
			Coverage = coverage;
		}

		public string FullPath { get; private set; }

		public int?[] Coverage { get; private set; }
	}
}