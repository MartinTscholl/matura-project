// Internal Dependencies


// External Dependencies
using CommandLine;
using CommandLine.Text;

namespace MaturaProject.Utilities;

/// <summary>
/// Represents a blueprint of the program for CLI and GUI.
/// </summary>
public abstract class Base
{
    /// <summary>
    /// The specified encryption flags
    /// </summary>
    /// <returns></returns>
    private protected EncryptOptions? _encrypt = null;

    /// <summary>
    /// The specified decryption flags
    /// </summary>
    /// <returns></returns>
    private protected DecryptOptions? _decrypt = null;

    /// <summary>
    /// Returns the user specified drive directory to use for ciphering.
    /// </summary>
    /// <returns>A DirectoryInfo object of the specified drive or null if no drive was chosen.</returns>
    private protected abstract DirectoryInfo GetDrive();

    /// <summary>
    /// Initiates the program execution.
    /// </summary>
    public abstract void Init();
}

/// <summary>
/// A flag marked when the encryption option has been specified on the program call.
/// </summary>
[Verb("encrypt", HelpText = "To encrypt the contents of the specified target(s)")]
public class EncryptOptions
{
    /// <summary>
    /// A flag that indicates the specified targets for encryption
    /// </summary>
    [Option('t', "target", Separator = ' ', Required = true, HelpText = "To specify the target(s)")]
    public IEnumerable<string>? Target { get; set; }

    /// <summary>
    /// A flag that indicates the specified drive
    /// </summary>
    [Option('d', "drive", Required = true, HelpText = "To specify the drive's name")]
    public string? DriveName { get; set; }

    /// <summary>
    /// A flag that indicates the specified encryption algorithm
    /// </summary>
    /// <value></value>
    [Option('a', "algorithm", HelpText = "The encryption algorithm")]
    public string? Algorithm { get; set; }

    /// <summary>
    /// Defines the examples for the help message.
    /// </summary>
    [Usage(ApplicationAlias = "crypass")]
    public static IEnumerable<Example> Examples
    {
        get
        {
            return new List<Example>() {
            new Example("Encrypts the specified directory dir and the file file using the drive drive",
                new EncryptOptions {
                    Target = Enumerable.Empty<string>().Append<string>("dir/").Append<string>("file"),
                    DriveName = "drive",
                }),
            };
        }
    }
}

/// <summary>
/// A flag marked when the decryption option has been specified on the program call.
/// </summary>
[Verb("decrypt", HelpText = "To decrypt the contents of the specified target(s)")]
public class DecryptOptions
{
    /// <summary>
    /// A flag that indicates the specified targets for decryption
    /// </summary>
    [Option('t', "target", Separator = ' ', Required = true, HelpText = "To specify the target(s)")]
    public IEnumerable<string>? Target { get; set; }

    /// <summary>
    /// A flag that indicates the specified drive
    /// </summary>
    [Option('d', "drive", Required = true, HelpText = "To specify the drive's name")]
    public string? DriveName { get; set; }

    /// <summary>
    /// Defines the examples for the help message.
    /// </summary>
    [Usage(ApplicationAlias = "crypass")]
    public static IEnumerable<Example> Examples
    {
        get
        {
            return new List<Example>() {
            new Example("Decrypts the specified directory dir and the file file using the drive drive",
                new DecryptOptions {
                    Target = Enumerable.Empty<string>().Append<string>("dir/").Append<string>("file"),
                    DriveName = "drive",
                }),
            };
        }
    }
}