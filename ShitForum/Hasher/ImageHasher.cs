using Domain;
using EnsureThat;
using System;
using System.Security.Cryptography;

namespace ShitForum.Hasher
{
    public static class ImageHasher
    {
        private static readonly MD5 md5 = MD5.Create();
        public static ImageHash Hash(byte[] bs)
        {
            EnsureArg.IsNotNull(bs, nameof(bs));
            
            return new ImageHash(Convert.ToBase64String(md5.ComputeHash(bs)));
        }
    }
}
