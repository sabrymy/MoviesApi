using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.Validation
{
    public class FirstLetterUpperCaseAttribute :ValidationAttribute
    {

     protected  bool   reqValidation()
        {
            bool reqValidation = RequiresValidationContext;
            return reqValidation;
    }





        protected   override  ValidationResult   IsValid( object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()) )
            {

                return ValidationResult.Success;
            }

            var firstLetter = value.ToString()[0].ToString();
            if (firstLetter != firstLetter.ToUpper())
            {
                return new ValidationResult("The first letter should be Capital");
            }
            return ValidationResult.Success;
        }
    }
}
