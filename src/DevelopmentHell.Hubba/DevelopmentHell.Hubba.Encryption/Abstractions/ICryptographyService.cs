using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.Cryptography.Service.Abstractions
{
    public interface ICryptographyService
    {
        byte[] Encrypt(string plainText);
        byte[] Encrypt(byte[] bytes);
        string Decrypt(byte[] encrypted);
        byte[] DecryptToBytes(byte[] encrypted);
        Result<HashData> HashString(string text, string salt);
        string GetSaltValidChars();
	}
}