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

namespace MoviesApi.Controllers
{
    [ApiController]
    [Consumes("application/json")]
    [Route("api/People")]
    public class PeopleController : ControllerBase
    {
        private readonly ApplicationDbContext context ;
        private readonly IMapper mapper;
        private readonly IFileStorageService fileStorageService;
        private readonly string containerName = "people";

        public PeopleController(ApplicationDbContext context, IMapper mapper, IFileStorageService fileStorageService)
        {
            this.context = context;
            this.mapper = mapper;
            this.fileStorageService = fileStorageService;

        }
        [HttpGet]
         public async Task<ActionResult<List<PersonDTO>>> GetPeople()
        {
            var people = await context.People.ToListAsync();
            return  mapper.Map <List<PersonDTO>>(people);
        }

        //pagination represent the page number of data we require to get i.e. which page 
        [HttpGet("Pagination")]
        public async Task<ActionResult<List<PersonDTO>>> GetPagination([FromQuery] PaginationDTO  pagination)
        {
            var queryable = context.People.AsQueryable();
            
            await HttpContext.InsertPaginationParametersInResponse(queryable, pagination.RecordsPerPage);
            var people = await queryable.Paginate(pagination).ToListAsync();
            return mapper.Map<List<PersonDTO>>(people);
        }




        [HttpGet("{Id}",Name ="getPerson")]
        public async Task<ActionResult<PersonDTO>> GetPeople(int id)
        {
            var person = await context.People.FirstOrDefaultAsync(x => x.Id == id);
            if (person == null)
                return NotFound();
          return   mapper.Map<PersonDTO>(person);
        }
        [HttpPost]
        public async Task<ActionResult> Post([FromForm] PersonCreationDTO personCreationDTO)
        {
            var person =  mapper.Map<Person>(personCreationDTO);
            if (personCreationDTO.picture != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await personCreationDTO.picture.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = personCreationDTO.picture.FileName.Split(".")[1];
                    string container = containerName;
                    person.Picture = await fileStorageService.SaveFile(content, extension, container,
                        personCreationDTO.picture.ContentType);
                }
            }


            context.Add(person);
            await  context.SaveChangesAsync();
            var personDTO = mapper.Map<PersonDTO>(person);
            return new CreatedAtRouteResult("getPerson", new {Id= person.Id }, personDTO);

        }
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id,  [FromForm] PersonCreationDTO personCreationDTO)
        {
            var personDB = await context.People.FirstOrDefaultAsync(x => x.Id == id);

            if (personDB == null) { return NotFound(); }

            personDB = mapper.Map(personCreationDTO, personDB);
            if (personCreationDTO.picture != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await personCreationDTO.picture.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(personCreationDTO.picture.FileName);
                    personDB.Picture =
                        await fileStorageService.EditFile(content, extension, containerName,
                                                            personDB.Picture,
                                                            personCreationDTO.picture.ContentType);
                }
            }

            await context.SaveChangesAsync();
            return NoContent();

        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exists = await context.People.AnyAsync(x => x.Id == id);
            if (!exists)
            {
                return NotFound();
            }

            context.Remove(new Person() { Id = id });
            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(int id,[FromBody] JsonPatchDocument<PersonPatchDTO>  patchDocument)
        {
              if  ( patchDocument == null)
            {
                return BadRequest();
            }
              var entityFromDB  = await context.People.FirstOrDefaultAsync(x=> x.Id == id) ;
            if (entityFromDB == null) return NotFound();
            var personDTO = mapper.Map<PersonPatchDTO>(entityFromDB);
           
            //apply patch json document operation on personDTO and log errors to model state dictionary
            patchDocument.ApplyTo(personDTO,ModelState);
            var valid = TryValidateModel(personDTO);
            if (!valid)
            {
                return BadRequest();
            }
         // var requiredEntity =   mapper.Map(personDTO, typeof(PersonPatchDTO), typeof(Person));
            var requiredEntity = mapper.Map(personDTO, entityFromDB);
          await  context.SaveChangesAsync();
            return NoContent();
        }




    }
}
