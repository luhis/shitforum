using Optional;
using System.Threading.Tasks;

using ResType = OneOf.OneOf<ShitForum.ImageValidation.Pass, ShitForum.ImageValidation.SizeExceeded, ShitForum.ImageValidation.InvalidImage, ShitForum.ImageValidation.BannedImage>;

namespace ShitForum.ImageValidation
{
    public interface IValidateImage
    {
        Task<ResType> ValidateAsync(Option<byte[]> data);
        Option<string> MapToErrorString(ResType r);
    }
}