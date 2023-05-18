using System.Text;
using IniParser;
using IniParser.Model;

namespace MaturaProject.Utilities;

/// <summary>
/// Manages the getting, reading and writing of/to drives.
/// </summary>
public interface IDriveKey
{
    /// <summary>
    /// Prints the specified drive names indented to the standard output stream in a green foreground color.
    /// </summary>
    /// <param name="driveNames">The StringBuilder of the by semicolons seperated drive names.</param>
    public static void PrintDriveNames(StringBuilder driveNames)
    {
        var driveName = "";
        var subChar = driveNames.ToString();
        for (var index = 0; index < driveNames.Length; index++)
        {
            if (subChar[index] != ';')
                driveName += subChar[index];
            else
            {
                CLI.Cli.WriteColor("\t [" + driveName + "]\n", ConsoleColor.Green);
                driveName = "";
            }
        }

        Console.WriteLine();
    }

    /// <summary>
    /// Gets a List that contains every drive on the device.
    /// </summary>
    /// <returns>A List with every drive.</returns>
    public static List<DriveInfo> GetCurrentDrives()
    {
        var driveInfos = new List<DriveInfo>();
        foreach (var drive in DriveInfo.GetDrives())
        {
            driveInfos.Add(drive);
        }

        return driveInfos;
    }

    /// <summary>
    /// Get a List that contains every removable currently connected drive on the
    /// device.
    /// </summary>
    /// <returns>A List with every removable drive.</returns>
    public static List<DriveInfo> GetCurrentRemovableDrives()
    {
        var driveInfos = new List<DriveInfo>();
        foreach (var drive in DriveInfo.GetDrives())
        {
            if (drive.DriveType == DriveType.Removable)
                driveInfos.Add(drive);
        }

        return driveInfos;
    }

    /// <summary>
    /// Gets a list of all key data files that contains a triple of the algorithm type, the prefix and the key. 
    /// </summary>
    /// <param name="directory">The directory of with the key data</param>
    /// <returns>A list of all key data</returns>
    public static List<(string, string, string)> GetKeyData(DirectoryInfo directory)
    {
        List<(string, string, string)> keyData = new List<(string, string, string)>();

        string[] fileEntries = Directory.GetFiles(directory.FullName);

        foreach (string fileName in fileEntries)
        {
            if (!Path.GetExtension(fileName).Equals(".ini", StringComparison.OrdinalIgnoreCase))
                continue;

            IniData configFile = new FileIniDataParser().ReadFile(fileName);

            SectionData keyDataSection = configFile.Sections.GetSectionData("KeyData");

            if (keyDataSection != null &&
                keyDataSection.Keys.ContainsKey("AlgorithmType") &&
                keyDataSection.Keys.ContainsKey("Prefix") &&
                keyDataSection.Keys.ContainsKey("Key"))
            {
                string algorithmType = keyDataSection.Keys["AlgorithmType"];
                string prefix = keyDataSection.Keys["Prefix"];
                string key = keyDataSection.Keys["Key"];
                keyData.Add((algorithmType, prefix, key));
            }
        }

        return keyData;
    }
}