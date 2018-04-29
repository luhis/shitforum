using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using Optional;
using File = Domain.File;

namespace ShitForum.Mappers
{
    public static class UploadMapper
    {
        private static byte[] ExtractStream(Stream s)
        {
            using (var memoryStream = new MemoryStream())
            {
                s.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public static Option<File> Map(IFormFile f, Guid postId)
        {
            if (f == null)
            {
                return Option.None<File>();
            }

            string mime = MimeTypes.GetMimeType(f.FileName);
            var imageData = ExtractStream(f.OpenReadStream());
            var thumbNail = Thumbnailer.Make(imageData);
            return Option.Some(new File(postId, f.FileName, thumbNail, imageData, mime));
        }

        public static Option<byte[]> ExtractData(IFormFile f) => f == null ? Option.None<byte[]>() : Option.Some(ExtractStream(f.OpenReadStream()));
    }
}
