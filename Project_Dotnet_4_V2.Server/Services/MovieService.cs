using Projet_Dotnet_4.Models;
using Projet_Dotnet_4.Repositories;

namespace Projet_Dotnet_4.Services
{
    public class MovieService : IMovieService
    {
        public Movie? Create(Movie movie)
        {
            movie.Id = MovieRepository.Movies.Max( m => m.Id ) + 1; 
            MovieRepository.Movies.Add( movie );
            return movie;

        }

        public bool Delete(int id)
        {
            var movieToDelete = Get(id);

            if (movieToDelete is null) return false;

            MovieRepository.Movies.Remove( movieToDelete );

            return true;
        }

        public Movie? Get(int id)
        {
            var movie = MovieRepository.Movies.FirstOrDefault(x => x.Id == id);

            if(movie is null) return null;

            return movie;
        }

        public List<Movie> List()
        {
            return MovieRepository.Movies;
        }

        public Movie? Update(Movie newMovie)
        {
            var oldMovie = MovieRepository.Movies.FirstOrDefault(x => x.Id == newMovie.Id);

            if (oldMovie is null) return null;

            oldMovie.Title = newMovie.Title;
            oldMovie.Description = newMovie.Description;
            oldMovie.Rating = newMovie.Rating;

            return oldMovie;
        }
    }
}
