using MoviesApi.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi
{
    public class GenreCreationDTO
{
        [Required]
        [StringLength(40)]
        [FirstLetterUpperCase]
        public string Name { get; set; }
    }
}
