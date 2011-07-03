namespace CaptchaMVC
{
    public interface IEncryption
    {
        string Encrypt(string inputText, string password, byte[] salt);

        string Decrypt(string inputText, string password, byte[] salt);
    }
}
