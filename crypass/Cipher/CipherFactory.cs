using MaturaProject.Cipher.Algorithm;

namespace MaturaProject.Cipher;

public class CipherFactory
{
    public Algorithm.Cipher CreateCipher(Algorithm.AlgorithmType algorithmType)
    {
        return algorithmType switch
        {
            Algorithm.AlgorithmType.Aes => new AES(),
            // Cipher.AlgorithmType.TripleDes => new Cipher.Cipher.TripleDES(),
            // Cipher.AlgorithmType.Des => new Cipher.Cipher.DES(),
            // Cipher.AlgorithmType.Blowfish => new Cipher.Cipher.Blowfish(),
            _ => throw new ArgumentOutOfRangeException(nameof(algorithmType), algorithmType, null)
        };
    }
}