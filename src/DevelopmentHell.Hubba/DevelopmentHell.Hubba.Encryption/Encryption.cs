using System.Security.Cryptography;

namespace DevelopmentHell.Hubba.Encryption
{
    public class Encryption
    {
        public static byte[] Encrypt(AesManaged aes, string plainText, byte[] Key, byte[] IV)
        {
            byte[] encrypted;
            // Create a new AesManaged.

            // Create encryptor
            ICryptoTransform encryptor = aes.CreateEncryptor(Key, IV);
            // Create MemoryStream
            using (MemoryStream ms = new MemoryStream())
            {
                // Create crypto stream using the CryptoStream class. This class is the key to encryption
                // and encrypts and decrypts data from any given stream. In this case, we will pass a memory stream
                // to encrypt
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    // Create StreamWriter and write data to a stream
                    using (StreamWriter sw = new StreamWriter(cs))
                        sw.Write(plainText);
                    encrypted = ms.ToArray();
                }
            }

            // Return encrypted data
            return encrypted;
        }
    }
}