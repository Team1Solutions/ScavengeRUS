using ScavengeRUs.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace ScavengeRUs.Attributes
{
    /// <summary>
    /// custom attribute to mark properties as subject to validation
    /// </summary>
    public class EndDateDateValidationAttribute : ValidationAttribute
    {
        /// <summary>
        /// implements the validation logic for this attribute
        /// </summary>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (Hunt)validationContext.ObjectInstance;
            if (model.EndDate < model.StartDate)
                return new ValidationResult(ErrorMessage);

            return ValidationResult.Success;
        }
    }
}
