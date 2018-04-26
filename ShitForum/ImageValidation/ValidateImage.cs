using System;
using System.Net;
using System.Threading.Tasks;
using Domain;
using Domain.IpHash;
using Domain.Repositories;
using Microsoft.Extensions.Logging;
using Optional;
using ShitForum.Hasher;
using SixLabors.ImageSharp;

using ResType = OneOf.OneOf<ShitForum.ImageValidation.Pass, ShitForum.ImageValidation.SizeExceeded, ShitForum.ImageValidation.InvalidImage, ShitForum.ImageValidation.BannedImage>;

namespace ShitForum.ImageValidation
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

        public async Task<ResType> ValidateAsync(IPAddress ip, IpHash ipHash, byte[] input)
        {
            if (input.Length > ImageMaxSize)
            {
                return new SizeExceeded(ImageMaxSize);
            }
            var hash = ImageHasher.Hash(input);
            if (await this.bannedImages.IsBanned(hash))
            {
                this.logger.LogInformation($"User ip:{ip.ToString()} hash:{ipHash.Val} attempted to upload banned image hash:{hash}");
                return new BannedImage();
            }

            try
            {
                Image.Load(input);
            }
            catch (Exception)
            {
                return new InvalidImage();
            }

            return new Pass();
        }

        public static readonly string BannedImageString = "Banned image";

        Option<string> IValidateImage.MapToErrorString(ResType r) =>
            r.Match(
                _ => Option.None<string>(),
                s => Option.Some<string>($"Image must not exceed {s.MaxSize} bytes"),
                _ => Option.Some<string>("Invalid image format"),
                _ => Option.Some<string>(BannedImageString));

        ////private readonly Action doNothing = () => { };
        ////Task IValidateImage.ValidateAsync(Option<byte[]> data, IPAddress ip, IpHash hash, Action<string> addError)
        ////{
        ////    return data.Match(async some =>
        ////    {
        ////        var imageValidationResult = await this.ValidateAsync(ip, hash, some);
        ////        MapToErrorString(imageValidationResult).Match(addError, doNothing);
        ////    }, () => Task.CompletedTask);
        ////}


        Task<ResType> IValidateImage.ValidateAsync(Option<byte[]> data)
        {
            return data.Match(async some =>
            {
                if (some.Length > ImageMaxSize)
                {
                    return (ResType)new SizeExceeded(ImageMaxSize);
                }
                var hash = ImageHasher.Hash(some);
                if (await this.bannedImages.IsBanned(hash))
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
    }
}