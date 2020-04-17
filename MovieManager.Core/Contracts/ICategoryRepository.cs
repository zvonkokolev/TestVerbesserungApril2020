using MovieManager.Core.Entities;
using System;

namespace MovieManager.Core.Contracts
{
	public interface ICategoryRepository
	{
		void AddRange(Category[] categories);
		Tuple<Category, int> GetCategoryWithMostMovies();
		(string categor, int count, double durateSum)[] GetGroupMoviesByCategory();
		(string categor, double avgSum)[] GetGroupMoviesByAvgDuration();
	}
}
