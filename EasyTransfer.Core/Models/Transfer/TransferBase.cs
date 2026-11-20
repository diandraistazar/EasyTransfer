using System.Collections.ObjectModel;

using EasyTransfer.Core.Models.Filesystem;

namespace EasyTransfer.Core.Models.Transfer;

public abstract class TransferModelBase
{
    public TransferContext? Context;

    public string Address
    {
        get => Context.Address.Get();
        set => Context.Address.Set(value);
    }

    public int Port
    {
        get => Context.Port.Get();
        set => Context.Port.Set(value);
    }

    public ObservableCollection<FileData> FileList
    {
        get => Context.FileList.Get();
        set => Context.FileList.Set(value);
    }

    public DirectoryData DestDirectory
    {
        get => Context.DestDirectory.Get();
        set => Context.DestDirectory.Set(value);
    }

    public int Progress
    {
        get => Context.Progress.Get();
        set => Context.Progress.Set(value);
    }
}