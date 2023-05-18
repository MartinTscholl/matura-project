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
    /// TODO change path on release       
    /// </summary>
    private protected IniData _configFile = new FileIniDataParser().ReadFile("/etc/crypass/config.ini");
    // private protected IniData _configFile = new FileIniDataParser().ReadFile(Directory.GetCurrentDirectory() + "/Local/config.ini");

    /// <summary>
    /// Initializes a new instance of the LinuxCli class.
    /// </summary>
    /// <param name="args">The specified arguments of the program call.</param>
    public LinuxCli(string[] args) : base(args)
    {

    }

    private protected override DirectoryInfo GetKeyDirectory()
    {
        string keyPath = "";

        if (_encrypt is not null)
        {
            if (_encrypt.KeyPath is null)
                throw new ErrorException("The directory for the [key] was not [specified]");

            keyPath = _encrypt.KeyPath;
        }

        else if (_decrypt is not null)
        {
            if (_decrypt.KeyPath is null)
                throw new ErrorException("The directory for they [key] was not [specified]");

            keyPath = _decrypt.KeyPath;
        }

        if (keyPath == "")
            throw new ErrorException("The directory for the [key] was not [specified]");

        if (!Directory.Exists(keyPath))
            throw new ErrorException("The directory for the [key] was not [found]");

        return new DirectoryInfo(keyPath);
    }

    private protected override void CheckTargets()
    {
        IEnumerable<string> targets = new List<string>();

        if (_encrypt is not null)
        {
            if (_encrypt.Targets is null)
                throw new ErrorException("The [target] was not [specified]");

            foreach (string target in _encrypt.Targets)
            {
                if (!File.Exists(target) && !Directory.Exists(target))
                    throw new ErrorException("The [target] " + target + " was not [found]");
            }
        }

        else if (_decrypt is not null)
        {
            if (_decrypt.Targets is null)
                throw new ErrorException("The [target] was not [specified]");

            foreach (string target in _decrypt.Targets)
            {
                if (!File.Exists(target) && !Directory.Exists(target))
                    throw new ErrorException("The [target] " + target + " was not [found]");
            }
        }
    }
}
