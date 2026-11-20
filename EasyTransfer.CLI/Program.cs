namespace EasyTransfer.CLI;

using EasyTransfer.API;
using EasyTransfer.Types;
using EasyTransfer.Logging;

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.CommandLine;

static class Program
{
    public static string? command;
    public static IPAddress? ipAddress;
    public static int port;
    public static List<FileInfo> fileList = new();
    public static DirectoryInfo? dirPath;

    public static void Main(string[] argv)
    {
        Argument.Parse(argv);

        Log.Info("CLI session running");

        if (command == "send")
        {
            SendCommand();
        }
        else if (command == "recv")
        {
            RecvCommand();
        }
    }
}