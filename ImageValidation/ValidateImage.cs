using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain.Repositories;
using Hashers;
using Microsoft.Extensions.Logging;
using Optional;
using SixLabors.ImageSharp;

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

        Option<string> IValidateImage.MapToErrorString(ResType r) =>
            r.Match(
                _ => Option.None<string>(),
                s => Option.Some<string>($"Image must not exceed {s.MaxSize} bytes"),
                _ => Option.Some<string>("Invalid image format"),
                _ => Option.Some<string>(BannedImageString));

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

                try
                {
                    Image.Load(some);
                }
                catch (Exception)
                {
                    return new InvalidImage();
                }

                return new Pass();
            }, () => Task.FromResult<ResType>(new Pass()));
        }

        IEnumerable<string> IValidateImage.AllowedExtensions() => new List<string>() { ".jpg", ".png", ".gif" };
    }
}
