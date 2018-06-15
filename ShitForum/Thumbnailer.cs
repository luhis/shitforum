using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Transforms;

namespace ShitForum
{
    public static class Thumbnailer
    {
        public static byte[] Make(byte[] input)
        {
            const int size = 150;

            using (var ms = new MemoryStream())
            {
                using (var image = Image.Load(input))
                {
                    image.Mutate(x => x
                        .Resize(size, size));
                    image.Save(ms, new JpegEncoder());
                }

                return ms.ToArray();
            }
        }
    }
}
