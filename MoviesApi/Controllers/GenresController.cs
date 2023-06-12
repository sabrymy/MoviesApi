using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using MoviesApi.Entitities;
using System.Collections.Generic;
using System.Threading.Tasks;

using AutoMapper;

using Microsoft.EntityFrameworkCore;
using MoviesApi.DTOs;
using Microsoft.AspNetCore.Server.HttpSys;
using System.Net;
using Microsoft.AspNetCore.Cors;

namespace MoviesApi.Controllers
{
    //url end point start by convention with api
    //endUrl BaseUrl/api/Genres   where /api/Genres is the endpoint baseUrl https://localhost:44377/ OR webserver, Genres is the controller
    [Route("api/Genres")]
    //equavelent to [Route("api/Controller")] which extract the controller part Genres
    // [Route("api/Controller")]
    [ApiController]
    [Consumes("application/json")]
    public class GenresController : ControllerBase
    {
        
       
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        //IMapper for Mapper service
        public GenresController( ApplicationDbContext context, IMapper mapper )
        {
            
            this.context = context;
            this.mapper = mapper;
            
        }

        //[HttpGet("list] api/Genres/list
        // [ResponseCache(Duration =60)]
        //   [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpGet]
        [EnableCors(PolicyName = "AllowPIRequestIO")]
          public async Task<List<GenreDTO>> GetGenre()
        {
           //Genres Actual Table Name
            var genres = await context.Genres.AsNoTracking().ToListAsync();
            var genresDTOs = mapper.Map<List<GenreDTO>>(genres);
        //    return await context.Genres.AsNoTracking().ToListAsync();
            return genresDTOs;
                }
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,  Roles ="Admin")]
        public async Task<ActionResult> PostAsync([FromBody] GenreCreationDTO genreCreation)

        {
            //ModelState used in validation if [ApiController] not specified for controller class
            // if (!ModelState.IsValid)
            //  {
            //    return BadRequest(ModelState);
            // }

            if (genreCreation == null)
            {
               
                return NoContent();
            }
            else
            {
                // repository.AddGenre(genre);
                var genre = mapper.Map<Genre>(genreCreation);
                context.Add(genre);
                await   context.SaveChangesAsync();
                var genreDTO = mapper.Map<GenreDTO>(genre);
             return new CreatedAtRouteResult("getGenre", new {Id =  genreDTO.Id, Status = 2 }, genreDTO);

            }
        }
        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> PutAsync([FromBody] GenreCreationDTO genreCreation, int id)

        {
            var genre = mapper.Map<Genre>(genreCreation);
            genre.Id = id;
            //mark that some or all properties of entity have been changed
            context.Entry(genre).State = EntityState.Modified;
            //commit changes
            await context.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            var exist = await context.Genres.AnyAsync(x => x.Id == id);
            if (!exist )
            {
                return NotFound();
            }
            context.Remove(new Genre { Id = id });
            await context.SaveChangesAsync();
            return NoContent();
        }



        //https:localhost:port/api/Genres/int-value1/int-value2

        [HttpGet("{Id:int}", Name = "getGenre")]
        public async Task<ActionResult<GenreDTO>> GetGenreByIdAsync([BindRequired] int Id, [BindNever][FromHeader]  string Status)
        {
            //ModelState used in validation if [ApiController] not specified for controller class
            //   if (!ModelState.IsValid)
            //   {
            //      return BadRequest(ModelState);
            //  }

            //  var genre = repository.GetGenreById(Id);
            var genre = await context.Genres.FirstOrDefaultAsync(x => x.Id == Id);

            if ( genre == null )
            {
               
                return NotFound();
            }
            var genreDTO = mapper.Map<GenreDTO>(genre);

            return Ok(genreDTO);
        }



    }
}
