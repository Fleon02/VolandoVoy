using System;
using System.Security.Cryptography;
using System.Text;

namespace TFGVolandoVoy.Modelo
{
    public class PasswordEncoder
    {
        private const string ALGORITHM = "SHA-256";
        private const int SALT_LENGTH = 16;

        public static string EncodePassword(string password, string salt = null)
        {
            if (salt == null)
            {
                salt = GenerateSalt();
            }

            string encodedPassword = null;
            try
            {
                using (var sha256 = SHA256.Create())
                {
                    string combined = password + salt;
                    byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combined));
                    encodedPassword = Convert.ToBase64String(hashedBytes);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al codificar la contraseña: {ex.Message}");
            }
            return encodedPassword;
        }

        public static string GenerateSalt()
        {
            byte[] salt = new byte[SALT_LENGTH];
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);
            return Convert.ToBase64String(salt);
        }

    }
}
