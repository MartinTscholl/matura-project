using System.Security.Cryptography;
using IniParser;
using IniParser.Model;
using MaturaProject.CipherAlgorithms;

namespace MaturaProject;

public class CipherManager : Cipher
{
    private SymmetricAlgorithm _symmetricAlgorithm;

    private byte[] _key;

    public byte[] Key
    {
        get => _key;
        set => _key = value ?? throw new ArgumentNullException(nameof(value));
    }

    private string _prefix;

    public string Prefix
    {
        get => _prefix;
        set => _prefix = value ?? throw new ArgumentNullException(nameof(value));
    }
    // 

    // key refresh 
    private static int refresh = 5;

    public override int Encrypt(IEnumerable<string> target, AlgorithmType algorithmType = AlgorithmType.Aes)
    {
        switch (algorithmType)
        {
            case AlgorithmType.Aes:
                var aesCipherAlgorithm = new AesCipherAlgorithm { Key = _key };
                return aesCipherAlgorithm.Encrypt(target, algorithmType);
            case AlgorithmType.Tdes:
                // TODO add Tdes
                break;
            case AlgorithmType.Blowfish:
                // TODO add Blowfish
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(algorithmType), algorithmType, null);
        }
        return 0;
    }

    public override int Decrypt(IEnumerable<string> paths)
    {
        return 0;
    }

    public string CreatePrefix()
    {
        return Guid.NewGuid().ToString();
    }

    public byte[] CreateKey(AlgorithmType algorithmType = AlgorithmType.Aes)
    {
        switch (algorithmType)
        {
            case AlgorithmType.Aes:
                _symmetricAlgorithm = Aes.Create();
                break;
            default:
                _symmetricAlgorithm = Aes.Create();
                break;
        }
        return _symmetricAlgorithm.Key;
    }

    public int SetKey(DirectoryInfo directoryInfo)
    {
        int refresh = 5;
        directoryInfo.Refresh();
        for (int i = 0; i < refresh && directoryInfo.Exists == false; ++i)
        {
            Thread.Sleep(refresh * 1000);
            directoryInfo.Refresh();
        }

        return 0;
    }

    public string GetKey(DirectoryInfo directoryInfo)
    {
        directoryInfo.Refresh();
        for (int i = 0; i < refresh && directoryInfo.Exists == false; ++i)
        {
            Thread.Sleep(refresh * 1000);
            directoryInfo.Refresh();
        }

        IniData driveKeyStorage = new FileIniDataParser().ReadFile(directoryInfo.FullName);
        string key = driveKeyStorage[_prefix]["Key"];

        return key;
    }

    public AlgorithmType GetAlgorithmType(DirectoryInfo directoryInfo)
    {
        directoryInfo.Refresh();
        for (int i = 0; i < refresh && directoryInfo.Exists == false; ++i)
        {
            Thread.Sleep(refresh * 1000);
            directoryInfo.Refresh();
        }

        IniData driveKeyStorage = new FileIniDataParser().ReadFile(directoryInfo.FullName);
        string algorithmType = driveKeyStorage[_prefix]["AlgorithmType"];

        return (AlgorithmType)Enum.Parse(typeof(AlgorithmType), algorithmType);
    }

    public IEnumerable<string> PathManager(IEnumerable<string> paths)
    {
        IEnumerable<string> fullPaths = null;

        foreach (var path in paths)
        {
            int flag = 0;
            // case 1: Check if path is fully parsed already
            var fullPath = path;
            try
            {
                fullPath = Path.GetFullPath(path);
                Console.WriteLine("path is rooted");
            }
            catch (ArgumentException)
            {
                flag = 0;
            }
            // case 2: Path is in the current directory
            
            // case 3: no path, only file

            // int index = paths.ToList().IndexOf(path);
            // paths.ToList()[index] = fullPath;
            yield return fullPath;
        }
    }

}