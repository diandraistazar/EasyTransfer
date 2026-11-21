using CommunityToolkit.Mvvm.Input;
using Avalonia;
using Avalonia.Platform.Storage;
using Avalonia.Controls;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System;

using EasyTransfer.Core.Models.Filesystem;
using EasyTransfer.Core.Models.Transfer;
using EasyTransfer.Core.Services;
using EasyTransfer.Core.Logging;

namespace EasyTransfer.Desktop.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private string _address = string.Empty;
    private int _port = 0;
    private int _dialog = 0;
    private int _progress = 0;
    private DirectoryData? _destDirectory = default;

    private TransferManagerService Manager { get; init; }
    private FilePickerService FilePicker { get; init; }

    public MainViewModel(IStorageProvider storageProvider)
    {
        Manager = new TransferManagerService();
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
    
    public bool IsSendRecvDialogOpen => Dialog != 0;
    public bool IsSendDialog => Dialog == 1;
    public bool IsRecvDialog => Dialog == 2; 

    public bool CanStartSend() => FileListDialog.Count > 0;
    public bool CanStartRecv() => DestDirectory != default;

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

    public int Dialog
    { 
        get => _dialog;
        set
        {
            SetProperty(ref _dialog, value);
            RaiseProperty(nameof(IsSendRecvDialogOpen));
            RaiseProperty(nameof(IsSendDialog));
            RaiseProperty(nameof(IsRecvDialog));
        }
    }

    public int ProgressDialog
    {
        get => _progress;
        set
        {
            SetProperty(ref _progress, value);
        }
    }

    // -------------------------------------------- Relay commands

    [RelayCommand]
    public void ShowSendDialog() => Dialog = 1;

    [RelayCommand]
    public void ShowRecvDialog() => Dialog = 2;

    [RelayCommand]
    public void CloseSendRecvDialog()
    {
        FileListDialog.Clear();
        DestDirectory = default;
        Address = string.Empty;
        Port = 0;
        Dialog = 0;
    }

    [RelayCommand(CanExecute = nameof(CanStartSend))]
    public async Task StartSending(object? e)
    {
        if(e is Button button)
            Manager.Send(new TransferContext(
                (() => Address, x => Address = x),
                (() => Port, x => Port = x),
                (() => FileListDialog, x => FileListDialog = x),
                (() => DestDirectory, x => DestDirectory = x),
                (() => ProgressDialog, x => ProgressDialog = x)
            ));
    }

    [RelayCommand(CanExecute = nameof(CanStartRecv))]
    public async Task StartReceiving(object? e)
    {
        if(e is Button button)
            Manager.Recv(new TransferContext(
                (() => Address, x => Address = x),
                (() => Port, x => Port = x),
                (() => FileListDialog, x => FileListDialog = x),
                (() => DestDirectory, x => DestDirectory = x),
                (() => ProgressDialog, x => ProgressDialog = x)
            ));
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
