using System.Text.RegularExpressions;

// Internal dependencies
using MaturaProject.Utilities;

// External dependencies
// CLI
using CommandLine;
using MaturaProject.CipherAlgorithms;

namespace MaturaProject.CLI;

/// <summary>
/// Represents a blueprint of the CLI for different Operating Systems.
/// Symbols used: ✔ ❌ ⚙ ℹ ⚠ 
/// </summary>
public abstract class Cli : Base
{
    /// <summary>
    /// The result of the CLI parser.
    /// </summary>
    private protected ParserResult<object> _parserResult;

    /// <summary>
    /// The specified arguments of the program call.
    /// </summary>
    private protected String[]? _args { get; init; }

    /// <summary>
    /// Initializes a new instance of the LinuxCli class.
    /// </summary>
    /// <param name="args">The specified arguments of the program call.</param>
    public Cli(string[] args)
    {
        _args = args;

        // Parse the command-line arguments
        MaturaProject.Log.Info("Parsing the command-line arguments: " + string.Join(" ", _args));
        _parserResult = Parser.Default.ParseArguments<EncryptOptions, DecryptOptions>(args)
            .WithParsed<EncryptOptions>(o =>
            {
                _encrypt = new EncryptOptions();

                _encrypt.Target = o.Target;
                _encrypt.DriveName = o.DriveName;
                _encrypt.Algorithm = o.Algorithm;
            }).WithParsed<DecryptOptions>(o =>
            {
                _decrypt = new DecryptOptions();

                _decrypt.Target = o.Target;
                _decrypt.DriveName = o.DriveName;
            }).WithNotParsed(HandleParseError);

        Init();
    }

    /// <summary>
    /// Prints a greet message to the standard output steam.
    /// </summary>
    private protected abstract void Greet();

    /// <summary>
    /// Handles the errors that occur when attempting to parse CLI arguments.
    /// </summary>
    /// <param name="errors">An enumerable object containing errors.</param>
    private protected void HandleParseError(IEnumerable<Error> errors)
    {
        MaturaProject.Log.Fatal("The required commands/flags have not been specified or could not have been parsed correctly");

        // TODO Do something with the parse errors here
        foreach (var error in errors)
        {
            if (error is HelpRequestedError)
                return;
        }

        // Console.WriteLine("Invalid command-line arguments");
    }

    public override void Init()
    {
        if (_parserResult.Errors.Any())
            return;

        // checks whether the help command was specified
        if (_parserResult.Errors.Any(e => e is HelpRequestedError))
            return;

        // Greet();

        RunOptions();
    }

    /// <summary>
    /// Runs the specified options.
    /// </summary>
    private protected void RunOptions()
    {
        DirectoryInfo? driveDirectory = GetDrive();

        MaturaProject.Log.Info("The drive '" + driveDirectory.Name + "' was specified (" + driveDirectory.FullName + ")");

        if (ConfirmSpecification() != 0)
        {
            WriteColor("\n[ℹ] [Terminating] the [execution]!\n", ConsoleColor.Blue);
            return;
        }

        WriteColor("\n[⚙] Executing the specifications...\n", ConsoleColor.Magenta);

        CipherManager cipherManager = new CipherManager();

        // encrypt and check if algorithm was specified
        // TODO pass the drive path to the encrypt method
        if (_encrypt is not null && _encrypt.Target is not null)
            if (_encrypt.Algorithm is null)
                cipherManager.Encrypt(_encrypt.Target, Cipher.AlgorithmType.Aes);
            else
                cipherManager.Encrypt(_encrypt.Target, (Cipher.AlgorithmType)Enum.Parse(typeof(Cipher.AlgorithmType), _encrypt.Algorithm));

        // decrypt
        else if (_decrypt is not null && _decrypt.Target is not null)
        {
            // TODO pass the keyData to the decrypt method
            List<(string, string, string)> keyData = DriveKey.GetKeyData(driveDirectory);
            cipherManager.Decrypt(_decrypt.Target);
        }
    }

    /// <summary>
    /// Prints a confirm message displaying all specified options and asks the user if he wants to continue.
    /// </summary>
    /// <returns>
    /// 0 - If the user wants to continue.
    /// -1 - If the user does not want to continue.
    /// </returns>
    public int ConfirmSpecification()
    {
        IEnumerable<string>? target = null;
        string? driveName = "";
        string? algorithm = "";

        // check if encrypt or decrypt was specified
        if (_encrypt is not null)
        {
            target = _encrypt.Target ?? throw new ErrorException("The [target] was not [specified]");
            driveName = _encrypt.DriveName ?? throw new ErrorException("The [drive] was not [specified]");

            algorithm = _encrypt.Algorithm;
        }

        else if (_decrypt is not null)
        {
            target = _decrypt.Target ?? throw new ErrorException("The [target] was not [specified]");
            driveName = _decrypt.DriveName ?? throw new ErrorException("The [drive] was not [specified]");
        }

        else throw new ErrorException("The [verb] was not [specified]");

        WriteColor("\n[(-)] The [" + (_encrypt is not null ? "encryption" : "decryption")
            + "] option has been specified.\n\n", ConsoleColor.Green);

        WriteColor("[(-)] The following files/directories are affected: \n", ConsoleColor.Green);

        foreach (var t in target)
        {
            WriteColor("\t[=>]\t[" + t + "]", ConsoleColor.Green);
        }

        WriteColor("\n\n[(-)] The drive '[" + driveName + "]' was specified.\n", ConsoleColor.Green);

        if (algorithm is not null)
            WriteColor("\n[(-)] The [" + algorithm + "] was specified.\n", ConsoleColor.Green);

        Console.WriteLine();

        return Confirm("Do you want to continue? [y/n] ") ? 0 : -1;
    }

    /// <summary>
    /// Prints the name of the program in ascii art to the standard output stream.
    /// </summary>
    public static void PrintProgramName()
    {
        // TODO ascii art with name of program
        Console.WriteLine(
            "\n                    __                           ____                                   __      \n /'\\_/`\\          /\\ \\__                       /\\  _`\\               __               /\\ \\__   \n/\\      \\     __  \\ \\ ,_\\  __  __  _ __    __  \\ \\ \\L\\ \\_ __   ___  /\\_\\     __    ___\\ \\ ,_\\  \n\\ \\ \\__\\ \\  /'__`\\ \\ \\ \\/ /\\ \\/\\ \\/\\`'__\\/'__`\\ \\ \\ ,__/\\`'__\\/ __`\\\\/\\ \\  /'__`\\ /'___\\ \\ \\/  \n \\ \\ \\_/\\ \\/\\ \\L\\.\\_\\ \\ \\_\\ \\ \\_\\ \\ \\ \\//\\ \\L\\.\\_\\ \\ \\/\\ \\ \\//\\ \\L\\ \\\\ \\ \\/\\  __//\\ \\__/\\ \\ \\_ \n  \\ \\_\\\\ \\_\\ \\__/.\\_\\\\ \\__\\\\ \\____/\\ \\_\\\\ \\__/.\\_\\\\ \\_\\ \\ \\_\\\\ \\____/_\\ \\ \\ \\____\\ \\____\\\\ \\__\\\n   \\/_/ \\/_/\\/__/\\/_/ \\/__/ \\/___/  \\/_/ \\/__/\\/_/ \\/_/  \\/_/ \\/___//\\ \\_\\ \\/____/\\/____/ \\/__/\n                                                                    \\ \\____/                   \n                                                                     \\/___/                    \n");
    }

    /// <summary>
    /// Writes the specified string message and parts of the message which are in square brackets in the specified color to
    /// the standard output stream.
    /// </summary>
    /// <param name="message">The message to write.</param>
    /// <param name="color">The color in which to write the message.</param>
    public static void WriteColor(string message, ConsoleColor color)
    {
        MaturaProject.Log.Info("Writing to the standard output stream: "
            + message.Replace("[", "").Replace("]", "").Replace("\n", "").Replace("\t", " "));

        var pieces = Regex.Split(message, @"(\[[^\]]*\])");

        foreach (var t in pieces)
        {
            var piece = t;

            if (piece.StartsWith("[") && piece.EndsWith("]"))
            {
                Console.ForegroundColor = color;
                piece = piece.Substring(1, piece.Length - 2);
            }

            Console.Write(piece);
            Console.ResetColor();
        }
    }

    /// <summary>
    /// Prints the specified string question to the standard output stream
    /// and reads the next line of characters from the standard input stream.
    /// </summary>
    /// <param name="question">The question to write.</param>
    /// <note name="note">The input is case insensitive.</note>
    /// <returns>
    /// <para/>True - if the standard input stream returns either yes or y 
    /// <para/>False - if the standard input stream returns either no or n and 
    /// <para/>Recourses again - if neither has been returned.
    /// </returns>
    public static bool Confirm(string question)
    {
        MaturaProject.Log.Info("Writing to the standard output stream: " + question);

        Console.Write(question);

        MaturaProject.Log.Info("Waiting for the user's response...");

        var key = Console.ReadLine();

        switch (key?.ToLowerInvariant())
        {
            case "y" or "yes":
                MaturaProject.Log.Info("Response: yes");
                return true;
            case "n" or "no":
                MaturaProject.Log.Info("Response: no");
                return false;
            default:
                MaturaProject.Log.Error("Response: unclear");
                return Confirm("Available option - yes / y / no / n : ");
        }
    }
}