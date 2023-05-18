using static MaturaProject.Cipher.Algorithm.AES;

namespace MaturaProject.Cipher.Algorithm;

public abstract class Cipher
{
    public abstract int Encrypt(IEnumerable<string> target, string keyPath, string keyFilename);
    public abstract int Decrypt(IEnumerable<string> target, List<(string, string, string)> keyData);
}