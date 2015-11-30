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

        /// <summary>
        /// Creates hash from password
        /// </summary>
        /// <param name="password"></param>
        /// <returns>Password hash and salt</returns>
        public static PasswordData CreateHash(string password)
        {
            using (var deriveBytes = new Rfc2898DeriveBytes(password, SaltSize, PBKDF2IterCount))
            {
                byte[] salt = deriveBytes.Salt;
                byte[] subkey = deriveBytes.GetBytes(PBKDF2SubkeyLength);

                return new PasswordData()
                {
                    PasswordHash = Convert.ToBase64String(subkey),
                    PasswordSalt = Convert.ToBase64String(salt)
                };
            }
        }

        /// <summary>
        /// Verifies password with existing hash and salt
        /// </summary>
        /// <param name="hashedPassword"></param>
        /// <param name="salt"></param>
        /// <param name="password"></param>
        /// <returns>Result of the verification</returns>
        public static bool VerifyHashedPassword(string hashedPassword, string salt, string password)
        {
            byte[] hashedPasswordBytes = Convert.FromBase64String(hashedPassword);
            byte[] saltBytes = Convert.FromBase64String(salt);

            using (var deriveBytes = new Rfc2898DeriveBytes(password, saltBytes, PBKDF2IterCount))
            {
                byte[] generatedSubkey = deriveBytes.GetBytes(PBKDF2SubkeyLength);
                return hashedPasswordBytes.SequenceEqual(generatedSubkey);
            }
            
        }
    }
}
