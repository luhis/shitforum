using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using ShitForum.Mappers;
using Microsoft.Extensions.DependencyInjection;
using ImageValidation;

namespace ShitForum.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ImageValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var validateImage = validationContext.GetService<IValidateImage>();
            var file = (IFormFile) value;
            
            var r = validateImage.ValidateAsync(UploadMapper.ExtractData(file)).Result;

            return r.IsT0 ? ValidationResult.Success : new ValidationResult(validateImage.MapToErrorString(r).ValueOr(string.Empty));
        }
    }
}
