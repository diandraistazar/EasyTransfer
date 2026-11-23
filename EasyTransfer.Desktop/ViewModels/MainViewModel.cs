using CommunityToolkit.Mvvm.Input;
using Avalonia.Platform.Storage;
using Avalonia.Controls;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using EasyTransfer.Core.Models.Filesystem;
using EasyTransfer.Core.Models;
using EasyTransfer.Core.Services;
using EasyTransfer.Core.Logging;

namespace EasyTransfer.Desktop.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly DataContext _context;

    private string _command = string.Empty;
    private string _address = string.Empty;
    private int _port = 0;
    private int _progress = 0;
    private DirectoryData? _destDirectory = default;

    private TransferManagerService Manager { get; init; }
    private FilePickerService FilePicker { get; init; }

    public MainViewModel(IStorageProvider storageProvider)
    {
        _context = new DataContext(
            (() => Command, x => Command = x),
            (() => Address, x => Address = x),
            (() => Port, x => Port = x),
            (() => FileListDialog, x => FileListDialog = x),
            (() => DestDirectory, x => DestDirectory = x),
            (() => Progress, x => Progress = x)
        );

        Manager = new TransferManagerService(_context);
        FilePicker = new FilePickerService(storageProvider);

        FileListDialog = new ObservableCollection<FileData>();
        HistoryTransfer = new ObservableCollection<string>();

        FileListDialog.CollectionChanged += (_, _) => StartSendingCommand.NotifyCanExecuteChanged();
    }

    public ObservableCollection<string> HistoryTransfer { get; set; }
    public ObservableCollection<FileData> FileListDialog { get; set; }
    public DirectoryData? DestDirectory
    {
        get => _destDirectory;
        set
        {
            SetProperty(ref _destDirectory, value);
            StartReceivingCommand.NotifyCanExecuteChanged();
        }
    }
    
    public bool IsSendRecvDialogOpen => Command != string.Empty;
    public bool IsSendDialog => Command == "Send";
    public bool IsRecvDialog => Command == "Recv"; 

    public bool CanStartSend() => FileListDialog.Count > 0;
    public bool CanStartRecv() => DestDirectory != default;

    public string Command
    {
        get => _command;
        set
        {
            SetProperty(ref _command, value);
            RaiseProperty(nameof(IsSendRecvDialogOpen));
            RaiseProperty(nameof(IsSendDialog));
            RaiseProperty(nameof(IsRecvDialog));
        }
    }

    public string Address
    {
        get => _address;
        set => SetProperty(ref _address, value);
    }

    public int Port
    {
        get => _port;
        set => SetProperty(ref _port, value);
    }

    public int Progress
    {
        get => _progress;
        set
        {
            SetProperty(ref _progress, value);
        }
    }

    // -------------------------------------------- Relay commands

    [RelayCommand]
    public void ShowSendDialog() => Command = "Send";

    [RelayCommand]
    public void ShowRecvDialog() => Command = "Recv";

    [RelayCommand]
    public void CloseSendRecvDialog()
    {
        FileListDialog.Clear();
        DestDirectory = default;
        Command = string.Empty;
        Address = string.Empty;
        Port = 0;
    }

    [RelayCommand(CanExecute = nameof(CanStartSend))]
    public async Task StartSending(object? e)
    {
        if(e is Button button)
            Manager.Send();
    }

    [RelayCommand(CanExecute = nameof(CanStartRecv))]
    public async Task StartReceiving(object? e)
    {
        if(e is Button button)
            Manager.Recv();
    }

    // -------------------------------------------- Picking files relay commands
    
    [RelayCommand]
    public async Task PickFilesDialog()
    {
        FileData[] fileList = await FilePicker.PickFilesAsync() ?? new FileData[0];

        foreach(var x in fileList)
            FileListDialog.Add(x);
    }

    [RelayCommand]
    public async Task PickDirectoryDialog()
    {
        DestDirectory = await FilePicker.PickDirectoryAsync() ?? default;
    }
}
