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

    private TransferManager Manager { get; init; }

    public MainViewModel()
    {
        Manager = new();

        DestDirectory = new DirectoryData(string.Empty, string.Empty, string.Empty);
        FileListDialog = new ObservableCollection<FileData>();
        HistoryTransfer = new ObservableCollection<string>();

        FileListDialog.CollectionChanged += (_, _) => StartSendingCommand.NotifyCanExecuteChanged();
    }

    public ObservableCollection<string> HistoryTransfer { get; set; }
    public ObservableCollection<FileData> FileListDialog { get; set; }
    public DirectoryData? DestDirectory
    {
        get => _destDirectory;
        set => SetProperty(ref _destDirectory, value);
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
        Address = string.Empty;
        Port = 0;
        FileListDialog.Clear();
        DestDirectory = default;
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
    public async Task PickFiles(object? e)
    {
        if(e is Visual visual)
        {
            TopLevel? topLevel = TopLevel.GetTopLevel(visual);

            if(topLevel == null)
                return;

            if(!topLevel.StorageProvider.CanOpen)
            {
                Log.Warn("Cannot open a file manager for picking files");
                return;
            }

            var itemList = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
            {
                Title = "Pick Files",
                AllowMultiple = true
            });

            foreach(var x in itemList)
                FileListDialog.Add(new FileData(
                    x.Name, x.Path.LocalPath,
                    new Uri(x.Path, ".").LocalPath,
                    new FileInfo(x.Path.LocalPath).Length
                ));
        }
    }

    [RelayCommand]
    public async Task PickDestDirectory(object? e)
    {
        if(e is Visual visual)
        {
            TopLevel? topLevel = TopLevel.GetTopLevel(visual);

            if(topLevel == null)
                return;

            if(!topLevel.StorageProvider.CanPickFolder)
            {
                Log.Warn("Cannot open a file manager for picking a directory");
                return;
            }

            var directory = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
            {
                Title = "Pick a Directory",
                AllowMultiple = false
            });

            if(directory.Count > 0)
            {
                var dir = directory[0];

                DestDirectory = new(dir.Name, dir.Path.LocalPath, new Uri(dir.Path, "..").LocalPath);
            }
            else
                Log.Warn("Directory have not been selected yet");
        }
    }
}
