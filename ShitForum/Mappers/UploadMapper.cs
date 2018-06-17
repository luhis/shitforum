using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using Optional;
using ThumbNailer;
using File = Domain.File;

namespace ShitForum.Mappers
{
    public class UploadMapper : IUploadMapper
    {
        private readonly IThumbNailer thumbNailer;

        public UploadMapper(IThumbNailer thumbNailer)
        {
            this.thumbNailer = thumbNailer;
        }

        private static byte[] ExtractStream(Stream s)
        {
            using (var memoryStream = new MemoryStream())
            {
                s.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        Option<File> IUploadMapper.Map(IFormFile f, Guid postId)
        {
            if (f == null)
            {
                return Option.None<File>();
            }

            string mime = MimeTypes.GetMimeType(f.FileName);
            var imageData = ExtractStream(f.OpenReadStream());
            var thumbNail = thumbNailer.Make(imageData, Path.GetExtension(f.FileName));
            return Option.Some(new File(postId, f.FileName, thumbNail, imageData, mime));
        }

        Option<byte[]> IUploadMapper.ExtractData(IFormFile f) => f == null ? Option.None<byte[]>() : Option.Some(ExtractStream(f.OpenReadStream()));
    }
}
