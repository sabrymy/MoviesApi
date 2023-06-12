using MoviesApi.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.DTOs
{
    public class MovieDTO
{
        public int Id { get; set; }
        
        public string Title { get; set; }
        public string Summary { get; set; }
        public bool InTheaters { get; set; }
        public DateTime ReleaseDate { get; set; }
        [FileSizeValidation(5000)]
        [ContentTypeValidator(ContentTypeGroup.Image)]
        public string Poster { get; set; }
    }
}
