// Internal dependencies
using MaturaProject.Utilities;

// External dependencies
// Ini configuration
using IniParser;
using IniParser.Model;

namespace MaturaProject.CLI;

/// <summary>
/// Represents an instantiation of the CLI for Windows.
/// </summary>
public class WindowsCli : Cli
{
    /// <summary>
    /// Creates an IniData instance which represents the user's configuration.
    /// TODO set path absolute to correct installation location (Appdata?)
    /// </summary>
    private protected IniData _configFile = new FileIniDataParser().ReadFile(Directory.GetCurrentDirectory() + "\\Local\\config.ini");

    /// <summary>
    /// Initializes a new instance of the WindowsCli class.
    /// </summary>
    /// <param name="args">The specified arguments of the program call.</param>
    public WindowsCli(string[] args) : base(args)
    {

    }

    private protected override void Greet()
    {
        PrintProgramName();
        Console.WriteLine("Windows detected :)\n\n");
    }

    private protected override DirectoryInfo GetDrive()
    {
        // TODO check for windows
        string driveName = "";

        if (_encrypt != null)
        {
            if (_encrypt.DriveName == null)
                throw new ErrorException("The [drive] was not [specified]");

            driveName = _encrypt.DriveName;
        }

        else if (_decrypt != null)
        {
            if (_decrypt.DriveName == null)
                throw new ErrorException("The [drive] was not [specified]");

            driveName = _decrypt.DriveName;
        }

        if (driveName == "")
        {
            throw new ErrorException("The [drive] was not [specified]");
        }

        foreach (DriveInfo drive in DriveKey.GetCurrentRemovableDrives())
        {
            if (drive.Name == driveName)
                return new DirectoryInfo(drive.ToString());
        }

        throw new ErrorException("The [drive] was not [found]");
    }
}