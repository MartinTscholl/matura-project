namespace MaturaProject.CipherAlgorithms;

public abstract class Cipher {
    public enum AlgorithmType
    {
        Aes,
        Tdes,
        Blowfish,
    }

    public abstract int Encrypt(IEnumerable<string> target, AlgorithmType algorithmType = AlgorithmType.Aes);
    public abstract int Decrypt(IEnumerable<string> paths);
}