using MovieManager.Core;
using MovieManager.Persistence;
using System;
using System.Linq;

namespace MovieManager.ImportConsole
{
	class Program
	{
		static void Main(string[] args)
		{
			InitData();
			AnalyzeData();

			Console.WriteLine();
			Console.Write("Beenden mit Eingabetaste ...");
			Console.ReadLine();
		}

		private static void InitData()
		{
			Console.WriteLine("***************************");
			Console.WriteLine("          Import");
			Console.WriteLine("***************************");

			Console.WriteLine("Import der Movies und Categories in die Datenbank");
			using (UnitOfWork unitOfWork = new UnitOfWork())
			{
				Console.WriteLine("Datenbank löschen");
				unitOfWork.DeleteDatabase();

				Console.WriteLine("Datenbank migrieren");
				unitOfWork.MigrateDatabase();

				Console.WriteLine("Movies/Categories werden eingelesen");

				var movies = ImportController.ReadFromCsv().ToArray();
				if (movies.Length == 0)
				{
					Console.WriteLine("!!! Es wurden keine Movies eingelesen");
					return;
				}

				var categories = movies
					.Select(s => s.Category)
					.Distinct()
					.ToArray();
				unitOfWork.MovieRepository.AddRange(movies);
				unitOfWork.CategoryRepository.AddRange(categories);
				Console.WriteLine($"  Es wurden {movies.Count()} Movies in {categories.Count()} Kategorien eingelesen!");

				unitOfWork.Save();

				Console.WriteLine();
			}
		}

		private static void AnalyzeData()
		{
			Console.WriteLine("***************************");
			Console.WriteLine("        Statistik");
			Console.WriteLine("***************************");

			using (UnitOfWork unitOfWork = new UnitOfWork())
			{
				// Längster Film: Bei mehreren gleichlangen Filmen, soll jener angezeigt werden, dessen Titel im Alphabet am weitesten vorne steht.
				// Die Dauer des längsten Films soll in Stunden und Minuten angezeigt werden!
				var movie = unitOfWork.MovieRepository.GetMoviesByDuration();
				Console.WriteLine($" Längster Film: {movie.Title} Länge: {GetDurationAsString(movie.Duration)}");

// KORREKTUR - ab Hier da alter Diesel, ohne Turbo, fährt nicht schneller

				// Top Kategorie:
				//   - Jene Kategorie mit den meisten Filmen.
				var kategorie = unitOfWork.CategoryRepository.GetCategoryWithMostMovies();
				Console.WriteLine($" Kategorie mit den meisten Filmen: '{kategorie.Item1.CategoryName}'; Filme: {kategorie.Item2}");

				// Jahr der Kategorie "Action":
				//  - In welchem Jahr wurden die meisten Action-Filme veröffentlicht?
				int kat = unitOfWork.MovieRepository.GetCategoryWithYear();
				Console.WriteLine($" Jahr der Action-Filme: {kat}");

				// Kategorie Auswertung (Teil 1):
				//   - Eine Liste in der je Kategorie die Anzahl der Filme und deren Gesamtdauer dargestellt wird.
				//   - Sortiert nach dem Namen der Kategorie (aufsteigend).
				//   - Die Gesamtdauer soll in Stunden und Minuten angezeigt werden!
				var groupMovByCat = unitOfWork.CategoryRepository.GetGroupMoviesByCategory();
				Console.WriteLine("\nKategorie Auswertung:\n");
				Console.WriteLine("Kategorie	Anzahl	Gesamtdauer");
				Console.WriteLine("====================================");
				foreach (var (categor, count, durateSum) in groupMovByCat)
				{
					Console.WriteLine($"{categor, -12}\t{count, -4}\t{GetDurationAsString(durateSum, false), -12}");
				}

				// Kategorie Auswertung (Teil 2):
				//   - Alle Kategorien und die durchschnittliche Dauer der Filme der Kategorie
				//   - Absteigend sortiert nach der durchschnittlichen Dauer der Filme.
				//     Bei gleicher Dauer dann nach dem Namen der Kategorie aufsteigend sortieren.
				//   - Die Gesamtdauer soll in Stunden, Minuten und Sekunden angezeigt werden!
				var groupMovByAvgTime = unitOfWork.CategoryRepository.GetGroupMoviesByAvgDuration();
				Console.WriteLine();
				Console.WriteLine("Kategorie	Durchschn. Gesamtdauer");
				Console.WriteLine("====================================");
				foreach (var (categor, avgSum) in groupMovByAvgTime)
				{
					Console.WriteLine($"{categor,-12}\t{GetDurationAsString(avgSum),-12}");
				}
			}
		}

		private static string GetDurationAsString(double minutes, bool withSeconds = true)
		{
			int h = (int)minutes / 60;
			int m = (int)minutes % 60;
			int s = (int)((minutes - (((int)minutes / 60 * 60) + (int)minutes % 60)) * 60);
			string outputString;

			if (withSeconds)
			{
				outputString = $"{h, 0:D2} h {m, 0:D2} min {s, 0:D2} sec";
			}
			else
			{
				outputString =  $"{h, 0:D2} h {m} min";
			}

			return outputString;
		}
	}
}
