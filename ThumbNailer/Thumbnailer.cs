using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Transforms;
using System;
using MediaToolkit;
using MediaToolkit.Options;
using MediaToolkit.Model;
using Microsoft.Extensions.Configuration;
using static ThumbNailer.StaticUsing;

namespace ThumbNailer
{
    public class Thumbnailer : IThumbNailer
    {
        private const int Size = 150;

        private readonly string ffmpegLocation;

        public Thumbnailer(IConfiguration configurtion)
        {
            this.ffmpegLocation = configurtion.GetSection("FfmpegLocation").Get<string>();
        }

        private static byte[] MakeFromImage(byte[] input) =>
            Using(() => new MemoryStream(), ms =>
            {
                Using(() => Image.Load(input), image =>
                {
                    image.Mutate(x => x
                        .Resize(Size, Size));
                    image.Save(ms, new JpegEncoder());
                });

                return ms.ToArray();
        });

        bool IThumbNailer.IsSettingValid() => File.Exists(this.ffmpegLocation);

        private T WithTempFile<T>(string extension, Func<string, T> f)
        {
            var tempInputName = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + extension);
            var r = f(tempInputName);
            File.Delete(tempInputName);
            return r;
        }

        byte[] IThumbNailer.Make(byte[] input, string extension)
        {
            if (string.Equals(extension, ".webm", StringComparison.InvariantCultureIgnoreCase))
            {
                return MakeFromImage(GetVideoThumbNail(input, extension));
            }
            else
            {
                return MakeFromImage(input);
            }
        }

        private byte[] GetVideoThumbNail(byte[] input, string extension) =>
            WithTempFile(extension, inputFileName =>
                WithTempFile(".jpg", outputFileName =>
                    Using(() => new Engine(this.ffmpegLocation), engine =>
                    {
                        File.WriteAllBytes(inputFileName, input);
                        var inputFile = new MediaFile(inputFileName);

                        var options = new ConversionOptions {Seek = TimeSpan.FromSeconds(0)};
                        var outputFile = new MediaFile(outputFileName);
                        engine.GetThumbnail(inputFile, outputFile, options);

                        var r = File.ReadAllBytes(outputFileName);
                        return r;
                    })));
    }
}
