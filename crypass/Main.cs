using System.Runtime.InteropServices;

// Internal dependencies
using MaturaProject.Utilities;

// External dependencies
using log4net;

namespace MaturaProject;

/// <summary>
/// TODO
/// </summary>
public static class MaturaProject
{
    /// <summary>
    /// The logger used for logging into {CurrentDir}/Local/log/crypass.log.
    /// </summary>
    /// <returns></returns>
    public static readonly ILog Log = LogManager.GetLogger(typeof(MaturaProject));

    /// <summary>
    /// TODO
    /// </summary>
    /// <param name="args">The specified arguments of the program call.</param>
    public static int Main(string[] args)
    {
        try
        {
            Base? program = null;

            //  check if OS is Linux
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                log4net.Config.XmlConfigurator.Configure(new FileInfo(Directory.GetCurrentDirectory() + "/Local/log/log4net.config"));
                Log.Info("Detected Linux");

                // TODO check for GUI, Log.Info(...gui...) and create GUI instead of CLI
                Log.Info("Creating Linux CLI");
                program = new CLI.LinuxCli(args);
            }

            // check if OS is Windows
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                log4net.Config.XmlConfigurator.Configure(new FileInfo(Directory.GetCurrentDirectory() + "\\Local\\log\\log4net.config"));
                Log.Info("Detected Windows");

                // TODO check for GUI, Log.Info(...gui...) and create GUI instead of CLI
                Log.Info("Creating Windows CLI");
                program = new CLI.WindowsCli(args);
            }

            if (program == null)
            {
                throw new CLI.FatalException("The detected [OS] is [not supported]");
            }

            Log.Info("Program exited normally");
        }
        catch (CLI.ErrorException e)
        {
            Log.Error("Program exited wih: " + e.Message.Replace("[", "").Replace("]", ""));
        }
        catch (CLI.FatalException e)
        {
            Log.Fatal("Program exited wih: " + e.Message.Replace("[", "").Replace("]", ""));
        }
        catch (Exception e)
        {
            Log.Fatal("Program exited wih: " + e.Message);
        }

        return 0;
    }
}