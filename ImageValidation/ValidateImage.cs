using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain.Repositories;
using Hashers;
using Microsoft.Extensions.Logging;
using Optional;

using ResType = OneOf.OneOf<ImageValidation.Pass, ImageValidation.SizeExceeded, ImageValidation.InvalidImage, ImageValidation.BannedImage>;

namespace ImageValidation
{
    public class ValidateImage : IValidateImage
    {
        private readonly IBannedImageRepository bannedImages;
        private readonly ILogger logger;

        public ValidateImage(IBannedImageRepository bannedImages, ILogger<ValidateImage> logger)
        {
            this.bannedImages = bannedImages;
            this.logger = logger;
        }

        private const int ImageMaxSize = 2 * 1024 * 1024;

        public static readonly string BannedImageString = "Banned image";

        Task<ResType> IValidateImage.ValidateAsync(Option<byte[]> data)
        {
            return data.Match(async some =>
            {
                if (some.Length > ImageMaxSize)
                {
                    return (ResType)new SizeExceeded(ImageMaxSize);
                }
                var hash = ImageHasher.Hash(some);
                if (await this.bannedImages.IsBanned(hash, CancellationToken.None))
                {
                    return new BannedImage();
                }

                return new Pass();
            }, () => Task.FromResult<ResType>(new Pass()));
        }

        IReadOnlyList<string> IValidateImage.AllowedExtensions() => new List<string>() { ".jpg", ".png", ".gif", ".webm" };
    }
}
