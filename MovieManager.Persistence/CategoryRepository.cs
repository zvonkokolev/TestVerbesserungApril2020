using MovieManager.Core.Contracts;
using MovieManager.Core.Entities;
using System;
using System.Linq;

namespace MovieManager.Persistence
{
	internal class CategoryRepository : ICategoryRepository
	{
		private readonly ApplicationDbContext _dbContext;

		public CategoryRepository(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;
		}
		public void AddRange(Category[] categ)
		{
			_dbContext.Categories.AddRange(categ);
		}
		public Tuple<Category, int> GetCategoryWithMostMovies()
		{
			var a = _dbContext.Categories.Select(c => new
			{
				Category = new Category { CategoryName = c.CategoryName },
				Counter = c.Movies.Count
			})
				.OrderByDescending(c => c.Counter)
				.ThenByDescending(c => c.Category.CategoryName)
				.FirstOrDefault()
				;
			return Tuple.Create(a.Category, a.Counter);
		}
		public (string categor, int count, double durateSum)[] GetGroupMoviesByCategory() =>
			_dbContext.Categories
				.Select(s => new
				{
					Kategorie = s.CategoryName,
					Anzahl = s.Movies.Count(),
					Dauer = s.Movies.Sum(c => (double)c.Duration)
				})
				.OrderBy(s => s.Kategorie)
				.AsEnumerable()
				.Select(s => (s.Kategorie, s.Anzahl, (double)s.Dauer))
				.ToArray()
				;
		public (string categor, double avgSum)[] GetGroupMoviesByAvgDuration() => _dbContext.Categories
				.Select(s => new
				{
					Kategorie = s.CategoryName,
					Dauer = s.Movies.Average(c => (double)c.Duration)
				})
				.OrderByDescending(s => s.Dauer)
				.ThenBy(s => s.Kategorie)
				.AsEnumerable()
				.Select(s => (s.Kategorie, (double)s.Dauer))
				.ToArray()
				;
	}
}