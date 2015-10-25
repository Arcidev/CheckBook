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
