using System.Text.RegularExpressions;

// Internal dependencies
using MaturaProject.Utilities;

// External dependencies
// CLI
using CommandLine;
using MaturaProject.Cipher;
using MaturaProject.Cipher.Algorithm;

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

                _encrypt.Targets = o.Targets;
                _encrypt.KeyPath = o.KeyPath;
                _encrypt.Name = o.Name;
                _encrypt.Algorithm = o.Algorithm;
            }).WithParsed<DecryptOptions>(o =>
            {
                _decrypt = new DecryptOptions();

                _decrypt.Targets = o.Targets;
                _decrypt.KeyPath = o.KeyPath;
            }).WithNotParsed(HandleParseError);

        Init();
    }

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
    }

    public override void Init()
    {
        if (_parserResult.Errors.Any())
            return;

        // checks whether the help command was specified
        if (_parserResult.Errors.Any(e => e is HelpRequestedError))
            return;

        Run();
    }

    /// <summary>
    /// Runs the specified command line options.
    /// </summary>
    private protected void Run()
    {
        DirectoryInfo? keyDirectory = GetKeyDirectory();
        CheckTargets();

        MaturaProject.Log.Info("The drive '" + keyDirectory.Name + "' was specified (" + keyDirectory.FullName + ")");

        if (ConfirmSpecification() != 0)
        {
            WriteColor("\n[ℹ] [Terminating] the [execution]!\n", ConsoleColor.Blue);
            return;
        }

        WriteColor("\n[⚙] Executing the specifications...", ConsoleColor.Magenta);

        CipherManager cipherManager = new CipherManager();

        if (_encrypt is not null && _encrypt.Targets is not null && _encrypt.Name is not null)
            if (_encrypt.Algorithm is null)
                cipherManager.Encrypt(_encrypt.Targets, keyDirectory.FullName, _encrypt.Name);
            else
                cipherManager.Encrypt(_encrypt.Targets, keyDirectory.FullName, _encrypt.Name, (AlgorithmType)Enum.Parse(typeof(AlgorithmType), _encrypt.Algorithm));

        else if (_decrypt is not null && _decrypt.Targets is not null)
        {
            List<(string, string, string)> keyData = IDriveKey.GetKeyData(keyDirectory);
            cipherManager.Decrypt(_decrypt.Targets, keyData);
        }

        WriteColor("\n[✔] [Done]!\n", ConsoleColor.Green);
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
        string? keyPath = "";

        string? name = null;
        string? algorithm = "";

        // check if encrypt or decrypt was specified
        if (_encrypt is not null)
        {
            target = _encrypt.Targets ?? throw new ErrorException("The [target] was not [specified]");
            keyPath = _encrypt.KeyPath ?? throw new ErrorException("The [key] was not [specified]");

            name = _encrypt.Name ?? throw new ErrorException("The [name] was not [specified]");
            algorithm = _encrypt.Algorithm;

            if (algorithm is not null)
            {
                try { var unused = (AlgorithmType)Enum.Parse(typeof(AlgorithmType), algorithm); }
                catch (ArgumentException) { throw new ErrorException("The algorithm [algorithm] is not supported. Try: Aes, TripleDes, Des, Blowfish"); }
            }
        }

        else if (_decrypt is not null)
        {
            target = _decrypt.Targets ?? throw new ErrorException("The [target] was not [specified]");
            keyPath = _decrypt.KeyPath ?? throw new ErrorException("The [key] was not [specified]");
        }

        else throw new ErrorException("The [verb] was not [specified]");

        WriteColor("\n[(-)] The [" + (_encrypt is not null ? "encryption" : "decryption")
            + "] option has been specified.\n\n", ConsoleColor.Green);

        WriteColor("[(-)] The following files/directories are affected: \n", ConsoleColor.Green);

        foreach (var t in target)
        {
            WriteColor("\t[=>]\t[" + t + "]", ConsoleColor.Green);
        }

        WriteColor("\n\n[(-)] The drive '[" + keyPath + "]' was specified.\n", ConsoleColor.Green);

        WriteColor("\n[(-)] The name [" + name + "] was specified.\n", ConsoleColor.Green);

        if (algorithm is not null)
            WriteColor("\n[(-)] The algorithm [" + algorithm + "] was specified.\n", ConsoleColor.Green);

        else
            WriteColor("\n[(-)] The standard algorithm [Aes] will be used.\n", ConsoleColor.Green);


        Console.WriteLine();

        return Confirm("Do you want to continue? [y/n] ") ? 0 : -1;
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