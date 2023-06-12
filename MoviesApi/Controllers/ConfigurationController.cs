using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.Controllers
{
    [ApiController]
    [Consumes("application/json")]
    [Route("api/configuration")]
    public class ConfigurationController : ControllerBase
    {
        private readonly IConfiguration configuration;
        public ConfigurationController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        [HttpGet]
        public IActionResult GetConfig()
        {
            //lastname is property in appsetting.json
            return Ok(configuration["lastname"]);
            //return default connection property
            //[sectionname:property name]
            //  return Ok(configuration["ConnectionStrings:DefaultConnection"]);
        }
    }
}
