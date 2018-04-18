using EnsureThat;
using System;

namespace Domain
{
    public sealed class File : DomainBase
    {
        public File(Guid id, Guid postId, string fileName, byte[] thumbNailJpeg, byte[] data, string mimeType) : base(id)
        {
            PostId = EnsureArg.IsNotNull(postId, nameof(postId));
            FileName = EnsureArg.IsNotNullOrWhiteSpace(fileName, nameof(fileName));
            ThumbNailJpeg = EnsureArg.IsNotNull(thumbNailJpeg, nameof(thumbNailJpeg));
            Data = EnsureArg.IsNotNull(data, nameof(data));
            MimeType = EnsureArg.IsNotNullOrWhiteSpace(mimeType, nameof(mimeType));
        }

        public File()
        {
        }

        public Guid PostId { get; private set; }

        public string FileName { get; private set; }

        public byte[] ThumbNailJpeg { get; private set; }

        public byte[] Data { get; private set; }

        public string MimeType { get; private set; }
    }
}