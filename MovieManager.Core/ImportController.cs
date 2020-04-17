using MovieManager.Core.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Utils;

namespace MovieManager.Core
{
	public class ImportController
	{
		const string Filename = "movies.csv";

		/// <summary>
		/// Liefert die Movies mit den dazugehörigen Kategorien
		/// </summary>
		public static IEnumerable<Movie> ReadFromCsv()
		{
			//var path = MyFile.GetFullFolderNameInApplicationTree(Filename);
			var path = MyFile.GetFullNameInApplicationTree(Filename);
			Dictionary<string, Category> categories = File.ReadAllLines(path)
				.Skip(1)
				.Select(s => s.Split(';'))
				.Select(s => s?[2])
				.Distinct()
				.ToDictionary(s => s, s => new Category { CategoryName = s });

			return File.ReadAllLines(path)
				.Skip(1)
				.Select(s => s.Split(';'))
				.Select(s => new Movie()
				{
					Title = s?[0],
					Duration = int.Parse(s?[3]),
					Year = int.Parse(s?[1]),
					Category = categories.TryGetValue(s?[2], out Category category) ? category : new Category { CategoryName = s[2] }
				})
				.ToArray();
		}
	}
}
