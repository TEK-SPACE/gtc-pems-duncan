using System;
using System.Security.Cryptography;
using System.Text;
using NLog;

namespace Duncan.PEMS.Security
{
    public class EncryptionManager
    {
        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        public enum HashType
        {
            MD5 = 128,
            SHA1 = 160,
            SHA256 = 256,
            SHA384 = 384,
            SHA512 = 512
        }


        public static string Hash(string text, byte[] salt)
        {
            return Hash(HashType.MD5, text, salt);
        }

        public static string Hash(HashType hashAlgorithm, string text, byte[] salt)
        {
            int index;

            // Convert plain text into a byte array.
            byte[] textBytes = Encoding.UTF8.GetBytes(text);

            // Allocate array, which will hold plain text and salt.
            byte[] textWithSalt = new byte[textBytes.Length + salt.Length];

            // Copy plain text bytes into resulting array.
            for (index = 0; index < textBytes.Length; index++)
                textWithSalt[index] = textBytes[index];

            // Append salt bytes to the resulting array.
            for (index = 0; index < salt.Length; index++)
                textWithSalt[textBytes.Length + index] = salt[index];

            // Because we support multiple hashing algorithms, we must define
            // hash object as a common (abstract) base class. We will specify the
            // actual hashing algorithm class later during object creation.
            HashAlgorithm hash;

            // Initialize appropriate hashing algorithm class.
            switch (hashAlgorithm)
            {
                case HashType.SHA1:
                    hash = new SHA1Managed();
                    break;
                case HashType.SHA256:
                    hash = new SHA256Managed();
                    break;
                case HashType.SHA384:
                    hash = new SHA384Managed();
                    break;
                case HashType.SHA512:
                    hash = new SHA512Managed();
                    break;
                default:
                    hash = new MD5CryptoServiceProvider();
                    break;
            }

            // Compute hash value of our plain text with appended salt.
            byte[] hashBytes = hash.ComputeHash(textWithSalt);

            // Create array which will hold hash and original salt bytes.
            byte[] hashWithsalt = new byte[hashBytes.Length + salt.Length];

            // Copy hash bytes into resulting array.
            for (index = 0; index < hashBytes.Length; index++)
                hashWithsalt[index] = hashBytes[index];

            // Append salt bytes to the result.
            for (index = 0; index < salt.Length; index++)
                hashWithsalt[hashBytes.Length + index] = salt[index];

            // Convert result into a base64-encoded string.
            return Encoding.Unicode.GetString(hashBytes) + Encoding.Unicode.GetString(salt);
        }


        public static bool Verify(string text, string hashValue)
        {
            return Verify(HashType.MD5, text, hashValue);
        }

        public static bool Verify(HashType hashAlgorithm, string text, string hashValue)
        {
            // Convert base64-encoded hash value into a byte array.
            byte[] hashWithsalt = Encoding.Unicode.GetBytes(hashValue);

            // We must know size of hash (without salt).
            // Size of hash is based on the specified algorithm.
            int hashSizeInBits = (int)hashAlgorithm;

            // Convert size of hash from bits to bytes.
            int hashSizeInBytes = hashSizeInBits / 8;

            // Make sure that the specified hash value is long enough.
            if (hashWithsalt.Length < hashSizeInBytes)
                return false;

            // Allocate array to hold original salt bytes retrieved from hash.
            byte[] salt = new byte[hashWithsalt.Length - hashSizeInBytes];

            // Copy salt from the end of the hash to the new array.
            for (int index = 0; index < salt.Length; index++)
                salt[index] = hashWithsalt[hashSizeInBytes + index];

            // Compute a new hash string.
            string expectedHashString = Hash(hashAlgorithm, text, salt);

            // If the computed hash matches the specified hash,
            // the plain text value must be correct.
            return (hashValue == expectedHashString);
        }
    }
}
