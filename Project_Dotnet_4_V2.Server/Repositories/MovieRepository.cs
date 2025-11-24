using Projet_Dotnet_4.Models;

namespace Projet_Dotnet_4.Repositories
{
    public class MovieRepository
    {
        public static List<Movie> Movies = new()
        {
            new() { Id = 1, Title = "Matrix", Description = "Film de science-fiction1 de type « cyberfilm » australo-américain écrit et réalisé par les Wachowski et sorti en 1999. ", Rating = 8.7 },
            new() { Id = 2, Title = "Avatar", Description = "Film de science-fiction américain réalisé par James Cameron, sorti en 2009. Il s'agit du premier film de la franchise cinématographique Avatar.", Rating = 7.8 },
            new() { Id = 3, Title = "Harry Potter", Description = "Série littéraire de low fantasy écrite par l'auteure britannique J. K. Rowling, dont la suite romanesque s'est achevée en 2007. Une pièce de théâtre, considérée comme la « huitième histoire » officielle, a été jouée et publiée en 2016", Rating = 7.6 },
            new() { Id = 4, Title = "Terminator 2", Description = "Film américain de science-fiction réalisé par James Cameron, sorti en 1991", Rating = 8.5 },
            new() { Id = 5, Title = "Alien", Description = "Film de science-fiction horrifique américain réalisé par Ridley Scott, écrit par Dan O'Bannon et sorti en 1979. Il fait partie de la saga Alien, dont il constitue l'opus initial. ", Rating = 8.4 },
        };
    }
}
