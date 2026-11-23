using EasyTransfer.Core.Logging;

namespace EasyTransfer.CLI;

internal static class Program
{
	public static void Main(string[] argv)
    {
        CLIMain cli = new(argv);

        cli.Run();
    }

    public static void PrintHelp()
    {
        Log.Print("Description:");
        Log.Print("  EasyTransfer.CLI");
        Log.Print("\nUsage:");
        Log.Print("  EasyTransfer.CLI <command> <address> <arguments>");
        Log.Print("\nCommand:");
        Log.Print("  send          Sending list of files");
        Log.Print("  recv          Receiving list of files");
        Log.Print("\nAddress:");
        Log.Print("  address:port  Specify the server's IP");
        Log.Print("\nArguments:");
        Log.Print("  files ...     For Sending command only, e.g Downloads/1.png, Downloads/2.png");
        Log.Print("  directory     For Receiving command only, e.g /tmp");
    }
}