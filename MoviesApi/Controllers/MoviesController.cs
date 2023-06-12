using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.DTOs;
using MoviesApi.Entitities;
using MoviesApi.Helper;
using MoviesApi.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors;

namespace MoviesApi.Controllers
{


    [ApiController]
    [Consumes("application/json")]
    [Route("api/movies")]
    [EnableCors(PolicyName = "AllowPIRequestIO")]
    public class MoviesController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly ILogger<MoviesController> logger ;
        private readonly IFileStorageService fileStorageService;
        private readonly string containerName = "movies";
        public MoviesController(ApplicationDbContext context, IMapper mapper, IFileStorageService fileStorageService, ILogger<MoviesController> logger)
        {
            this.context = context;
            this.mapper = mapper;
            this.fileStorageService = fileStorageService;
            this.logger = logger;
        }
        [HttpGet]
        public async Task<ActionResult<List<MovieDTO>>> GetMovie()
        {
            var movies = await context.Movies.ToListAsync();
            return mapper.Map<List<MovieDTO>>(movies);
        }
        [HttpGet("filterData")]
        public async Task<ActionResult<IndexMoviePageDTO>> GetFilter()
        {
            var top = 6;
            var today = DateTime.Today;
            var upcomingReleases = await context.Movies
                .Where(x => x.ReleaseDate > today)
                .OrderBy(x => x.ReleaseDate)
                .Take(top)
                .ToListAsync();

            var inTheaters = await context.Movies
                .Where(x => x.InTheaters)
                .Take(top)
                .ToListAsync();

            var result = new IndexMoviePageDTO();

            result.InTheaters = mapper.Map<List<MovieDTO>>(inTheaters);

            result.UpcomingReleases = mapper.Map<List<MovieDTO>>(upcomingReleases);
            var movies = await context.Movies.ToListAsync();
            return result;
        }

        [HttpGet("filter")]
        public async Task<ActionResult<List<MovieDTO>>> Filter([FromQuery] FilterMoviesDTO  filterMoviesDTO ) 
            {
            var moviesQueryable = context.Movies.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filterMoviesDTO.Title))
            {
                moviesQueryable = moviesQueryable.Where(x => x.Title.Contains(filterMoviesDTO.Title));
            }

            if (filterMoviesDTO.InTheaters)
            {
                moviesQueryable = moviesQueryable.Where(x => x.InTheaters);
            }

            if (filterMoviesDTO.UpcomingReleases)
            {
                var today = DateTime.Today;
                moviesQueryable = moviesQueryable.Where(x => x.ReleaseDate > today);
            }

            if (filterMoviesDTO.GenreId != 0)
            {
                moviesQueryable = moviesQueryable
                    .Where(x => x.MoviesGenres.Select(y => y.GenreId)
                    .Contains(filterMoviesDTO.GenreId));
            }

           if (!string.IsNullOrWhiteSpace(filterMoviesDTO.OrderingField))
            {

                try
                {
                    moviesQueryable = moviesQueryable
                        .OrderBy($"{filterMoviesDTO.OrderingField} {(filterMoviesDTO.AscendingOrder ? "ascending" : "descending")}");
                }
                catch
                {
                    // log this
                    logger.LogWarning("Could not order by field: " + filterMoviesDTO.OrderingField);
                }
            }


            await HttpContext.InsertPaginationParametersInResponse(moviesQueryable,
                filterMoviesDTO.RecordsPerPage);

            
            var movies = await moviesQueryable.Paginate(filterMoviesDTO.Pagination).ToListAsync();

            return mapper.Map<List<MovieDTO>>(movies);


        }



    [HttpGet("{id}",Name ="GetMovie")]
        public async Task<ActionResult<MoviesDetailsDTO>> GetMovieById(int id)
        {
            var movie = await context.Movies.Include(x=> x.MoviesActors).ThenInclude(x=>x.Person)
                .Include(x => x.MoviesGenres).ThenInclude(x => x.Genre).
                FirstOrDefaultAsync(x => x.Id == id);
            if (movie == null)
            {
                return NotFound();
            }
            return mapper.Map<MoviesDetailsDTO>(movie);

        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Post([FromForm] MovieCreationDTO movieCreationDTO)
        {
            var movie = mapper.Map<Movie>(movieCreationDTO);
           
            if (movieCreationDTO.Poster != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await movieCreationDTO.Poster.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = movieCreationDTO.Poster.FileName.Split(".")[1];
                    string container = containerName;
                    movie.Poster = await fileStorageService.SaveFile(content, extension, container,
                        movieCreationDTO.Poster.ContentType);
                   
                }
            }

            AnnotateActorsOrder(movie);
            context.Add(movie);
            await context.SaveChangesAsync();
            var moviecrDTO = mapper.Map<MovieDTO>(movie);
            return new CreatedAtRouteResult("GetMovie", new { Id = movie.Id }, moviecrDTO);

        }

        //
        private static void AnnotateActorsOrder(Movie movie)
        {
            if (movie.MoviesActors != null)
            {
                for (int i = 0; i < movie.MoviesActors.Count; i++)
                {
                    movie.MoviesActors[i].Order = i;
                }
            }
        }




        [HttpPut("{id}")]
        [DisableCors]
        public async Task<ActionResult> Put(int id, [FromForm] MovieCreationDTO movieCreationDTO)
        {
            var movieDB = await context.Movies.FirstOrDefaultAsync(x => x.Id == id);

            if (movieDB == null)
            {
                return NotFound();
            }

            movieDB = mapper.Map(movieCreationDTO, movieDB);

            if (movieCreationDTO.Poster != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await movieCreationDTO.Poster.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(movieCreationDTO.Poster.FileName);
                    movieDB.Poster =
                        await fileStorageService.EditFile(content, extension, containerName,
                                                            movieDB.Poster,
                                                            movieCreationDTO.Poster.ContentType);
                }
            }


            await context.Database.ExecuteSqlInterpolatedAsync($"delete from MoviesActors where MovieId = {movieDB.Id}; delete from MoviesGenres where MovieId = {movieDB.Id}");

            AnnotateActorsOrder(movieDB);


            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [DisableCors]
        public async Task<ActionResult> Delete(int id)
        {
            var exists = await context.Movies.AnyAsync(x => x.Id == id);

            if (!exists)
            {
                return NotFound();
            }

            context.Remove(new Movie() { Id = id });
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<MoviePatchDTO> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var entityFromDB = await context.Movies.FirstOrDefaultAsync(x => x.Id == id);

            if (entityFromDB == null)
            {
                return NotFound();
            }

            var entityDTO = mapper.Map<MoviePatchDTO>(entityFromDB);

            patchDocument.ApplyTo(entityDTO, ModelState);

            var isValid = TryValidateModel(entityDTO);

            if (!isValid)
            {
                return BadRequest(ModelState);
            }

            mapper.Map(entityDTO, entityFromDB);
            
            await context.SaveChangesAsync();

            return NoContent();
        }















    }
}
