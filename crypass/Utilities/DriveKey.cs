using System.Text;

using IniParser;
using IniParser.Model;

namespace MaturaProject.Utilities;

/// <summary>
/// Manages the getting, reading and writing of/to drives.
/// </summary>
public interface DriveKey
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
    /// <param name="drive">The directory of the drive with the key data</param>
    /// <returns>A list of all key data</returns>
    public static List<(string, string, string)> GetKeyData(DirectoryInfo drive)
    {
        List<(string, string, string)> keyData = new List<(string, string, string)>();

        string[] fileEntries = Directory.GetFiles(drive.FullName + "/crypass/key-data");
        foreach (string fileName in fileEntries)
        {
            IniData configFile = new FileIniDataParser().ReadFile(fileName);
            keyData.Add((configFile["KeyData"]["AlgorithmType"], configFile["KeyData"]["Prefix"], configFile["KeyData"]["Key"]));
        }

        return keyData;
    }
}