using System.Collections.ObjectModel;

using EasyTransfer.Core.Models.Filesystem;

namespace EasyTransfer.Core.Models;

public record class DataContext
{
    private readonly (Func<string> Get, Action<string> Set) _command;
    private readonly (Func<string> Get, Action<string> Set) _address;
    private readonly (Func<int> Get, Action<int> Set) _port;
    private readonly (Func<ObservableCollection<FileData>> Get, Action<ObservableCollection<FileData>> Set) _fileList; 
    private readonly (Func<DirectoryData> Get, Action<DirectoryData> Set) _destDirectory;
    private readonly (Func<int> Get, Action<int> Set) _progress;

    public DataContext(
        (Func<string> Get, Action<string> Set) Command,
        (Func<string> Get, Action<string> Set) Address,
        (Func<int> Get, Action<int> Set) Port,
        (Func<ObservableCollection<FileData>> Get, Action<ObservableCollection<FileData>> Set) FileList,
        (Func<DirectoryData> Get, Action<DirectoryData> Set) DestDirectory,
        (Func<int> Get, Action<int> Set) Progress
    )
    {
        _command = Command;
        _address = Address;
        _port = Port;
        _fileList = FileList;
        _destDirectory = DestDirectory;
        _progress = Progress;
    }

    public string? Command
    {
        get => _command.Get();
        set => _command.Set(value);
    }

    public string? Address
    {
        get => _address.Get();
        set => _address.Set(value);
    }

    public int Port
    {
        get => _port.Get();
        set => _port.Set(value);
    }

    public ObservableCollection<FileData>? FileList
    {
        get => _fileList.Get();
        set => _fileList.Set(value);
    }

    public DirectoryData? DestDirectory
    {
        get => _destDirectory.Get();
        set => _destDirectory.Set(value);
    }

    public int Progress
    {
        get => _progress.Get();
        set => _progress.Set(value);
    }
}