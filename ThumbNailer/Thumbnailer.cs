using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Transforms;
using System;
using System.Collections.Generic;
using MediaToolkit;
using MediaToolkit.Options;
using MediaToolkit.Model;
using Microsoft.Extensions.Configuration;
using Optional;
using Optional.Unsafe;
using static ThumbNailer.FunctionalUsing;

namespace ThumbNailer
{
    public class Thumbnailer : IThumbNailer
    {
        private const int Size = 150;

        private readonly string ffmpegLocation;
        private readonly Func<string, byte[], byte[]> preThumbNailingProcess;

        public Thumbnailer(IConfiguration configuration)
        {
            this.ffmpegLocation = configuration.GetSection("FfmpegLocation").Get<string>();
            var caseOptions = new Dictionary<Option<string>, Func<byte[], byte[]>>()
            {
                {Option.Some(".webm"), b => GetVideoThumbNail(b, ".webm")},
                {Option.None<string>(), b => b},
            };
            this.preThumbNailingProcess = (z, data) => FunctionalCase.Exec(caseOptions, ItemCompare, z, data);
        }

        private static bool ItemCompare(Option<string> o, string val) =>
            o.HasValue && string.Equals(o.ValueOrFailure(), val, StringComparison.InvariantCultureIgnoreCase);

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

        private static T WithTempFile<T>(string extension, Func<string, T> f)
        {
            var tempInputName = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + extension);
            var r = f(tempInputName);
            File.Delete(tempInputName);
            return r;
        }

        byte[] IThumbNailer.Make(byte[] input, string extension) => MakeFromImage(preThumbNailingProcess(extension, input));

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

                        return File.ReadAllBytes(outputFileName);
                    })));
    }
}
