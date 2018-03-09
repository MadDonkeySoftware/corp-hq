// Copyright (c) MadDonkeySoftware

namespace Api
{
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// Static class to hold helper methods related to security.
    /// </summary>
    internal static class SecurityHelpers
    {
        private static int saltLengthLimit = 32;

        internal static byte[] StringToBytes(string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }

        internal static string BytesToString(byte[] value)
        {
            return Encoding.UTF8.GetString(value);
        }

        internal static byte[] GenerateSaltedHash(string plainText, byte[] salt)
        {
            var bytes = StringToBytes(plainText);
            return GenerateSaltedHash(bytes, salt);
        }

        internal static byte[] GenerateSaltedHash(byte[] plainText, byte[] salt)
        {
            HashAlgorithm algorithm = new SHA256Managed();

            var plainTextWithSaltBytes = new byte[plainText.Length + salt.Length];

            for (int i = 0; i < plainText.Length; i++)
            {
                plainTextWithSaltBytes[i] = plainText[i];
            }

            for (int i = 0; i < salt.Length; i++)
            {
                plainTextWithSaltBytes[plainText.Length + i] = salt[i];
            }

            return algorithm.ComputeHash(plainTextWithSaltBytes);
        }

        internal static bool CompareByteArrays(byte[] array1, byte[] array2)
        {
            if (array1.Length != array2.Length)
            {
                return false;
            }

            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i])
                {
                    return false;
                }
            }

            return true;
        }

        internal static byte[] GetSalt()
        {
            return GetSalt(saltLengthLimit);
        }

        internal static byte[] GetSalt(int maximumSaltLength)
        {
            var salt = new byte[maximumSaltLength];
            using (var random = new RNGCryptoServiceProvider())
            {
                random.GetNonZeroBytes(salt);
            }

            return salt;
        }
    }
}