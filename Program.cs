namespace EasyTransfer;

using EasyTransfer.Backend;

using System.IO;
using System.CommandLine;
using System.Net;

class Program {
    public static string? command;
    public static IPAddress? ipAddress;
    public static int Port;
    public static string[]? listOfFiles;
    public static string? dirPath; 

    // Positional Arguments
    private static readonly Argument<string> commandArg = new("send|recv") { Description = "Specify the action that this program will take" };
    private static readonly Argument<string> addressArg = new("address") { Description = "Specify either this machine address (if send command selected) or the server address (if recv command selected)" };
    private static readonly Argument<string[]> filesOrDirArg = new("files..|dir-path") { Description = "Pass list of files as arguments for sending or directory path where the received list of files stored" };
    private static readonly Option<bool> verboseOpt = new("-v", "--verbose") { Description = "Show what the program does do verbosely"};
    private static readonly RootCommand rootCommand = new("EasyTransfer is a program to send or recv list of files at the same LAN.");
    private static ParseResult? argv;

    public static void Main(string[] args) {
        ValueTuple<int, int> cursorPos = Console.GetCursorPosition();

        if((cursorPos.Item1 | cursorPos.Item2) != 0) {
            ParseArguments(args);
            CLI.Run();
        } else {
            GUI.Run();
        }
    }

    private static void ParseArguments(string[] args) {
        commandArg.AcceptOnlyFromAmong("send", "recv");

        rootCommand.Options.Add(verboseOpt);
        rootCommand.Arguments.Add(commandArg);
        rootCommand.Arguments.Add(addressArg);
        rootCommand.Arguments.Add(filesOrDirArg);

        argv = rootCommand.Parse(args);
        if(argv.Invoke() != 0) {
            Log.Error("Failure while parsing program arguments");
            Exit(1);
        }

        command = argv.GetValue(commandArg);

        if(!ParseIPAndPort(argv.GetValue(addressArg))) {
            Log.Error("The specified address is invalid");
            Exit(1);
        }

        if(command == "send") {
            listOfFiles = argv.GetValue(filesOrDirArg);
        } else {
            dirPath = argv.GetValue(filesOrDirArg)[0];
            
            if(!Directory.Exists(dirPath)) {
                Log.Error($"The specified directory path \"{dirPath}\" doesn't exists");
                Exit(1);
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

        if(!IPAddress.TryParse(ip, out ipAddress) || !int.TryParse(port, out Port)) {
            return false;
        }

        try {
            IPHostEntry host = Dns.GetHostEntry(ipAddress);
        } catch(Exception e) {
            Log.Error($"{e} occured while testing the specified address");
            return false;
        }

        return true;
    }

    public static void Exit(int exitCode) {
        Network.CloseSocket();

        Environment.Exit(exitCode);
    }
}

class GUI {
    public static void Run() {

    }
}

class CLI {
    public static void Run() {
        if(Program.command == "send") {
            if(!Network.SetupServer()) {
                Log.Error("Error occured while setup the server");
            } else {
                Log.Succ("Successfully to setup the server");
            }

            if(!Network.Send()) {
                Log.Error("Error occured while sending list of resources");
                Program.Exit(1);
            } else {
                Log.Succ("Sending done");
            }
        } else {
            if(!Network.SetupClient()) {
                Log.Error("Error occured while setup the client");
            } else {
                Log.Succ("Successfully to setup the client");
            }

            if(!Network.Recv()) {
                Log.Error("Error occured while receiving list of files from the server");
                Program.Exit(1);
            } else {
                Log.Succ("Receiving done");
            }
        }
        
        Network.CloseSocket();
    }
}

class Log {
    public static void Debug(string message) {
        Console.WriteLine($"[DEB] {message}");
    }
    public static void Info(string message) {
        Console.WriteLine($"[INF] {message}");
    }
    public static void Warn(string message) {
        Console.WriteLine($"[WAR] {message}");
    }
    public static void Succ(string message) {
        Console.WriteLine($"[SUC] {message}");
    }
    public static void Error(string message) {
        Console.WriteLine($"[ERR] {message}");
    }
}