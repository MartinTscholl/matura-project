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
    /// Returns the user specified directory containing the key file to use for ciphering.
    /// </summary>
    /// <returns>A DirectoryInfo object of the specified file.</returns>
    private protected abstract DirectoryInfo GetKeyDirectory();

    /// <summary>
    /// Checks if the targets specified by the user exist.
    /// </summary>
    private protected abstract void CheckTargets();

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
    [Option('t', "targets", Separator = ' ', Required = true, HelpText = "The target(s)")]
    public IEnumerable<string>? Targets { get; set; }

    /// <summary>
    /// A flag that indicates the specified directory for the key file
    /// </summary>
    [Option('k', "key", Required = true, HelpText = "The absolute path to the directory for the key file")]
    public string? KeyPath { get; set; }

    /// <summary>
    /// A flag that indicates the specified name for the key file 
    /// </summary>
    [Option('n', "name", Required = true, HelpText = "The name of the key file that will be generated inside the key directory")]
    public string? Name { get; set; }

    /// <summary>
    /// A flag that indicates the specified encryption algorithm
    /// </summary>
    [Option('a', "algorithm", HelpText = "The encryption algorithm. Supported: Aes, TripleDes, Des, Blowfish")]
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
            new Example("Encrypts the specified directory dir and the file fil using the directory key-directory",
                new EncryptOptions {
                    Targets = Enumerable.Empty<string>().Append<string>("dir/").Append<string>("fil"),
                    KeyPath = "key-directory/",
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
    [Option('t', "targets", Separator = ' ', Required = true, HelpText = "To specify the target(s)")]
    public IEnumerable<string>? Targets { get; set; }

    /// <summary>
    /// A flag that indicates the specified directory for the key file
    /// </summary>
    [Option('k', "key", Required = true, HelpText = "The absolute path to the directory for the key file")]
    public string? KeyPath { get; set; }

    /// <summary>
    /// Defines the examples for the help message.
    /// </summary>
    [Usage(ApplicationAlias = "crypass")]
    public static IEnumerable<Example> Examples
    {
        get
        {
            return new List<Example>() {
            new Example("Decrypts the specified directory dir and the file fil using the directory key-directory",
                new DecryptOptions {
                    Targets = Enumerable.Empty<string>().Append<string>("dir/").Append<string>("fil"),
                    KeyPath = "key-directory/",
                }),
            };
        }
    }
}