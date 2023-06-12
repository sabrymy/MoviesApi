using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.Validation
{
    public class FileSizeValidation : ValidationAttribute
    {
        private readonly int  maxfileSizeInkbs ;
        public FileSizeValidation( int MaxFileSizeInkbs )
        {
            maxfileSizeInkbs = MaxFileSizeInkbs;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
           if ( value == null )
            {
                return ValidationResult.Success;
            }
                               // value as IFormFile
            IFormFile formFile = (IFormFile) value;

            if (formFile == null)
            {
                return ValidationResult.Success;
            }

            if (formFile.Length > maxfileSizeInkbs*1024)
            {
                return new ValidationResult($"Size of file is large greater than {maxfileSizeInkbs  } kb ");
            }
            return ValidationResult.Success;


        }

    }
}
