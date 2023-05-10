using System.Security.Cryptography;

namespace MaturaProject.CipherAlgorithms;

public class AesCipherAlgorithm : Cipher
{
    private byte[] _key;

    private SymmetricAlgorithm _symmetricAlgorithm;

    public SymmetricAlgorithm SymmetricAlgorithm
    {
        get => _symmetricAlgorithm;
        set => _symmetricAlgorithm = value ?? throw new ArgumentNullException(nameof(value));
    }

    public byte[] Key
    {
        get => _key;
        set => _key = value;
    }

    public override int Encrypt(IEnumerable<string> target, AlgorithmType algorithmType = AlgorithmType.Aes)
    {
        foreach (var path in target)
        {
            using FileStream outputFile = new FileStream(path, FileMode.Create);
            using FileStream inputFile = new FileStream(path, FileMode.Open);
            // Create an encryptor to perform the encryption
            ICryptoTransform encryptor = _symmetricAlgorithm.CreateEncryptor();
            using CryptoStream cryptoStream = new CryptoStream(outputFile, encryptor, CryptoStreamMode.Write);
            // Encrypt the data
            inputFile.CopyTo(cryptoStream);
        }
        
        return 0;
    }

    public override int Decrypt(IEnumerable<string> paths)
    {
        throw new NotImplementedException();
    }
}