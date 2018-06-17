using System;
using Domain;
using Microsoft.AspNetCore.Http;
using Optional;

namespace ShitForum.Mappers
{
    public interface IUploadMapper
    {
        Option<File> Map(IFormFile f, Guid postId);
        Option<byte[]> ExtractData(IFormFile f);
    }
}
