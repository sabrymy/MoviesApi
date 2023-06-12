using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using MoviesApi.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace MoviesApi.Services
{
    public class HashService
{
        public HashResult Hash(string input)
        {
            // Generates a random salt 
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            return Hash(input, salt);
        }

        public HashResult Hash(string input, byte[] salt)
        {
            // derives a 256 bits subkey (uses HMACSHA1 with 10,000 iterations)=256 byte *8 
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
          password: input,
          salt: salt,
          prf: KeyDerivationPrf.HMACSHA1,
          iterationCount: 10000,
          numBytesRequested: 256 / 8));

            return new HashResult()
            {
                Hash = hashed,
                Salt = salt
            };
        }
    }
}
