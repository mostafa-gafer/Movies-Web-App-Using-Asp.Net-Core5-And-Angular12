using System.ComponentModel.DataAnnotations;
namespace Movies.CORE.Validations
{
    public class FirstLetterUpperCaseAttribute :ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //to check if letter uppercase or not
            if (value == null || string.IsNullOrEmpty(value.ToString()))
                return ValidationResult.Success;
            
            var firstLetter = value.ToString()[0].ToString();

            if(firstLetter != firstLetter.ToUpper())
                return new ValidationResult("First Letter Should be UpperCase");


            return ValidationResult.Success;
        }
    }
}
