using System.Security.Cryptography;
using System.Text;
using Costealo.Services.Contracts;

namespace Costealo.Services.Security;

public class EncryptionService(IConfiguration cfg) : IEncryptionService
{
    private readonly string _key = cfg["Encryption:Key"] ?? "CostealoEncryptionKey1234567890"; // 32 chars for AES-256

    public string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText)) return plainText;

        using var aes = Aes.Create();
        aes.Key = GetKey();
        aes.GenerateIV();

        var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream();
        ms.Write(aes.IV, 0, aes.IV.Length); // Store IV at the beginning

        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        using (var writer = new StreamWriter(cs))
        {
            writer.Write(plainText);
        }

        return Convert.ToBase64String(ms.ToArray());
    }

    public string Decrypt(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText)) return cipherText;

        try
        {
            var fullCipher = Convert.FromBase64String(cipherText);

            using var aes = Aes.Create();
            aes.Key = GetKey();

            var iv = new byte[aes.IV.Length];
            Array.Copy(fullCipher, 0, iv, 0, iv.Length);
            aes.IV = iv;

            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream(fullCipher, iv.Length, fullCipher.Length - iv.Length);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var reader = new StreamReader(cs);

            return reader.ReadToEnd();
        }
        catch
        {
            // If decryption fails, return empty string (data might be corrupted or not encrypted)
            return string.Empty;
        }
    }

    private byte[] GetKey()
    {
        // Ensure key is exactly 32 bytes for AES-256
        var keyBytes = Encoding.UTF8.GetBytes(_key);
        if (keyBytes.Length < 32)
        {
            var paddedKey = new byte[32];
            Array.Copy(keyBytes, paddedKey, keyBytes.Length);
            return paddedKey;
        }
        return keyBytes.Take(32).ToArray();
    }
}
