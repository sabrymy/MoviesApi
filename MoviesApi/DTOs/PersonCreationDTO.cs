using Microsoft.AspNetCore.Http;
using MoviesApi.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.DTOs
{
    public class PersonCreationDTO :PersonPatchDTO
{
        
        [FileSizeValidation(5000)]
        [ContentTypeValidator(ContentTypeGroup.Image)]
        public IFormFile picture { get; set; }
    }
}
