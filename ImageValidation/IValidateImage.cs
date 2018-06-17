using Optional;
using System.Collections.Generic;
using System.Threading.Tasks;

using ResType = OneOf.OneOf<ImageValidation.Pass, ImageValidation.SizeExceeded, ImageValidation.InvalidImage, ImageValidation.BannedImage>;

namespace ImageValidation
{
    public interface IValidateImage
    {
        Task<ResType> ValidateAsync(Option<byte[]> data);
        IReadOnlyList<string> AllowedExtensions();
    }
}
