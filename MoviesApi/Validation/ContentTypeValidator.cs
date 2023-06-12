using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.Validation
{
    public class ContentTypeValidator : ValidationAttribute
    {
        private readonly String[] validContentTypes;
        private readonly string[] imageContentTypes = new string[] { "image/jpeg", "image/png", "image/gif" };




        public ContentTypeValidator(ContentTypeGroup ContentTypeGroup)
        {
            switch (ContentTypeGroup)
            {
                case ContentTypeGroup.Image:
                    validContentTypes = imageContentTypes;
                    break;

            }

        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }
            // value as IFormFile
            IFormFile formFile = (IFormFile)value;

            if (formFile == null)
            {
                return ValidationResult.Success;
            }
            if (!validContentTypes.Contains(formFile.ContentType))
            {
                return new ValidationResult($"ContentType should be on of the following : {string.Join(",", validContentTypes)  } ");

            }
            return ValidationResult.Success;
        }



    }

    public enum ContentTypeGroup
    {
        Image
    }
}