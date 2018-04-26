using System.Linq;
using System.Net;
using Domain.IpHash;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using ShitForum.ImageValidation;

namespace ShitForum.BannedImageLogger
{
    public class BannedImageLogger : IBannedImageLogger
    {
        private readonly ILogger logger;

        public BannedImageLogger(ILogger<BannedImageLogger> logger)
        {
            this.logger = logger;
        }

        void IBannedImageLogger.Log(ModelStateEntry modelStateEntry, IPAddress ip, IIpHash ipHash)
        {
            if (modelStateEntry != null)
            {
                var hasBannedImage = modelStateEntry.Errors.Select(a => a.ErrorMessage).Contains(ValidateImage.BannedImageString);
                if (hasBannedImage)
                {
                    this.logger.LogInformation($"User ip:{ip.ToString()} hash:{ipHash.Val} attempted to upload banned image.");
                }
            }
        }
    }
}
