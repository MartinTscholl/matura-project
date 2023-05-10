// Internal dependencies
using MaturaProject.Utilities;

// External dependencies
// Ini configuration
using IniParser;
using IniParser.Model;

namespace MaturaProject.CLI;

/// <summary>
/// Represents an instantiation of the CLI for Linux.
/// </summary>
public class LinuxCli : Cli
{
    /// <summary>
    /// Creates an IniData instance which represents the user's configuration.
    /// TODO set path absolute to $HOME/.config/crypass/Local/config.ini        
    /// </summary>
    private protected IniData _configFile = new FileIniDataParser().ReadFile(Directory.GetCurrentDirectory() + "/Local/config.ini");

    /// <summary>
    /// Initializes a new instance of the LinuxCli class.
    /// </summary>
    /// <param name="args">The specified arguments of the program call.</param>
    public LinuxCli(string[] args) : base(args)
    {

    }

    private protected override void Greet()
    {
        PrintProgramName();

        // TODO create method for automatic coloring 
        WriteColor("[L]", ConsoleColor.Red);
        WriteColor("[I]", ConsoleColor.DarkYellow);
        WriteColor("[N]", ConsoleColor.Yellow);
        WriteColor("[U]", ConsoleColor.Green);
        WriteColor("[X]", ConsoleColor.Cyan);

        Console.Write(" ");

        WriteColor("[D]", ConsoleColor.Blue);
        WriteColor("[E]", ConsoleColor.DarkMagenta);
        WriteColor("[T]", ConsoleColor.Red);
        WriteColor("[E]", ConsoleColor.DarkYellow);
        WriteColor("[C]", ConsoleColor.Yellow);
        WriteColor("[T]", ConsoleColor.Green);
        WriteColor("[E]", ConsoleColor.Cyan);
        WriteColor("[D]", ConsoleColor.Blue);
        Console.Write(" ");

        WriteColor("[:]", ConsoleColor.DarkMagenta);
        WriteColor("[D]", ConsoleColor.Red);

        Console.WriteLine("\n");
    }

    private protected override DirectoryInfo GetDrive()
    {
        string driveName = "";

        // null checking
        if (_encrypt is not null)
        {
            if (_encrypt.DriveName is null)
                throw new ErrorException("The [drive] was not [specified]");

            driveName = _encrypt.DriveName;
        }

        else if (_decrypt is not null)
        {
            if (_decrypt.DriveName is null)
                throw new ErrorException("The [drive] was not [specified]");

            driveName = _decrypt.DriveName;
        }

        if (driveName == "")
            throw new ErrorException("The [drive] was not [specified]");

        // return the user specified removable drive if connected
        IEnumerable<DriveInfo> removableDrives = DriveKey.GetCurrentRemovableDrives();
        foreach (var drive in removableDrives)
            if (driveName.Equals(drive.Name))
                return new DirectoryInfo(drive.ToString());

        MaturaProject.Log.Info("Drive was not found by search, waiting for new drive(s) to connect...");

        // drive was not found => ask the user to unplug the drive and plug it back
        IEnumerable<DriveInfo> currentDrives = DriveKey.GetCurrentDrives();
        IEnumerable<DriveInfo> newDrives = currentDrives;

        WriteColor("\n[⚠] The [drive] has [not] been found on the device!\n" +
                                   "\t [=>] Maybe it was [not connected correctly]!\n\n",
                            ConsoleColor.Yellow);
        WriteColor("[ℹ] Please [plug] your [drive] into the device.\n" +
                   "\t [=>] If your drive is [plugged in], please [unplug] the drive and [plug] it back into the device.\n\n",
            ConsoleColor.Cyan);
        WriteColor("[⚙ Searching] for a [drive]...\n" +
                   "\t [=>] This could take [a few seconds].", ConsoleColor.Magenta);

        while (true)
        {
            // TODO: when drive is unplugged but its not the correct drive

            // loops again until something connects
            while (newDrives.Count() <= currentDrives.Count())
            {
                if (newDrives.Count() < currentDrives.Count())
                    currentDrives = newDrives;

                Thread.Sleep(200);
                newDrives = DriveKey.GetCurrentDrives();
            }

            MaturaProject.Log.Info("New drive(s) connected");

            // return the user specified removable drive if connected
            IEnumerable<DriveInfo> newRemovableDrives = newDrives.Except(currentDrives).Concat(currentDrives.Except(newDrives));
            foreach (var drive in newRemovableDrives)
            {
                if (drive.Name.Length > driveName.Length
                    && drive.Name.Substring(drive.Name.Length - driveName.Length, driveName.Length)
                        .Equals(driveName))
                {
                    WriteColor("\n\n[✔] The following drive has been connected: \n\t [" + drive.Name + "]\n",
                        ConsoleColor.Green);

                    return new DirectoryInfo(drive.ToString());
                }
            }

            // set both drives equal to check for changes again
            currentDrives = newDrives;
        };
    }
}