using System.Security.Cryptography;
using System.Text;

namespace WebAppWithHighSecurity.Data;

public interface IHashingService
{
    object HashSha2(object textToHash);
    object HashHmac(object textToHash, byte[] key);
    object HashPbkdf2(object textToHash, byte[] salt, int iterations = 10000);
    object HashBcrypt(object textToHash);
}

public class HashingService : IHashingService
{
    public object HashSha2(object textToHash)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            if (textToHash is string text)
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(text));
                return Convert.ToBase64String(hashBytes);
            }
            else if (textToHash is byte[] bytes)
            {
                return sha256.ComputeHash(bytes);
            }
            throw new ArgumentException("Invalid input type");
        }
    }

    public object HashHmac(object textToHash, byte[] key)
    {
        using (HMACSHA256 hmac = new HMACSHA256(key))
        {
            if (textToHash is string text)
            {
                byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(text));
                return Convert.ToBase64String(hashBytes);
            }
            else if (textToHash is byte[] bytes)
            {
                return hmac.ComputeHash(bytes);
            }
            throw new ArgumentException("Invalid input type");
        }
    }

    public object HashPbkdf2(object textToHash, byte[] salt, int iterations = 10000)
    {
        if (textToHash is string text)
        {
            using (var deriveBytes = new Rfc2898DeriveBytes(text, salt, iterations, HashAlgorithmName.SHA256))
            {
                return Convert.ToBase64String(deriveBytes.GetBytes(32));
            }
        }
        else if (textToHash is byte[] bytes)
        {
            using (var deriveBytes = new Rfc2898DeriveBytes(bytes, salt, iterations, HashAlgorithmName.SHA256))
            {
                return deriveBytes.GetBytes(32);
            }
        }
        throw new ArgumentException("Invalid input type");
    }

    public object HashBcrypt(object textToHash)
    {
        if (textToHash is string text)
        {
            return BCrypt.Net.BCrypt.HashPassword(text);
        }
        throw new ArgumentException("BCRYPT only supports string input");
    }
}