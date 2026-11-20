using EasyTransfer.Core.Logging;

namespace EasyTransfer.CLI;

internal class Argument {
    // Positional Arguments
    private static readonly Argument<string> commandArg = new("send|recv") { Description = "Specify the action that this program will take" };
    private static readonly Argument<string> addressArg = new("address") { Description = "Specify either this machine address (if send command selected) or the server address (if recv command selected)" };
    private static readonly Argument<string[]> filesOrDirArg = new("files..|dir-path") { Description = "Pass list of files as arguments for sending or directory path where the received list of files stored" };
    private static readonly Option<bool> verboseOpt = new("-v", "--verbose") { Description = "Show what the program does do verbosely"};
    private static readonly RootCommand rootCommand = new("EasyTransfer is a program to send or recv list of files at the same LAN.");
    private static ParseResult? argv;

    public static void Parse(string[] args) {
        commandArg.AcceptOnlyFromAmong("send", "recv");

        rootCommand.Options.Add(verboseOpt);
        rootCommand.Arguments.Add(commandArg);
        rootCommand.Arguments.Add(addressArg);
        rootCommand.Arguments.Add(filesOrDirArg);

        argv = rootCommand.Parse(args);
        if(argv.Invoke() != 0) {
            Log.Error("Parsing the arguments: FAILED");
            Environment.Exit(1);
        }

        if(!ParseIPAndPort(argv.GetValue(addressArg))) {
            Log.Error("The specified address is invalid");
            Log.Error();
            Environment.Exit(1);
        }

        Program.command = argv.GetValue(commandArg);
        if(Program.command == "send") {
            foreach(string fileString in argv.GetValue(filesOrDirArg)) {
                FileInfo file = new (fileString);

                if(file.Exists) {
                    Program.fileList.Add(file);
                } else {
                    Log.Warn($"The file \"{file}\" doesn't exists");
                }
            }
        } else {
            Program.dirPath = new DirectoryInfo(argv.GetValue(filesOrDirArg)[0]);

            if(!Program.dirPath.Exists) {
                Log.Error($"The dir path \"{Program.dirPath}\"");
                Environment.Exit(1);
            }
        }
    }

    private static bool ParseIPAndPort(string address) {
        if(!address.Contains(':')) {
            return false;
        }

        string[] splitedAddr = address.Split(':', 2);
        if(splitedAddr.Length != 2) {
            return false;
        }

        string ip = splitedAddr[0];
        string port = splitedAddr[1];

        if(!IPAddress.TryParse(ip, out Program.ipAddress) || !int.TryParse(port, out Program.port)) {
            return false;
        }

        return true;
    }
}
