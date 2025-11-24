using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Projet_Dotnet_4.Models;
using Projet_Dotnet_4.Services;

namespace Projet_Dotnet_4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly IMovieService movieService;


        public MovieController(IMovieService _movieService) // DI
        {
            movieService = _movieService;
        }

        //[Authorize(Roles = "Admin")]
        [Authorize(Policy = "BossOnly")]

        [HttpGet("list")]
        public async Task<IActionResult> List()
        {
            try
            {
                var movies = await Task.Run(() => movieService.List());
                return Ok(movies);
            }
            catch (ArgumentException ex)
            {
                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, ex.Message);

            }
        }

        [Authorize(Roles = "Admin,Standard")]

        [HttpGet("get/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var movie = await Task.Run(() => movieService.Get(id));

                if (movie is null) return NotFound("Movie not found (bad id ?) ");

                return Ok(movie);

            }
            catch (ArgumentException ex)
            {
                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Route("create")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Movie newMovie)
        {
            try
            {
                var result = await Task.Run(() => movieService.Create(newMovie));
                return Ok(result);

            }
            catch (ArgumentException ex)
            {
                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] Movie newMovie)
        {
            try
            {
                var updatedMovie = await Task.Run(() => movieService.Update(newMovie));
                if (updatedMovie is null) return NotFound("Movie not found");

                return Ok(updatedMovie);

            }
            catch (ArgumentException ex)
            {
                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        [HttpGet("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await Task.Run(() => movieService.Delete(id));

                if (!result) return NotFound("Movie not found (bad id ?) " + id);

                return Ok(result);

            }
            catch (ArgumentException ex)
            {
                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
