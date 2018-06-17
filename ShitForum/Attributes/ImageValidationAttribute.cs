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
        private static ValidationResult VR(string s) => new ValidationResult(s);
   
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var validateImage = validationContext.GetService<IValidateImage>();
            var uploadMapper = validationContext.GetService<IUploadMapper>();
            var file = (IFormFile) value;
  
            var r = validateImage.ValidateAsync(uploadMapper.ExtractData(file)).Result;
            return r.Match(
                _ => ValidationResult.Success, 
                size => VR($"Image must not exceed {size.MaxSize} bytes"),
                invalid => VR("Invalid image format"), 
                banned => VR(ValidateImage.BannedImageString));
        }
    }
}
