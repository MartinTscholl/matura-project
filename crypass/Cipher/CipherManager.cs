using MaturaProject.Cipher.Algorithm;

namespace MaturaProject.Cipher;

public class CipherManager
{

    // TODO: write in File: AES:PREFIX:DATA
    // keyPath in drive | drivePath
    public int Encrypt(IEnumerable<string> target, string keyPath, string keyFilename, AlgorithmType algorithmType = AlgorithmType.Aes)
    {
        // TODO: gen prefix
        CipherFactory factory = new CipherFactory();
        Algorithm.Cipher cipher = factory.CreateCipher(algorithmType);
        return cipher.Encrypt(target, keyPath, keyFilename);
    }

    /**
     * keyData: (algorithmType, prefix, data)
     * decrypt all data with key and target->data by searching with prefix and algorithmType
     */
    public int Decrypt(IEnumerable<string> target, List<(string, string, string)> keyData)
    {
        // TODO: get prefix from key:file that should be decrypted 
        AlgorithmType algorithmType = AlgorithmType.Aes; // change obviously

        CipherFactory factory = new CipherFactory();
        Algorithm.Cipher aesCipher = factory.CreateCipher(algorithmType);
        return aesCipher.Decrypt(target, keyData);
    }
}