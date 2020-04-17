using MovieManager.Core.Contracts;
using MovieManager.Core.Entities;
using System.Linq;

namespace MovieManager.Persistence
{
	public class MovieRepository : IMovieRepository
	{
		private readonly ApplicationDbContext _dbContext;

		public MovieRepository(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;
		}
		public void AddRange(Movie[] movies)
		{
			_dbContext.Movies.AddRange(movies);
		}

		public Movie GetMoviesByDuration() => _dbContext.Movies
																.OrderByDescending(d => d.Duration)
																.ThenBy(d => d.Title)
																.Select(d => d)
																.FirstOrDefault();
		public int GetCategoryWithYear()
		{
			var a = _dbContext.Movies
				.Where(m => m.Category.CategoryName.Equals("Action"))
				.OrderBy(m => m.Year)
				.Select(s => s.Year)
				.ToArray();
			int c = 0, counter = 1, Y = 0;
			for (int i = 0; i < a.Length - 1; i++)
			{
				while(a[i] == a[i + 1])
				{
					i++;
					if (counter > c)
					{
						c = counter;
						Y = a[i];
					}
					counter++;
				}
				counter = 0;
			}
			return Y;
		}
	}
}