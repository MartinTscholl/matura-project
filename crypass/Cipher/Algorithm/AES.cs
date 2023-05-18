using System.IO.Compression;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using IniParser;
using IniParser.Model;

namespace MaturaProject.Cipher.Algorithm;

public class AES : Cipher
{
    private Aes _aes;

    public AES()
    {
        _aes = Aes.Create();
    }

    /**
     * target: data files/folders to encrypt
     * keyPath: path to key file
     */
    public override int Encrypt(IEnumerable<string> targets, string keyPath, string keyFilename)
    {
        // 1. encrypt all targets in for loop
        // 2. encrypt all files, if they're in a folder in a loop (else just encrypt file)
        foreach (string target in targets)
        {
            // check if target is directory or file
            if (Directory.Exists(target))
            {
                string[] files = Directory.GetFiles(target);
                EncryptFolder(target, keyPath, keyFilename);
            }
            else if (File.Exists(target))
            {
                // 4. process file here
                EncryptFile(target, keyPath, keyFilename);
            }
        }
        return 0;
    }

    private int EncryptFolder(string folderPath, string keyPath, string keyFilename)
    {
        int bytesProcessed = 0;
        byte[] buffer = new byte[1024 * 1024];

        // Create a new archive file to store the folder contents
        string archivePath = folderPath + ".zip";
        using (ZipArchive archive = ZipFile.Open(archivePath, ZipArchiveMode.Create))
        {
            // Add all the files in the folder to the archive
            foreach (string filePath in Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories))
            {
                // Create a new entry in the archive for the file
                ZipArchiveEntry entry = archive.CreateEntry(filePath.Substring(folderPath.Length + 1));

                // Open the file and copy its contents to the archive entry
                using (Stream inputStream = File.OpenRead(filePath))
                using (Stream outputStream = entry.Open())
                {
                    int bytesRead;
                    while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        outputStream.Write(buffer, 0, bytesRead);
                        bytesProcessed += bytesRead;
                    }
                }
            }
        }

        bytesProcessed += EncryptFile(archivePath, keyPath, keyFilename);
        File.Delete(archivePath);

        return bytesProcessed;
    }

    private int EncryptFile(string target, string keyPath, string keyFilename)
    {
        string prefix = Guid.NewGuid().ToString();
        int bytesProcessed = 0;
        byte[] buffer = new byte[1024 * 1024];
        using (FileStream inputFileStream = new FileStream(target, FileMode.Open, FileAccess.Read))
        {
            using (MemoryStream outputFileStream = new MemoryStream())
            {
                using (ICryptoTransform encryptor = _aes.CreateEncryptor())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(outputFileStream, encryptor, CryptoStreamMode.Write))
                    {
                        int bytesRead;
                        while ((bytesRead = inputFileStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            cryptoStream.Write(buffer, 0, bytesRead);
                            bytesProcessed += bytesRead;
                        }
                        cryptoStream.FlushFinalBlock();
                    }
                }
                byte[] encryptedData = outputFileStream.ToArray();
                string base64EncryptedData = Convert.ToBase64String(encryptedData);
                File.WriteAllText(target + ".encrypted", AlgorithmType.Aes + "|||" + prefix + "|||" + base64EncryptedData);
            }
        }

        IniData data = new IniData();
        string keyIvString = Convert.ToBase64String(_aes.Key) + "|||" + Convert.ToBase64String(_aes.IV);

        data.Sections.AddSection("KeyData");
        data["KeyData"].AddKey("AlgorithmType", AlgorithmType.Aes.ToString());
        data["KeyData"].AddKey("Prefix", prefix);
        data["KeyData"].AddKey("Key", keyIvString);

        FileIniDataParser parser = new FileIniDataParser();
        parser.WriteFile(keyPath + keyFilename + "." + Path.GetFileName(target) + ".ini", data);

        // without ini parser
        // string keyIvString = Convert.ToBase64String(_aes.Key) + "|||" + Convert.ToBase64String(_aes.IV);
        // File.WriteAllText(keyPath + "text.ini", keyIvString);

        return bytesProcessed;
    }

    /**
     * target: data files/folders to decrypt
     * keyData: (algorithmType, prefix, data) keys from key file
     */
    public override int Decrypt(IEnumerable<string> targets, List<(string, string, string)> keyData)
    {
        foreach ((string algorithmType, string prefix, string key) in keyData)
        foreach (var target in targets)
        // foreach ((string target, (string algorithmType, string prefix, string key)) in targets.Zip(keyData, (t, k) => (t, k)))
        {
            // check if target uuid and key uuid match
            string targetContent = File.ReadAllText(target);
            string[] targetParts = targetContent.Split("|||");
            string targetPrefix = targetParts[1];
            if (targetPrefix == prefix)
            {
                if (File.Exists(target) && target.EndsWith(".zip.encrypted"))
                {
                    DecryptFolder(target, (algorithmType, prefix, key));
                }
                else if (File.Exists(target) && target.EndsWith(".encrypted"))
                {
                    DecryptFile(target, (algorithmType, prefix, key));
                }
            }
        }

        return 0;
    }

    private int DecryptFolder(string folderPath, (string, string, string) keyData)
    {
        int bytesProcessed = 0;
        byte[] buffer = new byte[1024 * 1024];

        string[] keyIvParts = keyData.Item3.Split(new string[] { "|||" }, StringSplitOptions.None);
        _aes.Key = Convert.FromBase64String(keyIvParts[0].Trim());
        _aes.IV = Convert.FromBase64String(keyIvParts[1].Trim());

        string archiveFilePath = folderPath.Remove(folderPath.Length - 10);

        // Decrypt the archive file
        DecryptFile(folderPath, keyData);

        Directory.CreateDirectory(archiveFilePath.Remove(archiveFilePath.Length - 4));
        ZipFile.ExtractToDirectory(archiveFilePath, archiveFilePath.Remove(archiveFilePath.Length - 4));

        File.Delete(archiveFilePath);
        return bytesProcessed;
    }


    private int DecryptFile(string target, (string, string, string) keyData)
    {
        int bytesProcessed = 0;
        byte[] buffer = new byte[1024 * 1024];

        string[] keyIvParts = keyData.Item3.Split(new string[] { "|||" }, StringSplitOptions.None);
        _aes.Key = Convert.FromBase64String(keyIvParts[0].Trim());
        _aes.IV = Convert.FromBase64String(keyIvParts[1].Trim());

        using (FileStream inputFileStream = new FileStream(target, FileMode.Open, FileAccess.Read))
        {
            string newTarget = target.Remove(target.Length - 10); // -10 for ".encrypted"

            string targetContent = File.ReadAllText(target);
            string[] targetParts = targetContent.Split("|||");
            string encryptedDataString = targetParts[2];
            byte[] encryptedData = Convert.FromBase64String(encryptedDataString);

            using (MemoryStream inputStream = new MemoryStream(encryptedData))
            {
                using (MemoryStream outputStream = new MemoryStream())
                {
                    using (ICryptoTransform decryptor = _aes.CreateDecryptor())
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(inputStream, decryptor, CryptoStreamMode.Read))
                        {
                            int bytesRead;
                            while ((bytesRead = cryptoStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                outputStream.Write(buffer, 0, bytesRead);
                                bytesProcessed += bytesRead;
                            }
                        }
                    }
                    byte[] decryptedData = outputStream.ToArray();
                    File.WriteAllBytes(newTarget, decryptedData);
                }
            }
        }
        return bytesProcessed;
    }
}