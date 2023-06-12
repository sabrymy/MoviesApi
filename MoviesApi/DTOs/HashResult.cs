using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.DTOs
{
    public class HashResult
{
        public string Hash { get; set; }
        public byte[] Salt { get; set; }
    }
}
