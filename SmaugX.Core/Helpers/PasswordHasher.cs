using System.Security.Cryptography;

namespace SmaugX.Core.Helpers;

public class PasswordHasher
{
    private const int SaltSize = 16; // Choose an appropriate salt size
    private const int Iterations = 10000; // Choose an appropriate number of iterations
    private const int HashSize = 32; // Choose an appropriate hash size

    public static string HashPassword(string password)
    {
        // Generate a random salt
        byte[] salt;
        using (var rng = new RNGCryptoServiceProvider())
        {
            salt = new byte[SaltSize];
            rng.GetBytes(salt);
        }

        // Hash the password with the salt using PBKDF2 with HMACSHA256
        using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
        {
            byte[] hash = pbkdf2.GetBytes(HashSize);

            // Combine the salt and hash into a single byte array
            byte[] hashBytes = new byte[SaltSize + HashSize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

            // Convert the combined byte array to a base64-encoded string
            string hashedPassword = Convert.ToBase64String(hashBytes);

            return hashedPassword;
        }
    }

    public static bool VerifyPassword(string password, string hashedPassword)
    {
        // Convert the base64-encoded string back to a byte array
        byte[] hashBytes = Convert.FromBase64String(hashedPassword);

        // Extract the salt from the byte array
        byte[] salt = new byte[SaltSize];
        Array.Copy(hashBytes, 0, salt, 0, SaltSize);

        // Hash the provided password with the stored salt and iterations using HMACSHA256
        using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
        {
            byte[] hash = pbkdf2.GetBytes(HashSize);

            // Compare the computed hash with the stored hash
            for (int i = 0; i < HashSize; i++)
            {
                if (hashBytes[SaltSize + i] != hash[i])
                {
                    return false; // Passwords don't match
                }
            }

            return true; // Passwords match
        }
    }
}
