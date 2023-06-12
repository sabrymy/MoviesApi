using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using MoviesApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.Controllers
{
    [ApiController]
    [Route("api/security")]
    [Consumes("application/json")]
    public class SecurityController : ControllerBase
    {
        private readonly IDataProtector _protector;
        private readonly HashService hashService;

        public SecurityController(IDataProtectionProvider protectionProvider, HashService hashService)
        {
            this._protector = protectionProvider.CreateProtector("value_secret_and_unique");
            this.hashService = hashService;
        }
        [HttpGet]
        public IActionResult  GetSec()
        {
            string plainText = "Mohamed Youssef";
            string encryptedText = _protector.Protect(plainText);
            string decreptedText = _protector.Unprotect(encryptedText);
            return Ok(new { orignalText = plainText, encrypted = encryptedText, decrepted = decreptedText });
        }

        [HttpGet("TimeBound")]
        public async Task<IActionResult> GetTimeBound()
        {
            var protectorTimeBound = _protector.ToTimeLimitedDataProtector();
            string plainText = "Mohamed Youssef";
            string encryptedText = protectorTimeBound.Protect(plainText,TimeSpan.FromSeconds(5.0));
            await Task.Delay(6000);
            string decreptedText = protectorTimeBound.Unprotect(encryptedText);
            return  Ok(new { orignalText = plainText, encrypted = encryptedText, decrepted = decreptedText });
        }
        [HttpGet("Hash")]
        public IActionResult GetHash()
        {
            var plainText = "Mohamed Youssef";
            var hashresult1 = hashService.Hash(plainText);
            var hashresult2 = hashService.Hash(plainText);
            return Ok(new { plainText, hashresult1, hashresult2 });
        }

    }
}
