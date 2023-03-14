using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace JP.DataHub.Com.HashAlgorithm
{
    public enum HashAlgorithmType
    {
        Sha256,
        Sha384,
        Sha512,
    }


    public class HashCalculation
    {
        private const int BUFFERSIZE = 4096; // Stream Hashingのバッファサイズ

        public static byte[] ComputeHash(byte[] value, HashAlgorithmType hashAlgorithm = HashAlgorithmType.Sha384)
        {
            using (var hashCalc = CreateHashAlgorithm(hashAlgorithm))
            {
                return hashCalc.ComputeHash(value);
            }

        }

        public static byte[] ComputeHash(Stream value, HashAlgorithmType hashAlgorithm = HashAlgorithmType.Sha384)
        {
            if (!value.CanRead || !value.CanSeek) { throw new InvalidOperationException("Unreadable or unseekable stream"); }
            if (value.Length <= value.Position) { throw new InvalidOperationException("stream has reached end position."); }

            using (var hashCalc = CreateHashAlgorithm(hashAlgorithm))
            {
                var buf = new byte[BUFFERSIZE];
                while (value.Length > value.Position)
                {
                    value.Read(buf, 0, buf.Length);
                    hashCalc.TransformBlock(buf, 0, buf.Length, null, 0);
                }
                hashCalc.TransformFinalBlock(buf, 0, 0);
                return hashCalc.Hash;
            }
        }

        public static string ComputeHashString(byte[] value, HashAlgorithmType hashAlgorithm = HashAlgorithmType.Sha384)
        {
            var hashStr = new StringBuilder();
            foreach (var hashByte in ComputeHash(value, hashAlgorithm))
            {
                hashStr.Append(hashByte.ToString("x2"));
            }
            return hashStr.ToString();
        }

        public static string GetHashedPassword(string plainPassword, string salt)
        {
            var sha512 = SHA512.Create();
            var hash = sha512.ComputeHash(Encoding.UTF8.GetBytes((salt ?? "") + plainPassword));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }


        private static System.Security.Cryptography.HashAlgorithm CreateHashAlgorithm(HashAlgorithmType hashArgorithm)
        {
            switch (hashArgorithm)
            {
                case HashAlgorithmType.Sha256:
                    return SHA256.Create();
                case HashAlgorithmType.Sha384:
                    return SHA384.Create();
                case HashAlgorithmType.Sha512:
                    return SHA512.Create();
            }
            //デフォルトはSha256
            return SHA384.Create();
        }



    }
}
