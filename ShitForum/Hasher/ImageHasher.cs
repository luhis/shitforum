using Domain;
using EnsureThat;
using System;
using System.Security.Cryptography;

namespace ShitForum.Hasher
{
    public static class ImageHasher
    {
        private static readonly SHA1 sha1 = SHA1.Create();
        public static ImageHash Hash(byte[] bs)
        {
            EnsureArg.IsNotNull(bs, nameof(bs));
            
            return new ImageHash(Convert.ToBase64String(sha1.ComputeHash(bs)));
        }
    }
}
