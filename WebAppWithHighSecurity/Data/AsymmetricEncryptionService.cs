using System.Security.Cryptography;
using System.Text;

namespace WebAppWithHighSecurity.Data
{
    public interface IAsymmetricEncryptionService
    {
        byte[] Encrypt(string plainText, string publicKey);
        string Decrypt(byte[] cipherBytes);
        string GetPublicKey();
    }

    public class AsymmetricEncryptionService : IAsymmetricEncryptionService
    {
        private readonly RSA _privateKey;
        private readonly RSA _publicKey;

        public AsymmetricEncryptionService()
        {
            _privateKey = RSA.Create(2048); // Ensure key size is 2048 bits
            _publicKey = RSA.Create(2048);  // Ensure key size is 2048 bits

            if (File.Exists("privateKey.xml") && File.Exists("publicKey.xml"))
            {
                _privateKey.FromXmlString(File.ReadAllText("privateKey.xml"));
                _publicKey.FromXmlString(File.ReadAllText("publicKey.xml"));
            }
            else
            {
                File.WriteAllText("privateKey.xml", _privateKey.ToXmlString(true));
                File.WriteAllText("publicKey.xml", _publicKey.ToXmlString(false));
            }
        }

        public byte[] Encrypt(string plainText, string publicKey)
        {
            using (var rsa = RSA.Create())
            {
                rsa.FromXmlString(publicKey);
                return rsa.Encrypt(Encoding.UTF8.GetBytes(plainText), RSAEncryptionPadding.OaepSHA256);
            }
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
    }
}