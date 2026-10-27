namespace EasyTransfer.Console;

using EasyTransfer.API;
using EasyTransfer.Types;
using EasyTransfer.Logging;

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.CommandLine;

class Program {
    public static string command;
    public static IPAddress ipAddress;
    public static int port;
    public static List<FileInfo> fileList = new ();
    public static DirectoryInfo dirPath;

    public static void Main(string[] argv) {
        Argument.Parse(argv);

        Log.Info("CLI session running");

        if(command == "send") {
            SendCommand();
        } else if(command == "recv") {
            RecvCommand();
        }
    }

    public static void SendCommand() {
        TcpListener? server = Network.SetupServer(ipAddress, port);

        Log.Info("Server listening ... ", '\0');
        TcpClient? client = server.AcceptTcpClient();

        Log.Print($"{client.Client.RemoteEndPoint} connected");

        FileInfo? file;
        Status? status;
        byte[]? statusBytes;
        int bytes;

        for(int i = 0; i < fileList.Count; i++) {
            file = fileList[i];
            FileHeader header = new () {
                Index = i,
                Filename = file.Name,
                Length = file.Length,
                Next = i == fileList.Count-1 ? false : true
            };

            byte[] headerBytes = FileHeader.Serialize(header);
            Network.SendData(client, headerBytes);

            bytes = Network.ReceiveData(client, 128, out statusBytes);
            status = Status.Deserialize(statusBytes.Take(bytes).ToArray());
            if(status?.Message != Status.iGot) {
                i--;
                continue;
            }

            Network.SendFile(client, file.FullName);

            bytes = Network.ReceiveData(client, 128, out statusBytes);
            status = Status.Deserialize(statusBytes.Take(bytes).ToArray());
            if(status?.Message != Status.iGot) {
                i--;
                continue;
            }

            Log.Succ($"Successfully sending \"{file.Name}\"");
        }
            
        server.Stop();
        client.Client.Shutdown(SocketShutdown.Both);
        client.Close();
    }

    public static void RecvCommand() {
        TcpClient client = Network.SetupClient(ipAddress, port);

        Log.Info($"Connected to {client.Client.RemoteEndPoint}");

        FileHeader? header;
        byte[] iGotBytes = Status.Serialize(new Status() { Message = Status.iGot });
        byte[] resendStatus = Status.Serialize(new Status() { Message = Status.resendPls });
        byte[]? buffer;
        bool looping = true;

        while(looping) {
            Network.ReceiveData(client, 128, out buffer);
            try {
                header = FileHeader.Deserialize(buffer);
            } catch {
                Network.SendData(client, resendStatus);
                continue;
            }
                
            if(header?.Next == false) { // using !(bool)header?.Next also works but not as readable as this
                                        // Basically, header?.Next returns bool? not bool, this is because of ? keyword.
                                        // bool means either true or false, however bool? means either true, false, or null
                                        // So that's we need to cast from bool? to bool using (bool) cast.
                looping = false;
            }

            Network.SendData(client, iGotBytes);
                
            try {
                Network.ReceiveFile(client, (int)header?.Length, Path.Combine(dirPath.FullName, header?.Filename));
            } catch {
                Network.SendData(client, resendStatus);
                continue;
            }

            Network.SendData(client, iGotBytes);
            Log.Succ($"Successfully receiving \"{header?.Filename}\", store to \"{dirPath}\"");
        }

        client.Client.Shutdown(SocketShutdown.Both);
        client.Close();
    }
}

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
            Log.Error("Failure while parsing program arguments");
            Environment.Exit(1);
        }

        if(!ParseIPAndPort(argv.GetValue(addressArg))) {
            Log.Error("The specified address is invalid");
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