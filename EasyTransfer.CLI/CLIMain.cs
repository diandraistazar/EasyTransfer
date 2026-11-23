using System.Collections.ObjectModel;

using EasyTransfer.Core.Models.Filesystem;
using EasyTransfer.Core.Models;
using EasyTransfer.Core.Services;
using EasyTransfer.Core.Logging;
using EasyTransfer.CLI.Argument;

namespace EasyTransfer.CLI;

internal sealed class CLIMain 
{
    private DataContext _context;

    private TransferManagerService Manager { get; init; }

    public string? Command { get; set; }
    public string? Address { get; set; }
    public int Port { get; set; }
    public ObservableCollection<FileData> FileList { get; set; }
    public DirectoryData DestDirectory { get; set; }
    public int Progress { get; set; }

    public CLIMain(string[] argv)
    {
        _context = new DataContext(
            (() => Command, x => Command = x),
            (() => Address, x => Address = x),
            (() => Port, x => Port = x),
            (() => FileList, x => FileList = x),
            (() => DestDirectory, x => DestDirectory = x),
            (() => Progress, x => Progress = x)
        );

        Manager = new TransferManagerService(_context);

        FileList = new ObservableCollection<FileData>();
        DestDirectory = new DirectoryData(string.Empty, string.Empty, string.Empty);

        try
        {
            ArgumentParser.Parsing(argv, _context);
        }
        catch(Exception e)
        {
            Log.Error($"{e}\n");

            Program.PrintHelp();
            Environment.Exit(1);
        }
    }

    public void Run()
    {
        Log.Info("CLI session is running ...");

        if(Command == "send")
            Manager.Send();
        else
            Manager.Recv();
    }
}