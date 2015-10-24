using System;
using System.Linq;
using System.Security.Cryptography;

namespace DataAccess.Security
{
    public static class PasswordHelper
    {
        private static readonly int PBKDF2IterCount = 100000;
        private static readonly int PBKDF2SubkeyLength = 160 / 8;
        private static readonly int SaltSize = 128 / 8;

        public static PasswordData CreateHash(string password)
        {
            byte[] salt;
            byte[] subkey;
            using (var deriveBytes = new Rfc2898DeriveBytes(password, SaltSize, PBKDF2IterCount))
            {
                salt = deriveBytes.Salt;
                subkey = deriveBytes.GetBytes(PBKDF2SubkeyLength);
            }

            byte[] outputBytes = new byte[1 + SaltSize + PBKDF2SubkeyLength];
            Buffer.BlockCopy(salt, 0, outputBytes, 1, SaltSize);
            Buffer.BlockCopy(subkey, 0, outputBytes, 1 + SaltSize, PBKDF2SubkeyLength);
            return new PasswordData()
            {
                PasswordHash = Convert.ToBase64String(outputBytes),
                PasswordSalt = Convert.ToBase64String(salt)
            };
        }

        public static bool VerifyHashedPassword(string hashedPassword, string salt, string password)
        {
            byte[] hashedPasswordBytes = Convert.FromBase64String(hashedPassword);

            // Wrong length or version header.
            if (hashedPasswordBytes.Length != (1 + SaltSize + PBKDF2SubkeyLength) || hashedPasswordBytes[0] != 0x00)
                return false;

            byte[] saltBytes = Convert.FromBase64String(salt);
            Buffer.BlockCopy(hashedPasswordBytes, 1, saltBytes, 0, SaltSize);
            byte[] storedSubkey = new byte[PBKDF2SubkeyLength];
            Buffer.BlockCopy(hashedPasswordBytes, 1 + SaltSize, storedSubkey, 0, PBKDF2SubkeyLength);

            byte[] generatedSubkey;
            using (var deriveBytes = new Rfc2898DeriveBytes(password, saltBytes, PBKDF2IterCount))
            {
                generatedSubkey = deriveBytes.GetBytes(PBKDF2SubkeyLength);
            }
            return storedSubkey.SequenceEqual(generatedSubkey);
        }
    }
}