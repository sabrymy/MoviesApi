using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.DTOs
{
    public class IndexMoviePageDTO
{
        public List<MovieDTO> UpcomingReleases { get; set; }
        public List<MovieDTO> InTheaters { get; set; }
    }
}
