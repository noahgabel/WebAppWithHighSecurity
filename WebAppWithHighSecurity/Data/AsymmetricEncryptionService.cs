using System.Security.Cryptography;
using System.Text;

namespace WebAppWithHighSecurity.Data
{
    public interface IAsymmetricEncryptionService
    {
        byte[] Encrypt(string plainText, string publicKey);
        string Decrypt(byte[] cipherBytes);
        string GetPublicKey();
        string GetPublicKeyFingerprint();
    }

    public class AsymmetricEncryptionService : IAsymmetricEncryptionService
    {
        private readonly RSA _privateKey;
        private readonly RSA _publicKey;

        // Store keys in a shared, fixed location
        private static readonly string KeyDirectory = @"C:\Users\noahg\.aspnet\keys";
        private static readonly string PrivateKeyPath = Path.Combine(KeyDirectory, "privateKey.xml");
        private static readonly string PublicKeyPath = Path.Combine(KeyDirectory, "publicKey.xml");

        public AsymmetricEncryptionService()
        {
            Directory.CreateDirectory(KeyDirectory); // Ensure directory exists

            _privateKey = RSA.Create(2048);
            _publicKey = RSA.Create(2048);

            if (File.Exists(PrivateKeyPath) && File.Exists(PublicKeyPath))
            {
                _privateKey.FromXmlString(File.ReadAllText(PrivateKeyPath));
                _publicKey.FromXmlString(File.ReadAllText(PublicKeyPath));
            }
            else
            {
                File.WriteAllText(PrivateKeyPath, _privateKey.ToXmlString(true));
                File.WriteAllText(PublicKeyPath, _publicKey.ToXmlString(false));
            }

        }

        public byte[] Encrypt(string plainText, string publicKey)
        {
            using var rsa = RSA.Create();
            rsa.FromXmlString(publicKey);
            return rsa.Encrypt(Encoding.UTF8.GetBytes(plainText), RSAEncryptionPadding.OaepSHA256);
        }

        public string Decrypt(byte[] cipherBytes)
        {
            try
            {
                var decryptedBytes = _privateKey.Decrypt(cipherBytes, RSAEncryptionPadding.OaepSHA256);
                return Encoding.UTF8.GetString(decryptedBytes);
            }
            catch (CryptographicException ex)
            {
                Console.WriteLine($"CryptographicException: {ex.Message}");
                throw;
            }
        }

        public string GetPublicKey()
        {
            return _publicKey.ToXmlString(false);
        }

        public string GetPublicKeyFingerprint()
        {
            using var sha256 = SHA256.Create();
            var keyBytes = Encoding.UTF8.GetBytes(GetPublicKey());
            var hash = sha256.ComputeHash(keyBytes);
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

    }
}
