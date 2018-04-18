using System;
using System.Security.Cryptography;

namespace ShitForum.Hasher
{
    public static class Sha256Hasher
    {
        private static byte[] ToBytes(string s) => System.Text.Encoding.ASCII.GetBytes(s);

        public static string Hash(string s)
        {
            var hash = SHA256.Create();
            return Convert.ToBase64String(hash.ComputeHash(ToBytes(s)));
        }
    }
}