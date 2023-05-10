using System;

namespace MaturaProject.CLI
{
    public class FatalException : Exception
    {
        /// <summary>
        /// Logs a fatal error message replacing '[' and ']' and prints the message to the standard output, 
        /// coloring the contents inside [] and ending it with an exclamation mark.
        /// </summary>
        /// <param name="message">The message to print.</param>
        public FatalException(string message) : base(message)
        {
            MaturaProject.Log.Error(message.Replace("[", "").Replace("]", ""));
            Cli.WriteColor("[❌❌❌] " + message + "!", ConsoleColor.Red);
        }

        /// <summary>
        /// Logs a fatal error message replacing '[' and ']' and prints the message to the standard output, 
        /// coloring the contents inside [] and ending it with an exclamation mark.
        /// </summary>
        /// <param name="message">The message to print.</param>
        /// <param name="e">The Exception.</param>
        public FatalException(string message, Exception e) : base(message, e)
        {
            MaturaProject.Log.Error(message.Replace("[", "").Replace("]", ""));
            Cli.WriteColor("[❌❌❌] " + message + "!", ConsoleColor.Red);
        }
    }

    public class ErrorException : Exception
    {
        /// <summary>
        /// Logs an error message replacing '[' and ']' and prints the message to the standard output, 
        /// coloring the contents inside [] and ending it with an exclamation mark.
        /// </summary>
        /// <param name="message">The message to print.</param>
        public ErrorException(string message) : base(message)
        {
            MaturaProject.Log.Error(message.Replace("[", "").Replace("]", ""));
            Cli.WriteColor("[❌] " + message + "!", ConsoleColor.Red);
        }

        /// <summary>
        /// Logs an error message replacing '[' and ']' and prints the message to the standard output, 
        /// coloring the contents inside [] and ending it with an exclamation mark.
        /// </summary>
        /// <param name="message">The message to print.</param>
        /// <param name="e">The exception.</param>
        public ErrorException(string message, Exception e) : base(message, e)
        {
            MaturaProject.Log.Error(message.Replace("[", "").Replace("]", ""));
            Cli.WriteColor("[❌] " + message + "!", ConsoleColor.Red);
        }
    }
}